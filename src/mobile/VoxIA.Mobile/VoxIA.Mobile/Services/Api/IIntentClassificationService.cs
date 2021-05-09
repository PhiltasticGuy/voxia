using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.Mobile.Services.Api
{
    public interface IIntentClassificationService
    {
        Task<RasaRoot> ParseIntent(string text);
    }
}
