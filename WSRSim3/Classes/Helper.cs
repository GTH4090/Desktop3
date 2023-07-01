using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WSRSim3.Models;


namespace WSRSim3.Classes
{
    internal class Helper
    {
        public static WSRSim3Entities Db = new WSRSim3Entities();
        public static Project SelectedProject = null;

        public static void Error(string message = "Ошибка подключения к БД")
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void Info(string message = "Ошибка подключения к БД")
        {
            MessageBox.Show(message, "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
