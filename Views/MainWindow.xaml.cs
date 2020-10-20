using MinecraftTranslatorTool.Data;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinecraftTranslatorTool.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Title = "MinecraftTranslatorTool";
            ProjectsList.SelectedIndex = 0;

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
        /// Handles double click on a CultureInfo in the LanguageList ListView.
        /// Opens a new TranslateWindow for the clicked project.
        /// </summary>
        private void ProjectDoubleClick(object sender, MouseButtonEventArgs e) {
            TranslateProject project = ProjectsList.Items[ProjectsList.SelectedIndex] as TranslateProject;
            if (project == null) return;
            CultureInfo cultureInfo = LanguageList.Items[LanguageList.SelectedIndex] as CultureInfo;
            if (cultureInfo == null) return;
            TranslateWindow window = new TranslateWindow(project, cultureInfo.Name);
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Handles selection changes in the ProjectsList ListView.
        /// </summary>
        private void ProjectsListSelected(object sender, SelectionChangedEventArgs e) {
            TranslateProject project = ProjectsList.Items[ProjectsList.SelectedIndex] as TranslateProject;
            if (project == null) return;
            foreach (var item in project.GetLanguages()) {
                LanguageList.Items.Add(item);
            }
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

        /// <summary>
        /// Handles clicks on the "Add new Language" button.
        /// </summary>
        private void NewLanguageClick(object sender, RoutedEventArgs e) {
            if (NewLanguageBox.Text == string.Empty) return;
            try {
                CultureInfo cultureInfo = new CultureInfo(NewLanguageBox.Text);
                TranslateProject project = ProjectsList.Items[ProjectsList.SelectedIndex] as TranslateProject;
                if (project == null) return;
                project.AddLanguage(cultureInfo);
                LanguageList.Items.Add(cultureInfo);
            } catch {
                MessageBox.Show("The language you entered is not valid.");
            }
        }
    }
}
