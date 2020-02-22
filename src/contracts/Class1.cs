using System;
using System.Collections.Generic;
using System.Text;

namespace contracts
{
    public class MessageMetaData
    {

        public Guid MessageId { get; set; }
      
        public Guid? MassTransitMessageId { get; set; }

        public Guid? ConversationId { get; set; }

        public string SourceAddress { get; set; }

        public string DestinationAddress { get; set; }

        public List<string> MessageType { get; set; }

        public Host Host { get; set; }

        public string FilePath { get; set; }

        public string UserId { get; set; }

        public DateTime? Timestamp { get; set; }

        public string RequestUri { get; set; }

        public string ServiceFamilyName { get; set; }

        public short? ResponseCode { get; set; }
    }

    public class Host
    {
        public string Machine { get; set; }

        public string Assembly { get; set; }

        public string AssemblyVersion { get; set; }

        public string FrameworkVersion { get; set; }

        public string MassTransitVersion { get; set; }
    }
}
