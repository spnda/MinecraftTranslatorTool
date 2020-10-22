using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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

        public List<CultureInfo> GetLanguages() {
            var ret = new List<CultureInfo>();
            foreach (string file in Directory.GetFiles(ProjectFolder)) {
                try {
                    CultureInfo info = new CultureInfo(Path.GetFileNameWithoutExtension(file));
                    ret.Add(info);
                } catch {
                    continue;
                }
            }
            return ret;
        }

        /// <summary>
        /// Add a new language to the project. This will create a new JSON file in the 
        /// ProjectFolder from given CultureInfo, e.g. 'de_de.json'.
        /// </summary>
        /// <param name="cultureInfo"></param>
        public void AddLanguage(CultureInfo cultureInfo) {
            string filePath = Path.Combine(ProjectFolder, $"{cultureInfo.Name.ToLower().Replace('-', '_')}.json");
            File.Create(filePath).Close();
            File.WriteAllText(filePath, "{}"); // Make it a valid JSON
        }
    }
}
