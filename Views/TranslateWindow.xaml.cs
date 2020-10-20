using MinecraftTranslatorTool.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace MinecraftTranslatorTool.Views {
    /// <summary>
    /// Interaction logic for TranslateWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window {
        private new string Language = "de_de";

        private bool Changed = false;

        private Dictionary<string, string> defaultTranslations;
        private Dictionary<string, string> newTranslations;
        private List<TranslationItem> items = new List<TranslationItem>();

        public TranslateProject project;

        public TranslateWindow(TranslateProject project, string language) {
            InitializeComponent();
            this.Language = language;
            if (project != null) {
                this.project = project;
                Title = $"Project - {project.Name}";

                LoadTranslations();
                StringsList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Load all translations from the default and translated file into the ListView shown on screen.
        /// Also checks if translations are present or not.
        /// </summary>
        private void LoadTranslations() {
            string defaultFile = Path.Combine(this.project.ProjectFolder, $"{this.project.OriginalLanguage}.json");
            string translationFile = Path.Combine(this.project.ProjectFolder, $"{this.Language}.json");
            if (!File.Exists(defaultFile)) {
                MessageBox.Show("Default language file not present. Can't load project.");
            } else {
                defaultTranslations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(defaultFile));
                if (File.Exists(translationFile)) newTranslations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(translationFile));
                else newTranslations = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> translation in defaultTranslations) {
                    bool translated = newTranslations.ContainsKey(translation.Key);
                    items.Add(new TranslationItem() {
                        Id = translation.Key,
                        Translation = translation.Value,
                        Translated = translated,
                        TranslatedString = translated ? newTranslations[translation.Key] : string.Empty,
                    });
                }
                foreach (var item in items.OrderBy(i => i.Translated).ToList()) StringsList.Items.Add(item);
            }
        }

        #region ClickHandlers

        private void SaveAllClick(object sender, RoutedEventArgs e) {
            if (Changed) {
                SaveToFile();
                Changed = false;
            }
        }

        /// <summary>
        /// Discard all chankes
        /// </summary>
        private void DiscardClick(object sender, RoutedEventArgs e) {
            if (!Changed) return; // Nothing to discard
            MessageBoxResult result = MessageBox.Show("Are you sure you want to discard everything?", "Discard", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Changed = false;
                // We are going to discard all changes and reset the window.
                defaultTranslations.Clear();
                newTranslations.Clear();
                LoadTranslations();
            }
        }

        /// <summary>
        /// Save the current translation to the temporary Dictionary
        /// </summary>
        private void SaveClick(object sender, RoutedEventArgs e) {
            if (NewTranslation.Text == string.Empty) return;
            TranslationItem translationItem = StringsList.SelectedItem as TranslationItem;
            // Some strings contain %s or %d for the game to replace with a player name or something else.#
            // Warn the user that their translation is missing this placeholder string.
            if (translationItem.Translation.Contains("%s") && !NewTranslation.Text.Contains("%s")) {
                MessageBoxResult result = MessageBox.Show(this, "The original translation contains a \"%s\", but yours doesn't. Do you still want to continue?", "Are you sure?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }
            newTranslations[translationItem.Id] = NewTranslation.Text;
            int index = items.FindIndex(i => i.Id == translationItem.Id);
            items[index].TranslatedString = NewTranslation.Text;
            items[index].Translated = true;
            StringsList.Items.Clear();
            foreach (var item in items.OrderBy(i => i.Translated).ToList()) StringsList.Items.Add(item);
            Changed = true;
        }

        #endregion

        /// <summary>
        /// Saves the `newTranslations` to the minecraft language JSON file.
        /// </summary>
        private void SaveToFile() {
            string defaultFileLocation = Path.Combine(this.project.ProjectFolder, $"{this.project.OriginalLanguage}.json");
            string defaultFile = File.ReadAllText(defaultFileLocation);
            // Replace translations for new ones
            foreach (KeyValuePair<string, string> translation in defaultTranslations) {
                try {
                    string newTranslation = newTranslations[translation.Key];
                    Console.WriteLine($"\"{translation.Key}\": \"{translation.Value}\"");
                    defaultFile = defaultFile.Replace($"\"{translation.Key}\": \"{translation.Value}\"", $"\"{translation.Key}\": \"{newTranslation}\"");
                } catch (KeyNotFoundException) {
                    defaultFile = defaultFile.Replace($"\n  \"{translation.Key}\": \"{translation.Value}\",", "");
                    continue;
                }
            }
            // Remove empty multi line sections.
            // These are usually caused by sections of translations missing in the 
            // translated document.
            defaultFile = Regex.Replace(defaultFile, @"(^\s*$\n){2,}", string.Empty, RegexOptions.Multiline);

            string newFile = Path.Combine(this.project.ProjectFolder, $"{this.Language}.json");
            if (!File.Exists(newFile)) File.Create(newFile).Close();
            File.WriteAllText(newFile, defaultFile);
        }

        /// <summary>
        /// Executes when the window closes to save changes to the JSON file.
        /// </summary>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            // Skip any unneccesary saving if no changes have been made.
            if (Changed) {
                SaveToFile();
                Changed = false;
            }
        }
    }
}
