# SteadFast Shipping Plugin for nopCommerce 4.70

## Overview

This plugin integrates SteadFast courier service (https://steadfast.com.bd/) into nopCommerce 4.70, providing shipping rate calculation and automated shipment management through the SteadFast API.

## Features

### Core Features
- **Shipping Rate Calculation**: Supports both fixed rates and calculation by weight/order total
- **Automatic Shipment Creation**: Automatically creates shipments on SteadFast when orders are placed
- **Manual Shipment Management**: Create shipments manually from the admin panel
- **Bulk Operations**: Create multiple shipments at once
- **Shipment Tracking**: Track delivery status and update tracking numbers
- **Balance Checking**: View current SteadFast account balance
- **Flexible Configuration**: Support for sandbox mode and customizable settings

### Technical Features
- Full nopCommerce 4.70 plugin architecture compliance
- Dependency injection and repository pattern
- Event-driven architecture with EntityInsertedEvent consumer
- FluentMigrator database migrations
- Localized resource strings
- Admin area with Razor views
- RESTful API client for SteadFast integration

## Installation

### Prerequisites
- nopCommerce 4.70
- .NET 8.0
- SteadFast merchant account with API credentials

### Steps

1. **Build the Plugin**
   ```bash
   cd src/Plugins/Nop.Plugin.Shipping.SteadFast
   dotnet build
   ```

2. **Deploy to nopCommerce**
   The build process automatically copies the plugin DLL and related files to:
   ```
   src/Presentation/Nop.Web/Plugins/Shipping.SteadFast/
   ```

3. **Install via Admin**
   - Navigate to **Admin > Configuration > Local plugins**
   - Find "SteadFast Shipping" in the list
   - Click **Install**
   - Restart the application when prompted

4. **Configure the Plugin**
   - Navigate to **Admin > Configuration > Shipping > Shipping Providers**
   - Click **Configure** next to "SteadFast Shipping"
   - Enter your SteadFast API credentials:
     - **API Key**: Your SteadFast API key
     - **API Secret Key**: Your SteadFast API secret key
   - Configure settings:
     - **Use Sandbox**: Enable for testing with test environment
     - **Auto Create Shipment**: Enable to automatically create shipments when orders are placed
     - **Default Note**: Default note to include in shipments
     - **Calculate by Weight/Total**: Enable shipping rate calculation by weight and order total
     - **Limit to Configured Methods**: Only show configured shipping methods
   - Click **Save**

5. **Test Connection**
   - Click the **Test Connection** button to verify API credentials
   - If successful, your current balance will be displayed

## Configuration

### API Credentials

To obtain API credentials:
1. Log in to your SteadFast merchant dashboard
2. Navigate to API settings
3. Generate or copy your API Key and Secret Key
4. Contact SteadFast support if you need assistance

### Shipping Rate Configuration

#### Fixed Rate Mode
When "Calculate by Weight/Total" is disabled:
- Configure fixed rates for each shipping method via **Configuration > Shipping > Shipping Methods**
- Each method can have its own fixed rate and transit days

#### Weight/Total Calculation Mode
When "Calculate by Weight/Total" is enabled:
- Create rate rules based on:
  - Order weight range
  - Order subtotal range
  - Destination country/state/zip
  - Store
  - Warehouse
- Configure per shipping method:
  - Additional fixed cost
  - Rate per weight unit
  - Percentage of subtotal
  - Lower weight limit
  - Transit days

### Automatic Shipment Creation

When enabled, the plugin will automatically:
1. Listen for shipment creation events
2. Check if the order uses SteadFast shipping
3. Verify the order is not for pickup in store
4. Extract order and shipping address details
5. Call SteadFast API to create shipment
6. Store shipment details and tracking number
7. Update nopCommerce shipment with tracking number

## Usage

### Manual Shipment Creation

1. Navigate to **Admin > Configuration > Shipping Providers**
2. Click **Configure** next to "SteadFast Shipping"
3. Click **Manage Shipments**
4. Use the shipment grid to:
   - View all SteadFast shipments
   - Filter by order ID or consignment ID
   - Check delivery status
   - View tracking numbers

### Creating Individual Shipments

From the order details page:
1. Navigate to **Admin > Sales > Orders**
2. Open an order
3. Use the SteadFast integration to create shipment manually

### Checking Shipment Status

The plugin stores:
- Consignment ID from SteadFast
- Invoice number
- Recipient details
- COD amount
- Delivery status
- Tracking number
- API response for debugging

You can update delivery status by calling the SteadFast status API.

## API Integration

### Endpoints Used

1. **Create Order** (`POST /api/v1/create_order`)
   - Creates a new shipment on SteadFast
   - Required fields: invoice, recipient details, COD amount
   - Returns: consignment ID and tracking code

2. **Get Balance** (`GET /api/v1/get_balance`)
   - Retrieves current account balance
   - Used for connection testing

3. **Get Status by Consignment ID** (`GET /api/v1/status_by_cid/{id}`)
   - Retrieves delivery status for a shipment
   - Returns: current delivery status and details

### Request/Response Models

All API models are defined in:
- `Models/Api/CreateOrderModels.cs`
- `Models/Api/OtherApiModels.cs`

### Error Handling

The plugin includes comprehensive error handling:
- API connection failures
- Invalid credentials
- Missing shipping addresses
- Duplicate shipment prevention
- Logging of all errors

## Database Schema

### SteadFastShipmentRecord Table
Stores shipment data synchronized with SteadFast:
- ShipmentId (FK to nopCommerce Shipment)
- OrderId (FK to nopCommerce Order)
- ConsignmentId (from SteadFast)
- InvoiceNumber
- Recipient details (name, phone, address)
- CodAmount
- DeliveryStatus
- TrackingNumber
- IsSent (success flag)
- ApiResponse (raw JSON for debugging)
- Timestamps

### ShippingByWeightByTotalRecord Table
Stores rate calculation rules:
- Store, Warehouse, Country, State, Zip filters
- ShippingMethodId
- Weight range (WeightFrom, WeightTo)
- Subtotal range (OrderSubtotalFrom, OrderSubtotalTo)
- Rate configuration (AdditionalFixedCost, RatePerWeightUnit, PercentageRateOfSubtotal)
- LowerWeightLimit
- TransitDays

## Localization

All UI strings use resource keys with pattern:
- `Plugins.Shipping.SteadFast.*`

Resource strings are automatically installed during plugin installation.

To customize:
1. Navigate to **Admin > Configuration > Languages**
2. Select your language
3. Click **String resources**
4. Filter by "Plugins.Shipping.SteadFast"
5. Edit as needed

## Troubleshooting

### Plugin Fails to Install
- Check .NET 8.0 is installed
- Ensure nopCommerce 4.70 compatibility
- Verify plugin files are in correct directory
- Check application logs for migration errors

### API Connection Fails
- Verify API credentials are correct
- Check "Use Sandbox" setting matches your environment
- Ensure firewall allows outbound HTTPS to steadfast.com.bd
- Test with "Test Connection" button
- Check API key hasn't expired

### Shipments Not Auto-Created
- Verify "Auto Create Shipment" is enabled
- Check order uses SteadFast shipping method
- Ensure order is not marked as "Pick Up In Store"
- Review application logs for event consumer errors
- Verify shipping address exists and is complete

### Shipping Rates Not Showing
- Check if "Calculate by Weight/Total" is properly configured
- Verify rate records exist for the order's weight/total
- Ensure shipping methods are active
- Check country/state restrictions
- Review "Limit to Configured Methods" setting

## Development

### Project Structure
```
Nop.Plugin.Shipping.SteadFast/
├── Controllers/                    # Admin controllers
│   └── SteadFastController.cs
├── Data/                          # (Reserved for future use)
├── Domain/                        # Domain entities
│   ├── ShippingByWeightByTotalRecord.cs
│   └── SteadFastShipmentRecord.cs
├── Factories/                     # (Reserved for future use)
├── Infrastructure/                # Plugin infrastructure
│   ├── Cache/
│   │   └── ShipmentEventConsumer.cs
│   ├── Migrations/
│   │   └── SchemaMigration.cs
│   ├── NopStartup.cs
│   ├── RouteProvider.cs
│   └── ViewLocationExpander.cs
├── Models/                        # View and API models
│   ├── Admin/
│   │   ├── ConfigurationModel.cs
│   │   └── ShipmentListModel.cs
│   └── Api/
│       ├── CreateOrderModels.cs
│       └── OtherApiModels.cs
├── Services/                      # Business logic services
│   ├── ISteadFastService.cs
│   ├── SteadFastService.cs
│   ├── IShippingByWeightByTotalService.cs
│   └── ShippingByWeightByTotalService.cs
├── Views/                         # Razor views
│   ├── Configure.cshtml
│   ├── ShipmentList.cshtml
│   └── _ViewImports.cshtml
├── logo.jpg                       # Plugin icon
├── plugin.json                    # Plugin metadata
├── SteadFastComputationMethod.cs  # Main plugin class
├── SteadFastDefaults.cs          # Constants
└── SteadFastSettings.cs          # Settings model
```

### Extending the Plugin

To add new features:

1. **Add New API Endpoint**
   - Define request/response models in `Models/Api/`
   - Add method to `ISteadFastService.cs`
   - Implement in `SteadFastService.cs`

2. **Add New Admin Feature**
   - Create action in `SteadFastController.cs`
   - Add corresponding view in `Views/`
   - Update routes in `RouteProvider.cs` if needed
   - Add resource strings in `InstallAsync()`

3. **Add New Domain Entity**
   - Create entity class in `Domain/`
   - Add migration in `Infrastructure/Migrations/`
   - Create repository service in `Services/`
   - Register in `NopStartup.cs`

4. **Add New Event Consumer**
   - Create consumer class in `Infrastructure/Cache/`
   - Implement `IConsumer<TEvent>`
   - Register automatically via dependency injection

## Support

For issues or questions:
- **Plugin Issues**: Create an issue in the repository
- **SteadFast API**: Contact SteadFast support at https://steadfast.com.bd/contact
- **nopCommerce**: Visit https://www.nopcommerce.com/boards

## License

This plugin follows nopCommerce's licensing model. Refer to the repository LICENSE file for details.

## Version History

### 4.70.0 (Initial Release)
- Full SteadFast API integration
- Automatic and manual shipment creation
- Shipping rate calculation (fixed and by weight/total)
- Admin configuration interface
- Shipment management grid
- Database migrations
- Localization support
- Event-driven architecture

## Credits

- Developed for nopCommerce 4.70
- Based on Nop.Plugin.Shipping.FixedByWeightByTotal architecture
- SteadFast API documentation: https://docs.google.com/document/d/e/2PACX-1vTi0sTyR353xu1AK0nR8E_WKe5onCkUXGEf8ch8uoJy9qxGfgGnboSIkNosjQ0OOdXkJhgGuAsWxnIh/pub
