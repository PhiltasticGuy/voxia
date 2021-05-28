using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoxIA.Core.Intents
{
    public partial class RasaService : IIntentClassificationService
    {
        private HttpClient _client;

        // Puisque les classes et les propriétés en .NET Core suivent le PascalCase,
        // nous devons s'assurer que la sérialisation utilise bien le camelCase puisque
        // c'est le format attendu par l'API REST.
        private readonly JsonSerializerOptions _jsonSerializerOptions =
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        public RasaService()
        {
            SetServerUrl("127.0.0.1", 5005);
        }

        public RasaService(string ipAddress, int port)
        {
            SetServerUrl(ipAddress, port);
        }

        public void SetServerUrl(string ipAddress, int port)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                //TODO: Error! Log something...
                return;
            }

            if (port < 0 || port > 65535)
            {
                //TODO: Error! Log something...
                return;
            }

            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://{ipAddress}:{port}");

            _client.DefaultRequestHeaders.Add(
                "Accept",
                "text/plain");
            _client.DefaultRequestHeaders.Add(
                "User-Agent",
                "VoxIA.Mobile");
        }

        public async Task<RasaRoot> ParseIntent(string text)
        {
            try
            {
                // Sérialiser les valeurs afin les joindre au contenu de la requête.
                var content = new StringContent(
                    JsonSerializer.Serialize(new { text = text }, _jsonSerializerOptions),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _client.PostAsync("model/parse", content);

                response.EnsureSuccessStatusCode();

                // Désérialiser le contenu de la réponse.
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<RasaRoot>(responseStream, _jsonSerializerOptions);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return await Task.FromResult<RasaRoot>(null);
        }
    }
}
