using MinecraftTranslatorTool.Data;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MinecraftTranslatorTool.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Title = "MinecraftTranslatorTool";

            LoadProjects();
        }

        /// <summary>
        /// Loads all different projects from the projects folder.
        /// </summary>
        private void LoadProjects() {
            ProjectsList.Items.Clear(); // Reset data if any profiles were already read
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "projects"));
            foreach (string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "projects"), "*.*", SearchOption.AllDirectories)) {
                if (!File.Exists(file) || !file.EndsWith("project.json")) continue;
                TranslateProject project = JsonConvert.DeserializeObject<TranslateProject>(File.ReadAllText(file));
                if (project == null) continue;
                project.ProjectFolder = file.Replace("project.json", "");
                ProjectsList.Items.Add(project);
            }
        }

        /// <summary>
        /// Handles double click on a TranslateProject in the ProjectsList ListView.
        /// Opens a new TranslateWindow for the clicked project.
        /// </summary>
        private void ProjectDoubleClick(object sender, MouseButtonEventArgs e) {
            TranslateProject project = ProjectsList.Items[ProjectsList.SelectedIndex] as TranslateProject;
            if (project == null) return;
            TranslateWindow window = new TranslateWindow(project, "de_de");
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Handles clicks on the "Create new project" button.
        /// Opens the CreateProject window 
        /// </summary>
        private void CreateNewClick(object sender, RoutedEventArgs e) {
            CreateProject createNewWindow = new CreateProject();
            createNewWindow.ShowDialog();
            LoadProjects();
        }
    }
}
