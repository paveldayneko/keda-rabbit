namespace queue_binder.Rabbit
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public sealed class RabbitMqHttpApiGateway: IRabbitMqHttpApiGateway
    {
        private readonly HttpClient _httpClient;

        public RabbitMqHttpApiGateway(IHttpClientFactory clientFactory, IOptions<RabbitOptions> options)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri($"http://{options.Value.Host}:15672");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{options.Value.Username}:{options.Value.Password}")));
        }

        public async Task<IEnumerable<ExchangeInformation>> GetExchangesAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/api/exchanges", cancellationToken);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<ExchangeInformation>>(stream, options);
        }
    }
}