namespace console_app_consumer.Repository
{
    using System.Threading;
    using System.Threading.Tasks;
    using contracts;

    // ReSharper disable once ClassNeverInstantiated.Global

    public interface IMessageInsertRepository
    {
        Task<bool> InsertAsync(InsertableMessage message, CancellationToken token);
    }

}