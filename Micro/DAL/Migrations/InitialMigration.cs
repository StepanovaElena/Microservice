using FluentMigrator;

namespace MetricsManager.DAL.Migrations
{    
    [Migration(1)]
    public class InitialMigration : Migration
    {
        private readonly string[] _tablesNames = { "cpumetrics", "rammetrics", "networkmetrics", "dotnetmetrics", "hddmetrics" };
        public override void Up()
        {
            Create.Table("agents")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("AgentId").AsInt32()
                .WithColumn("AgentUrl").AsString();


            foreach (var tableName in _tablesNames)
            {
                Create.Table(tableName)
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }
        }

        public override void Down()
        {
            Delete.Table("agents");

            foreach (var tableName in _tablesNames)
            {
                Delete.Table(tableName);
            }            
        }
    }

}
