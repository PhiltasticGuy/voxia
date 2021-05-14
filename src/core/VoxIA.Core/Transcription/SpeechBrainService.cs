using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VoxIA.Core.Transcription
{
    public class SpeechBrainService : ITranscriptionService
    {
        private HttpClient _client;

        public SpeechBrainService()
        {
            SetServerUrl("http://192.168.0.11:5000", 5000);
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
