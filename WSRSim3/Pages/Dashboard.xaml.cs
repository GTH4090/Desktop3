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
using System.Windows.Threading;
using static WSRSim3.Classes.Helper;
using WSRSim3.Models;
using System.Reflection;

namespace WSRSim3.Pages
{
    /// <summary>
    /// Логика взаимодействия для Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        DispatcherTimer Timer = new DispatcherTimer();

        public Dashboard()
        {
            InitializeComponent();
            File.WriteAllText("Memory.txt", "1");
            Timer.Interval = new TimeSpan(0, 0, 30);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            loadData();
        }

        private void loadData()
        {

            try
            {
                List<Models.Task> openTasks = Db.Task.Where(el => el.ProjectId == SelectedProject.Id && (el.StatusId == 1 || el.StatusId == 2)).ToList();
                OpenedLb.Content = openTasks.Count();
                OpentaskDataGrid.ItemsSource = openTasks;

                List<Models.Task> deadlinedTasks = Db.Task.Where(el => el.ProjectId == SelectedProject.Id && el.Deadline < DateTime.Now).ToList();
                DeadlinedLb.Content = deadlinedTasks.Count();
                DeadlinedtaskDataGrid.ItemsSource = deadlinedTasks;
                if(deadlinedTasks.Count > 2)
                {
                    DeadlinedSp.Background = Brushes.Red;
                }

                List<Models.Task> activeTasks = Db.Task.Where(el => el.ProjectId == SelectedProject.Id && el.StatusId == 2 && (el.StartActualTime <= DateTime.Now || el.CreatedTime <= DateTime.Now) &&
                (el.FinishActualTime >= DateTime.Now || el.Deadline >= DateTime.Now)).ToList();
                ActiveLb.Content = activeTasks.Count();
                ActivetaskDataGrid.ItemsSource = activeTasks;
                if(activeTasks.Count == 0 && DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 18)
                {
                    ActiveSp.Background = Brushes.Red;
                }


                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                int weekDay = (int)DateTime.Now.DayOfWeek;
                if (weekDay != 0)
                {
                    
                    endDate = DateTime.Now.AddDays(7 - weekDay).Date;
                }
                else
                {
                    endDate = DateTime.Now.Date;
                }
                startDate = endDate.AddDays(-6);

                List<Models.Task> openedPerWeekTasks = Db.Task.Where(el => el.ProjectId == SelectedProject.Id && (el.StatusId == 1 || el.StatusId == 2 )
                && (((el.StartActualTime < startDate || el.CreatedTime < startDate) &&
                (el.FinishActualTime >= startDate || el.Deadline >= startDate)) || ((el.StartActualTime >= startDate || el.CreatedTime >= startDate)&&
                (el.StartActualTime <= endDate || el.CreatedTime <= endDate) ))).ToList();
                OpenedPerWeektaskDataGrid.ItemsSource = openedPerWeekTasks;
                OpenedPerWeekLb.Content = openedPerWeekTasks.Count();



                List<Employee> bestEmployees = Db.Employee.ToList().OrderByDescending(el => el.ClosedCount).Take(5).ToList();
                BestemployeeDataGrid.ItemsSource = bestEmployees;

                List<Employee> worstEmployees = Db.Employee.ToList().OrderByDescending(el => el.DeadlinedCount).Take(5).ToList();
                WorstemployeeDataGrid.ItemsSource = worstEmployees;


            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadData();
        }
    }
}
