using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WSRSim3.Models;
using WSRSim3.Pages;
using static WSRSim3.Classes.Helper;

namespace WSRSim3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            TitleLb.Content = (e.Content as Page).Title;
            if (MainFrame.CanGoBack)
            {
                BackBtn.Visibility = Visibility.Visible;
            }
            else
            {
                BackBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionLb.Content = version.Major + "." + version.Minor + "." + version.Build.ToString() + version.Revision.ToString("D5");
            ProjectLv.ItemsSource = Db.Project.ToList();
            ProjectLv.SelectedIndex = 0;
            if (File.Exists("Memory.txt"))
            {
                string read = File.ReadAllText("Memory.txt");
                if (read == "1")
                {
                    MainFrame.Navigate(new Dashboard());
                }
                if (read == "2")
                {
                    MainFrame.Navigate(new TaskList());
                }
                if (read == "3")
                {
                    MainFrame.Navigate(new Gant());
                }
            }
            else
            {
                MainFrame.Navigate(new Dashboard());
            }

        }

        private void DashBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Dashboard());
        }

        private void TaskListBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TaskList());
        }

        private void GantBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Gant());
        }

        private void ProjectLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ProjectLv.SelectedItem as Project;
            SelectedProject = item;
            if (File.Exists("Memory.txt"))
            {
                string read = File.ReadAllText("Memory.txt");
                if(read == "1")
                {
                    MainFrame.Navigate(new Dashboard());
                }
                if (read == "2")
                {
                    MainFrame.Navigate(new TaskList());
                }
                if (read == "3")
                {
                    MainFrame.Navigate(new Gant());
                }
            }
            else
            {
                MainFrame.Navigate(new Dashboard());
            }
        }
    }
}
