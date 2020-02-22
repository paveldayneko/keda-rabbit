namespace contracts
{
    using System;

    public class InsertableMessage
    {
        public Guid MessageId { get; set; }

        public Guid? MassTransitMessageId { get; set; }

        public Guid? ConversationId { get; set; }

        public string SourceAddress { get; set; }

        public string DestinationAddress { get; set; }

        public string MessageType { get; set; }

        public string Body { get; set; }

        public string HostMachine { get; set; }

        public string HostAssembly { get; set; }

        public string HostAssemblyVersion { get; set; }

        public string HostFrameworkVersion { get; set; }

        public string HostMassTransitVersion { get; set; }

        public string FilePath { get; set; }

        public string UserId { get; set; }

        public DateTime? Timestamp { get; set; }

        public string RequestUri { get; set; }

        public string ServiceFamilyName { get; set; }

        public short? ResponseCode { get; set; }
    }
}
