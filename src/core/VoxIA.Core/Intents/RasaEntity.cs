namespace VoxIA.Core.Intents
{
    public class RasaEntity
    {
        public string entity { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public double confidence_entity { get; set; }
        public string value { get; set; }
        public string extractor { get; set; }
    }
}
