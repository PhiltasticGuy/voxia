using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VoxIA.Mobile.Services.Api
{
    public class SpeechBrainService : ITranscriptionService
    {
        private readonly HttpClient _client;

        public SpeechBrainService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.0.11:5000");

            _client.DefaultRequestHeaders.Add(
                "Accept",
                "text/plain");
            _client.DefaultRequestHeaders.Add(
                "User-Agent",
                "VoxIA.Mobile");
        }

        public async Task<string> TranscribeRecording(string filename, byte[] content)
        {
            try
            {
                using var multipartContent = new MultipartFormDataContent()
                {
                    { new ByteArrayContent(content), "fileVoice", filename }
                };

                using var response = await _client.PostAsync("transcribe", multipartContent);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return string.Empty;
        }
    }
}
