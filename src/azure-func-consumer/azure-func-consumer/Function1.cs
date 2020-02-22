namespace azure_func_consumer
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([RabbitMQTrigger("Keda.Contracts_All_AF", ConnectionStringSetting = "RabbitMQConnection")] string inputMessage,

            ILogger logger)
        {
            var msq = inputMessage;
            await Task.CompletedTask;

        }
    }
}
