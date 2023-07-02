using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static WSRSim3.Classes.Helper;
using WSRSim3.Models;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media.Converters;
using Path = System.Windows.Shapes.Path;

namespace WSRSim3.UpWindows
{
    /// <summary>
    /// Логика взаимодействия для GantWin.xaml
    /// </summary>
    public partial class GantWin : Window
    {
        DateTime DateStart = DateTime.Now.Date;
        DateTime DateEnd = DateTime.Now.Date;
        bool firstLoad = true;
        public GantWin()
        {
            InitializeComponent();
            
        }
         /// <summary>
         /// обрабоотчик события нажатия кнопки закрытия окна
         /// </summary>
         /// <param name="sender">отпраавитель</param>
         /// <param name="e">аргумент</param>
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Функция загрузки диаграммы ганта
        /// </summary>
        private void loadGant()
        {

            try
            {
                if (GantGrid != null)
                {
                    GantGrid.ColumnDefinitions.Clear();
                    GantGrid.RowDefinitions.Clear();
                    GantGrid.Children.Clear();
                }
                else
                {
                    return;
                }
                for (int i = 0; i <= Db.Task.Where(el => el.ProjectId == SelectedProject.Id).Count(); i++)
                {
                    GantGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                }
                for (int i = 0; i <= (DateEnd.Date - DateStart.Date).Days; i++)
                {
                    GantGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(ScaleSl.Value) });
                    Label dateLb = new Label();
                    dateLb.Width = double.NaN;
                    dateLb.Height = double.NaN;
                    dateLb.Content = DateStart.AddDays(i).ToString("dd MMM ddd");
                    Grid.SetColumn(dateLb, i);
                    Grid.SetRow(dateLb, GantGrid.RowDefinitions.Count() - 1);
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2);
                    Grid.SetColumn(border, i);
                    Grid.SetRowSpan(border, GantGrid.RowDefinitions.Count() - 1);
                    DateTime thisDate = DateStart.AddDays(i);
                    if (thisDate.Date == DateTime.Now.Date)
                    {
                        border.Background = Brushes.Blue;
                    }
                    if (thisDate.DayOfWeek == DayOfWeek.Saturday || thisDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        border.Background = Brushes.Red;
                    }
                    GantGrid.Children.Add(dateLb);
                    GantGrid.Children.Add(border);
                }
                for (int i = 0; i < Db.Task.Where(el => el.ProjectId == SelectedProject.Id).Count(); i++)
                {
                    Label label = new Label();
                    label.VerticalAlignment = VerticalAlignment.Stretch;
                    label.HorizontalAlignment = HorizontalAlignment.Stretch;
                    label.Width = double.NaN;
                    label.Height = double.NaN;
                    label.Background = Brushes.Wheat;
                    var item = Db.Task.Where(el => el.ProjectId == SelectedProject.Id).ToList()[i];
                    DateTime thisStart = new DateTime();
                    DateTime thisEnd = new DateTime();
                    if (item.StartActualTime != null)
                    {
                        thisStart = (DateTime)item.StartActualTime;
                    }
                    else
                    {
                        thisStart = (DateTime)item.CreatedTime;
                    }
                    if (item.FinishActualTime != null)
                    {
                        thisEnd = (DateTime)item.FinishActualTime;
                    }
                    else
                    {
                        thisEnd = (DateTime)item.Deadline;
                    }
                    try
                    {
                        Grid.SetRow(label, i);
                        if (thisStart < DateStart && thisEnd >= DateStart)
                        {
                            label.Content = $"(<-- не помещается) {item.FullTitle}";
                            Grid.SetColumn(label, 0);
                            Grid.SetColumnSpan(label, (thisEnd.Date - DateStart.Date).Days + 1);
                            if (thisEnd > DateEnd)
                            {
                                label.Content = $"(<-- не помещается) {item.FullTitle} (не помещается -->)";
                            }
                        }
                        else if (thisStart >= DateStart && thisStart <= DateEnd)
                        {
                            if (thisEnd <= DateEnd)
                            {
                                label.Content = item.FullTitle;
                                Grid.SetColumn(label, (thisStart.Date - DateStart.Date).Days);
                                Grid.SetColumnSpan(label, (thisEnd.Date - thisStart.Date).Days + 1);
                            }
                            else
                            {
                                label.Content = $"{item.FullTitle} (не помещается -->)";
                                Grid.SetColumn(label, (thisStart.Date - DateStart.Date).Days);
                                Grid.SetColumnSpan(label, (DateEnd.Date - thisStart.Date).Days + 1);
                            }
                        }
                        else
                        {
                            continue;
                        }
                        label.Tag = item;
                        label.MouseDown += Label_MouseDown;
                        label.MouseMove += Label_MouseMove;
                        GantGrid.Children.Add(label);
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }

                List<Path> pathes= new List<Path>();
                foreach (var item in GantGrid.Children)
                {
                    if (item is Label && (item as Label).Tag != null)
                    {
                        var task = (item as Label).Tag as Models.Task;
                        if (task.PreviousTaskId != null)
                        {
                            foreach (var item2 in GantGrid.Children)
                            {
                                if (item2 is Label && (item2 as Label).Tag != null)
                                {
                                    var task2 = (item2 as Label).Tag as Models.Task;
                                    if (task2.Id == task.PreviousTaskId)
                                    {
                                        double startX = ScaleSl.Value * (Grid.GetColumn(item2 as Label) ) + ScaleSl.Value / 2;
                                        double startY = GantGrid.ActualHeight / GantGrid.RowDefinitions.Count() * (Grid.GetRow(item2 as Label)) + GantGrid.ActualHeight / GantGrid.RowDefinitions.Count() / 2;
                                        Point startPoint = new Point(startX, startY);
                                        double endX = ScaleSl.Value * (Grid.GetColumn(item as Label) ) + ScaleSl.Value / 2;
                                        double endY = GantGrid.ActualHeight / GantGrid.RowDefinitions.Count() * (Grid.GetRow(item as Label)) + GantGrid.ActualHeight / GantGrid.RowDefinitions.Count() / 2;
                                        Point endPoint = new Point(endX, endY);

                                        
                                        LineGeometry line1 = new LineGeometry(startPoint, endPoint);
                                        LineGeometry line2 = new LineGeometry(endPoint, new Point(endPoint.X + 20, endPoint.Y + 20));
                                        LineGeometry line3 = new LineGeometry(endPoint, new Point(endPoint.X + 20, endPoint.Y - 20));

                                        GeometryGroup combined = new GeometryGroup();
                                        combined.Children.Add(line1);
                                        combined.Children.Add(line2);
                                        combined.Children.Add(line3);  
                                        Path path = new Path();
                                        path.Stroke = Brushes.Black;
                                        path.StrokeThickness = 4;

                                        

                                        path.Data = combined;
                                        Grid.SetColumnSpan(path, GantGrid.ColumnDefinitions.Count());
                                        Grid.SetRowSpan(path, GantGrid.RowDefinitions.Count());

                                        pathes.Add(path);
                                    }
                                }
                            }
                        }

                    }
                }
                foreach(Path item in pathes)
                {
                    GantGrid.Children.Add(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            (sender as Label).Opacity = 1;
            foreach (var item in GantGrid.Children)
            {
                if (item is Label && (item as Label).Tag != null && (item as Label).Tag != (sender as Label).Tag)
                {
                    (item as Label).Opacity = 0.7;
                    
                }
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {

            try
            {
                Tp.Visibility = Visibility.Visible;
                Tp.IsOpen = true;
                Tp.DataContext = (sender as Label).Tag;
                
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            
        }

        private void IntervalCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                int index = IntervalCbx.SelectedIndex;
                if(index == 0 || index == 1)
                {
                    int weekDay = (int)DateEnd.DayOfWeek;
                    if(weekDay != 0)
                    {
                        DateEnd = DateEnd.AddDays(7 - weekDay).Date;
                    }

                    if(index == 0)
                    {
                        DateStart = DateEnd.AddDays(-6);
                    }
                    else
                    {
                        DateStart = DateEnd.AddDays(-13);
                    }
                }
                if(index == 2)
                {
                    DateStart = new DateTime(DateStart.Year, DateStart.Month, 1);
                    DateEnd = DateStart.AddMonths(1).AddDays(-1);
                }
                if(index == 3)
                {
                    DateStart = new DateTime(DateStart.Year, 1, 1);
                    DateEnd = DateStart.AddYears(1).AddDays(-1);
                }

                FromLb.Content = DateStart.ToString("dd MMMM yyyy") + "г.";
                ToLb.Content = DateEnd.ToString("dd MMMM yyyy") + "г.";

                File.WriteAllText("Dates.txt", DateStart + ";" + DateEnd);
                File.WriteAllText("Interval.txt", index.ToString());
                loadGant();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int index = IntervalCbx.SelectedIndex;
                if(index == 0)
                {
                    DateStart = DateStart.AddDays(-7);
                    DateEnd = DateEnd.AddDays(-7);
                }
                if (index == 1)
                {
                    DateStart = DateStart.AddDays(-14);
                    DateEnd = DateEnd.AddDays(-14);
                }
                if (index == 2)
                {
                    DateStart = DateStart.AddMonths(-1);
                    DateEnd = DateEnd.AddMonths(-1);
                }
                if (index == 3)
                {
                    DateStart = DateStart.AddYears(-1);
                    DateEnd = DateEnd.AddYears(-1);
                }
                FromLb.Content = DateStart.ToString("dd MMMM yyyy") + "г.";
                ToLb.Content = DateEnd.ToString("dd MMMM yyyy") + "г.";
                File.WriteAllText("Dates.txt", DateStart + ";" + DateEnd);
                loadGant();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = IntervalCbx.SelectedIndex;
                if (index == 0)
                {
                    DateStart = DateStart.AddDays(7);
                    DateEnd = DateEnd.AddDays(7);
                }
                if (index == 1)
                {
                    DateStart = DateStart.AddDays(14);
                    DateEnd = DateEnd.AddDays(14);
                }
                if (index == 2)
                {
                    DateStart = DateStart.AddMonths(1);
                    DateEnd = DateEnd.AddMonths(1);
                }
                if (index == 3)
                {
                    DateStart = DateStart.AddYears(1);
                    DateEnd = DateEnd.AddYears(1);
                }
                FromLb.Content = DateStart.ToString("dd MMMM yyyy") + "г.";
                ToLb.Content = DateEnd.ToString("dd MMMM yyyy") + "г.";
                File.WriteAllText("Dates.txt", DateStart + ";" + DateEnd);
                loadGant();
            }

            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Dates.txt"))
            {
                string input = File.ReadAllText("Dates.txt");
                DateStart = (DateTime)new DateTimeConverter().ConvertFromString(input.Split(';')[0]);
                DateEnd = (DateTime)new DateTimeConverter().ConvertFromString(input.Split(';')[1]);
            }
            if (File.Exists("Interval.txt"))
            {
                string input = File.ReadAllText("Interval.txt");
                IntervalCbx.SelectedIndex = Convert.ToInt32(input);
            }
            if (File.Exists("Scale.txt"))
            {
                string input = File.ReadAllText("Scale.txt");
                ScaleSl.Value = Convert.ToDouble(input);
            }
            else
            {
                IntervalCbx.SelectedIndex = 0;
            }
            loadGant();
        }

        private void ScaleSl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(firstLoad)
            {
                firstLoad = false;
            }
            else
            {
                File.WriteAllText("Scale.txt", ScaleSl.Value.ToString());
                loadGant();
            }
            
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "CSV|*.csv";
                if(openFile.ShowDialog() == true)
                {

                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void GantGrid_MouseMove(object sender, MouseEventArgs e)
        {
            
            Tp.Visibility = Visibility.Collapsed;
            Tp.IsOpen = false;
            Tp.DataContext = null;
        }
    }
}
