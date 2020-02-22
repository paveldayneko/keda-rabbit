namespace queue_binder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentScheduler;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Rabbit;
    using RabbitMQ.Client;

    public class QueueBinder : IHostedService
    {
        private readonly IBindingJob _bindingJob;

        public QueueBinder(IBindingJob bindingJob)
        {
            _bindingJob = bindingJob;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var registry = new Registry();
            registry.Schedule(() => _bindingJob.Execute()).ToRunNow().AndEvery(10).Seconds();

            JobManager.Initialize(registry);


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            JobManager.Stop();
            return Task.CompletedTask;
        }
    }

    public class BindingJob : IBindingJob
    {
        private readonly RabbitOptions _options;
        private readonly IRabbitMqHttpApiGateway _apiGateway;
        private const string Queue = "Keda.Contracts_All";

        public BindingJob(IOptions<RabbitOptions> options, IRabbitMqHttpApiGateway apiGateway)
        {
            _apiGateway = apiGateway;
            _options = options.Value;
        }

        public void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync()
        {
            var exchanges = await _apiGateway.GetExchangesAsync(CancellationToken.None);
            var validExchanges = exchanges.Where(x =>
                !string.IsNullOrWhiteSpace(x.Name) &&
                x.Name.StartsWith("Keda.Contracts", StringComparison.InvariantCultureIgnoreCase));

            var factory = new ConnectionFactory
            {
                UserName = _options.Username, Password = _options.Password, HostName = _options.Host
            };
            using var connection = factory.CreateConnection();

            using var model = connection.CreateModel();
            var args = new Dictionary<string, object> {{"x-queue-mode", "lazy"}};

            model.QueueDeclare(Queue, true, false, false, args);

            foreach (var exchange in validExchanges)
                model.QueueBind(Queue, exchange.Name, string.Empty, args);
        }
    }

    public interface IBindingJob
    {
        void Execute();
    }
}