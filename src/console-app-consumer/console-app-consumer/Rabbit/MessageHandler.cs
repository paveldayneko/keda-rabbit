namespace console_app_consumer.Rabbit
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using contracts;
    using RabbitMQ.Client.Events;
    using Repository;

    public class MessageHandler : IMessageHandler
    {
        private readonly IMessageInsertRepository _insertRepository;

        public MessageHandler(IMessageInsertRepository insertRepository)
        {
            _insertRepository = insertRepository;
        }

        public async Task HandleMessageAsync(AsyncEventingBasicConsumer consumer, BasicDeliverEventArgs args)
        {
            var json = System.Text.Encoding.UTF8.GetString(args.Body);
            try
            {
                var messageMetaData = JsonSerializer.Deserialize<MessageMetaData>(json,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

                var message = new InsertableMessage
                {
                    ConversationId = messageMetaData.ConversationId,
                    MessageId = messageMetaData.MessageId,
                    SourceAddress = messageMetaData.SourceAddress,
                    DestinationAddress = messageMetaData.DestinationAddress,
                    FilePath = messageMetaData.FilePath,
                    HostAssembly = messageMetaData.Host.Assembly,
                    HostAssemblyVersion = messageMetaData.Host.AssemblyVersion,
                    HostFrameworkVersion = messageMetaData.Host.FrameworkVersion,
                    MessageType = messageMetaData.MessageType.FirstOrDefault(),
                    Timestamp = messageMetaData.Timestamp,
                    UserId = messageMetaData.UserId
                };

                var options = new JsonDocumentOptions
                {
                    AllowTrailingCommas = true
                };

                using (var document = JsonDocument.Parse(json, options))
                {
                    message.Body = document.RootElement.GetProperty("message").GetRawText();
                }

                var result = await _insertRepository.InsertAsync(message, CancellationToken.None);


                if (result)
                {
                    consumer.Model.BasicAck(args.DeliveryTag, false);
                    Console.WriteLine("Message acked positive");
                }
                else
                {
                    consumer.Model.BasicNack(args.DeliveryTag, false, !args.Redelivered);
                    Console.WriteLine("Message acked negative");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public interface IMessageHandler
    {
        Task HandleMessageAsync(AsyncEventingBasicConsumer consumer, BasicDeliverEventArgs args);
    }
}