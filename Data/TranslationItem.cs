using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinecraftTranslatorTool.Data {
    public class TranslationItem {
        public string Id { get; set; }

        public string Translation { get; set; }

        public bool Translated { get; set; }
    
        public string TranslatedString { get; set; }

        public Brush Color { 
            get {
                return Translated ? Brushes.Green : Brushes.Red;
            }
        }
    }
}
