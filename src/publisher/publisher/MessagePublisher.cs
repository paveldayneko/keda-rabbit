namespace publisher
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Keda.Contracts;
    using MassTransit;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    public class MessagePublisher : IHostedService
    {
        private readonly RabbitOptions _options;
        private IBusControl _bus;
        private readonly IFixture _fixture;

        public MessagePublisher(IOptions<RabbitOptions> options)
        {
            _options = options.Value;
            _fixture = new Fixture();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bus = await GetBusAsync();

            while (true)
            {
                var messages = _fixture.CreateMany<Person>(_options.MessagesPerSecond / 2).ToList<object>();

                messages.AddRange(_fixture.CreateMany<CompositeItem>(_options.MessagesPerSecond / 2));

                var tasks = messages.Select(m => _bus.Publish(m, cancellationToken));

                await Task.WhenAll(tasks);

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus?.StopAsync(cancellationToken);
        }


        public async Task<IBusControl> GetBusAsync()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host($"rabbitmq://{_options.Host}", h =>
                {
                    h.Username(_options.Username);
                    h.Password(_options.Password);
                });
            });

            await bus.StartAsync();
            return bus;
        }
    }
}