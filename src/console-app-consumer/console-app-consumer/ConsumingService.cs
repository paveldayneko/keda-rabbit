namespace console_app_consumer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Rabbit;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class ConsumingService:IHostedService
    {
        private readonly RabbitOptions _options;
        private  IModel _channel;
        private  IConnection _connection;
        private IMessageHandler _messageHandler;
        public ConsumingService(IOptions<RabbitOptions> options, IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                UserName = _options.Username, Password = _options.Password, HostName = _options.Host, DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(0,100,true);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += (sender, args) => _messageHandler.HandleMessageAsync((AsyncEventingBasicConsumer) sender, args);
            var consumerTag = _channel.BasicConsume("Keda.Contracts_All", false, consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Dispose();
            _connection?.Dispose();
            return Task.CompletedTask;
        }
    }
}
