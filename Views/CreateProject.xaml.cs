using Microsoft.Win32;
using MinecraftTranslatorTool.Data;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace MinecraftTranslatorTool.Views {
    /// <summary>
    /// Interaction logic for CreateProject.xaml
    /// </summary>
    public partial class CreateProject : Window {
        string originalJsonFile = string.Empty;

        private TranslateProject project = new TranslateProject();

        public CreateProject() {
            InitializeComponent();
            Title = "Create new project";
        }

        private void LoadFileClick(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select your original language JSON file.";
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON Files (*.json)|*.json";

            Nullable<bool> result = dialog.ShowDialog();

            if (result.HasValue && result.Value) {
                if (Path.GetExtension(dialog.FileName) != ".json") return;
                originalJsonFile = dialog.FileName;
                project.OriginalLanguage = DefaultLanguageTextBox.Text = Path.GetFileNameWithoutExtension(originalJsonFile);
            }
        }

        private void CreateProjectClick(object sender, RoutedEventArgs e) {
            if (NameTextBox.Text == string.Empty || DefaultLanguageTextBox.Text == string.Empty || originalJsonFile == string.Empty) return;
            string projectBaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "projects", NameTextBox.Text);
            string projectJsonFile = Path.Combine(projectBaseFolder, "project.json");
            if (File.Exists(projectJsonFile)) {
                MessageBox.Show($"Project with following name already exists. {NameTextBox.Text}");
            } else {
                Directory.CreateDirectory(Path.GetDirectoryName(projectJsonFile));
                project.Id = NameTextBox.Text.ToLower().Replace(" ", "-");
                project.Name = NameTextBox.Text;
                project.Version = VersionTextBox.Text == string.Empty ? "1.0.0" : VersionTextBox.Text;
                project.OriginalLanguage = DefaultLanguageTextBox.Text;
                File.Create(projectJsonFile).Close();
                File.WriteAllText(projectJsonFile, JsonConvert.SerializeObject(project));
                File.Copy(originalJsonFile, Path.Combine(projectBaseFolder, Path.GetFileName(originalJsonFile)));
                this.Close();
            }
        }
    }
}
