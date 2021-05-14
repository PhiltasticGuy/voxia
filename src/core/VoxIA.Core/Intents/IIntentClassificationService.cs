using System.Threading.Tasks;

namespace VoxIA.Core.Intents
{
    public interface IIntentClassificationService
    {
        Task<RasaRoot> ParseIntent(string text);
        void SetServerUrl(string nluIpAddress, int nluPort);
    }
}
