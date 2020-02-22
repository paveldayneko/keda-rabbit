namespace queue_binder.Rabbit
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRabbitMqHttpApiGateway
    {
        Task<IEnumerable<ExchangeInformation>> GetExchangesAsync(CancellationToken cancellationToken);
    }
}