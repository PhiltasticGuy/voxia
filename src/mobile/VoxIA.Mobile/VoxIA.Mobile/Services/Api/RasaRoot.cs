using System.Collections.Generic;

namespace VoxIA.Mobile.Services.Api
{

    public class RasaRoot
    {
        public List<RasaEntity> entities { get; set; }
        public RasaIntent intent { get; set; }
        public List<RasaIntentRanking> intent_ranking { get; set; }
        public string text { get; set; }
    }
}
