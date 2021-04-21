using AutoMapper;
using FluentMigrator.Runner;
using MetricsManager.Client;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Repositories;
using MetricsManager.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace MetricsManager
{
    public class Startup
    {
		private const string _connectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";
		private const string _cronExpression = "0/5 * * * * ?";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{			
			services.AddControllers();
			
			services.AddScoped<IAgentsRepository, AgentsRepository>();
			services.AddScoped<ICpuMetricsRepository, CpuMetricsRepository>();
			services.AddScoped<IDotNetMetricsRepository, DotNetMetricsRepository>();
			services.AddScoped<IHddMetricsRepository, HddMetricsRepository>();
			services.AddScoped<INetworkMetricsRepository, NetworkMetricsRepository>();
			services.AddScoped<IRamMetricsRepository, RamMetricsRepository>();
						
			var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
			var mapper = mapperConfiguration.CreateMapper();
			services.AddSingleton(mapper);

			services.AddHttpClient<IMetricsManagerClient, MetricsManagerClient>();
			
			services.AddFluentMigratorCore()
				.ConfigureRunner(rb => rb.AddSQLite() 
					.WithGlobalConnectionString(_connectionString)
					.ScanIn(typeof(Startup).Assembly).For.Migrations())
				.AddLogging(lb => lb.AddFluentMigratorConsole());

			services.AddSingleton<IJobFactory, SingletonJobFactory>();
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

			
			services.AddSingleton<CpuMetricsJob>();
			services.AddSingleton<DotNetMetricsJob>();
			services.AddSingleton<HddMetricsJob>();
			services.AddSingleton<NetworkMetricsJob>();
			services.AddSingleton<RamMetricsJob>();
			
			services.AddSingleton(new JobSchedule(
				jobType: typeof(CpuMetricsJob),
				cronExpression: _cronExpression));
			services.AddSingleton(new JobSchedule(
				jobType: typeof(DotNetMetricsJob),
				cronExpression: _cronExpression));
			services.AddSingleton(new JobSchedule(
				jobType: typeof(HddMetricsJob),
				cronExpression: _cronExpression));
			services.AddSingleton(new JobSchedule(
				jobType: typeof(NetworkMetricsJob),
				cronExpression: _cronExpression));
			services.AddSingleton(new JobSchedule(
				jobType: typeof(RamMetricsJob),
				cronExpression: _cronExpression));
			
			services.AddHostedService<QuartzHostedService>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

			// запускаем миграции
			migrationRunner.MigrateUp();

			app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
		}
    }
}
