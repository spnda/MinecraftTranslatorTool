using Newtonsoft.Json;

namespace MinecraftTranslatorTool.Data {
    public class TranslateProject {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("originalLanguage")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonIgnore]
        public string ProjectFolder { get; set; }
    }
}
