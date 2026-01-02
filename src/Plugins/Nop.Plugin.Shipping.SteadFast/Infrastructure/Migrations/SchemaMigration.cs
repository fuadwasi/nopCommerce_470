using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Infrastructure.Migrations;

[NopMigration("2024/12/26 08:00:00:0000000", "Shipping.SteadFast base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<SteadFastShippingByWeightByTotalRecord>();
        Create.TableFor<SteadFastShipmentRecord>();
        Create.TableFor<SteadFastShipmentEventLog>();
    }
}
