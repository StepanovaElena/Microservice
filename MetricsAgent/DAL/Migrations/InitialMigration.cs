using FluentMigrator;

namespace MetricsAgent.DAL.Migrations
{    
    [Migration(1)]
    public class InitialMigration : Migration
    {
        private readonly string[] _tablesNames = { "cpumetrics", "rammetrics", "networkmetrics", "dotnetmetrics", "hddmetrics" };
        public override void Up()
        {
            foreach (var tableName in _tablesNames)
            {
                Create.Table(tableName)
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }
        }

        public override void Down()
        {
            foreach (var tableName in _tablesNames)
            {
                Delete.Table(tableName);
            }            
        }
    }

}
