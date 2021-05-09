using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoxIA.Mobile.Services.Api
{
    public partial class RasaService : IIntentClassificationService
    {
        private readonly HttpClient _client;

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
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.0.11:5005");

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
