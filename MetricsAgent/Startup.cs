using AutoMapper;
using MetricsAgent.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentMigrator.Runner;
using Quartz.Spi;
using MetricsAgent.Jobs;
using Quartz;
using Quartz.Impl;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;

namespace MetricsAgent
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

            services.AddScoped<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddScoped<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddScoped<IHddMetricsRepository, HddMetricsRepository>();
            services.AddScoped<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddScoped<IRamMetricsRepository, RamMetricsRepository>();

            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // ��������� ��������� SQLite 
                    .AddSQLite()
                    // ������������� ������ �����������
                    .WithGlobalConnectionString(_connectionString)
                    // ������������ ��� ������ ������ � ����������
                    .ScanIn(typeof(Startup).Assembly).For.Migrations()
                ).AddLogging(lb => lb
                    .AddFluentMigratorConsole());

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
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API ������� ������ ����� ������",
                    Description = "��� ����� �������� � api ������ �������",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "...",
                        Email = string.Empty,
                        Url = new Uri("https://example.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "����� ������� ��� ����� ��������� ��� ������������",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                // ��������� ���� �� �������� ����� ����������� ��� Swagger UI
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });       
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

            // ��������� ��������
            migrationRunner.MigrateUp();

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ������� ������ ����� ������");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
