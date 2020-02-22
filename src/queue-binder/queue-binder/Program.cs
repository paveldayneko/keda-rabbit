using System;

namespace queue_binder
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rabbit;

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
                    services.AddHttpClient();
                    services.AddTransient<IRabbitMqHttpApiGateway, RabbitMqHttpApiGateway>();
                    services.AddTransient<IBindingJob, BindingJob>();
                    services.AddHostedService<QueueBinder>();
                    services.Configure<RabbitOptions>(hostContext.Configuration.GetSection("rabbit"));
                });
    }
}
