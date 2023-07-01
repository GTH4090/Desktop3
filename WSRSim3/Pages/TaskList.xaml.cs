using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using static WSRSim3.Classes.Helper;
using WSRSim3.Models;
using Microsoft.Win32;

namespace WSRSim3.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskList.xaml
    /// </summary>
    public partial class TaskList : Page
    {
        bool IsAdd = false;
        public TaskList()
        {
            InitializeComponent();
            File.WriteAllText("Memory.txt", "2");

        }

        private void loadCbx()
        {

            try
            {
                executiveEmployeeIdCbx.ItemsSource = Db.Employee.ToList();
                statusIdCbx.ItemsSource = Db.TaskStatus.ToList();
                
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void loadData()
        {

            try
            {
                List<Models.Task> tasks = Db.Task.ToList().Where(el => el.ProjectId == SelectedProject.Id && el.StatusId != 3 && el.StatusId != 4).OrderBy(el => el.SortNum).ToList();
                if(SearchTbx.Text != "")
                {
                    tasks = tasks.Where(el => el.FullTitle.Contains(SearchTbx.Text) || el.Description.Contains(SearchTbx.Text)).ToList();
                }
                taskDataGrid.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadData();
            loadCbx();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (IsAdd)
                {
                    Db.Task.Add(grid1.DataContext as Models.Task);
                    SpectatorSp.Visibility = Visibility.Visible;
                    AddAttachmentBtn.Visibility = Visibility.Visible;
                    var item = (grid1.DataContext as Models.Task);
                    SpectatorCbx.ItemsSource = Db.Employee.Where(el => el.TaskSpectator.FirstOrDefault(l => l.TaskId == item.Id) == null).ToList();
                    SpectatorCbx.SelectedIndex = 0;
                }
                Db.SaveChanges();
                IsAdd = false;
                loadData();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Grid.SetColumnSpan(taskDataGrid, 2);
                TaskSv.Visibility = Visibility.Collapsed;
                grid1.DataContext = null;
                IsAdd = false;
                SpectatorSp.Visibility = Visibility.Visible;
                AddAttachmentBtn.Visibility = Visibility.Visible;
                taskDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            
        }
        private void loadTask()
        {

            try
            {
                if (taskDataGrid.SelectedItem != null)
                {
                    var item = taskDataGrid.SelectedItem as Models.Task;
                    Grid.SetColumnSpan(taskDataGrid, 1);
                    TaskSv.Visibility = Visibility.Visible;
                    grid1.DataContext = item;
                    SpectatorSp.Visibility = Visibility.Visible;
                    AddAttachmentBtn.Visibility = Visibility.Visible;

                    SpectatorCbx.ItemsSource = Db.Employee.Where(el => el.TaskSpectator.FirstOrDefault(l => l.TaskId == item.Id) == null).ToList();
                    SpectatorCbx.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            
        }

        private void taskDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsAdd = false;
            loadTask();
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                if(saveFile.ShowDialog() == true)
                {
                    var item = ((sender as Button).DataContext as Models.TaskAttachment).Attachment;
                    File.WriteAllBytes(saveFile.FileName, item);
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void AddAttachmentBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog() == true)
            {
                TaskAttachment attachment = new TaskAttachment();
                attachment.TaskId = (grid1.DataContext as Models.Task).Id;
                attachment.Attachment = File.ReadAllBytes(openFile.FileName);
                
                Db.TaskAttachment.Add(attachment);
                loadTask();
            }
        }

        private void AddSpectatorBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SpectatorCbx.SelectedItem != null)
            {
                var item = SpectatorCbx.SelectedItem as Employee;
                TaskSpectator spectator = new TaskSpectator();
                spectator.TaskId = (grid1.DataContext as Models.Task).Id;
                spectator.EmployeeId = item.Id;
                Db.TaskSpectator.Add(spectator);
                Db.SaveChanges();
                loadTask();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Grid.SetColumnSpan(taskDataGrid, 1);
                TaskSv.Visibility = Visibility.Visible;
                grid1.DataContext = new Models.Task();
                IsAdd = true;
                SpectatorSp.Visibility = Visibility.Collapsed;
                AddAttachmentBtn.Visibility = Visibility.Collapsed;
                executiveEmployeeIdCbx.SelectedIndex = 0;
                statusIdCbx.SelectedIndex = 0;
                deadlineDatePicker.SelectedDate = DateTime.Now.Date;
                (grid1.DataContext as Models.Task).ShortTitle = SelectedProject.ShortTitle + (SelectedProject.Task.Count() + 1).ToString();
                (grid1.DataContext as Models.Task).ProjectId = SelectedProject.Id;
                
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void SearchTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            loadData();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if(MessageBox.Show("Вы уверены?", "Точно?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var item = (sender as Button).DataContext as Models.Task;
                    if(item.Task1.Count() != 0)
                    {
                        if (MessageBox.Show("Будут удалены все ссылки на удаляемую задачу?", "Точно?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    
                        
                        item.StatusId = 4;
                        foreach(var i in item.Task1)
                        {
                            i.PreviousTaskId = null;
                        }
                        Db.SaveChanges();
                        loadData();
                    
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }
    }
}
