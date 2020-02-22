namespace console_app_consumer.Repository
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using contracts;
    using Dapper;
    using Npgsql;

    public class MessageInsertRepository : IMessageInsertRepository
    {
        private readonly string _connectionString;

        // https://www.postgresql.org/docs/current/static/errcodes-appendix.html
        private const string UniqueViolationPgErrorCode = "23505";

        public MessageInsertRepository( string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> InsertAsync(InsertableMessage message, CancellationToken token)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(token);
            if (token.IsCancellationRequested)
            {
                return false;
            }

            var timestamp = message.Timestamp ?? (DateTime?) DateTime.UtcNow;

            try
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO Messages(
                        Message_Id,
                        Mass_Transit_Message_Id,
                        Conversation_Id,
                        Source_Address,
                        Destination_Address,
                        Message_Type,
                        Protocol,
                        Body,
                        Host_Machine,
                        Host_Assembly,
                        Host_Assembly_Version,
                        Host_Framework_Version,
                        Host_Mass_Transit_Version,
                        File_Path,
                        User_Id,
                        Timestamp,
                        Request_Uri,
                        Service_Family_Name,
                        Response_Code)
                    VALUES(
                        @MessageId,
                        @MassTransitMessageId,
                        @ConversationId,
                        @SourceAddress,
                        @DestinationAddress,
                        @MessageType,
                        @Protocol,
                        @Body::jsonb,
                        @HostMachine,
                        @HostAssembly,
                        @HostAssemblyVersion,
                        @HostFrameworkVersion,
                        @HostMassTransitVersion,
                        @FilePath,
                        @UserId,
                        @Timestamp,
                        @RequestUri,
                        @ServiceFamilyName,
                        @ResponseCode::smallint)",
                    new
                    {
                        message.MessageId,
                        message.MassTransitMessageId,
                        message.ConversationId,
                        message.SourceAddress,
                        message.DestinationAddress,
                        message.MessageType,
                        Protocol = "AMQP",
                        message.Body,
                        message.HostMachine,
                        message.HostAssembly,
                        message.HostAssemblyVersion,
                        message.HostFrameworkVersion,
                        message.HostMassTransitVersion,
                        message.FilePath,
                        message.UserId,
                        Timestamp = timestamp,
                        message.RequestUri,
                        message.ServiceFamilyName,
                        message.ResponseCode
                    });
            }
            catch (PostgresException e)
            {
                if (e.SqlState != UniqueViolationPgErrorCode)
                    throw;
            }

            return true;
        }
    }
}