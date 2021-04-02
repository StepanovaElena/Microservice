using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace MetricsManager
{
    public class Program
    {
        public static void Main(string[] args)
{
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }

            // Отлов всех исключений в рамках работы приложения.
            catch (Exception exception)
            {
                //NLog: устанавливаем отлов исключений.
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Остановка логера. 
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging(logging =>
            {
                // создание провайдеров логирования
                logging.ClearProviders();
                // устанавливаем минимальный уровень логирования
                logging.SetMinimumLevel(LogLevel.Trace); 
            })
            // добавляем библиотеку nlog
            .UseNLog(); 
    }
}
