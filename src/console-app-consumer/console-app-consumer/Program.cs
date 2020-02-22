namespace console_app_consumer
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rabbit;
    using Repository;

    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.AddSingleton<IMessageInsertRepository>(provider =>
                        new MessageInsertRepository(config.GetConnectionString("MessageDatabase")));
                    services.AddTransient<IMessageHandler, MessageHandler>();
                    services.AddHostedService<ConsumingService>();
                    services.Configure<RabbitOptions>(config.GetSection("rabbit"));
                });
    }
}