using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.SteadFast.Domain;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Logging;
using System.Text.Json;

namespace Nop.Plugin.Shipping.SteadFast.Controllers;

public class SteadFastWebhookController : Controller
{
    #region Fields

    private readonly ISteadFastShipmentEventLogService _eventLogService;
    private readonly ISteadFastShipmentRecordService _shipmentRecordService;
    private readonly ILogger _logger;
    private readonly SteadFastSettings _steadFastSettings;

    #endregion

    #region Ctor

    public SteadFastWebhookController(
        ISteadFastShipmentEventLogService eventLogService,
        ISteadFastShipmentRecordService shipmentRecordService,
        ILogger logger,
        SteadFastSettings steadFastSettings)
    {
        _eventLogService = eventLogService;
        _shipmentRecordService = shipmentRecordService;
        _logger = logger;
        _steadFastSettings = steadFastSettings;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> WebhookHandler()
    {
        try
        {
            // Check if webhook is enabled
            if (!_steadFastSettings.WebhookEnabled)
            {
                await _logger.WarningAsync("SteadFast webhook received but webhook is disabled");
                return Ok();
            }

            // Read the request body
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(body))
            {
                await _logger.ErrorAsync("SteadFast webhook received with empty body");
                return BadRequest();
            }

            // Parse webhook payload
            var webhookData = JsonSerializer.Deserialize<JsonElement>(body);

            // Extract data based on SteadFast webhook format
            var consignmentId = webhookData.GetProperty("consignment_id").GetString();
            var status = webhookData.GetProperty("status").GetString();
            var message = webhookData.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "";

            if (string.IsNullOrEmpty(consignmentId))
            {
                await _logger.ErrorAsync("SteadFast webhook received without consignment ID");
                return BadRequest();
            }

            // Get existing shipment record
            var shipmentRecords = await _shipmentRecordService.GetAllShipmentRecordsAsync(consignmentId: consignmentId);
            var shipmentRecord = shipmentRecords.FirstOrDefault();

            if (shipmentRecord == null)
            {
                await _logger.WarningAsync($"SteadFast webhook received for unknown consignment ID: {consignmentId}");
                // Still log the event even if we don't have a record
            }

            // Create event log
            var eventLog = new SteadFastShipmentEventLog
            {
                ShipmentId = shipmentRecord?.ShipmentId ?? 0,
                ConsignmentId = consignmentId,
                EventType = "StatusChanged",
                OldStatus = shipmentRecord?.DeliveryStatus ?? "",
                NewStatus = status,
                StatusMessage = message,
                WebhookPayload = body,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _eventLogService.InsertEventLogAsync(eventLog);

            // Update shipment record if it exists
            if (shipmentRecord != null)
            {
                shipmentRecord.DeliveryStatus = status;
                shipmentRecord.UpdatedOnUtc = DateTime.UtcNow;
                await _shipmentRecordService.UpdateShipmentRecordAsync(shipmentRecord);
            }

            await _logger.InformationAsync($"SteadFast webhook processed successfully for consignment ID: {consignmentId}, Status: {status}");

            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error processing SteadFast webhook", ex);
            return StatusCode(500);
        }
    }

    #endregion
}
