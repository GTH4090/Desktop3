using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WSRSim3.Classes.Helper;

namespace WSRSim3.Models
{
    public partial class Employee
    {
        public int ClosedCount
        {
            get
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                return this.Task.Where(el => el.ProjectId == SelectedProject.Id && el.StatusId == 3
                && (((el.StartActualTime < startDate || el.CreatedTime < startDate) &&
                (el.FinishActualTime >= startDate || el.Deadline >= startDate)) || ((el.StartActualTime >= startDate || el.CreatedTime >= startDate) &&
                (el.StartActualTime <= endDate || el.CreatedTime <= endDate)))).Count();
            }
        }
        public int DeadlinedCount
        {
            get
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                return this.Task.Where(el => el.ProjectId == SelectedProject.Id && el.Deadline < DateTime.Now
                && (((el.StartActualTime < startDate || el.CreatedTime < startDate) &&
                (el.FinishActualTime >= startDate || el.Deadline >= startDate)) || ((el.StartActualTime >= startDate || el.CreatedTime >= startDate) &&
                (el.StartActualTime <= endDate || el.CreatedTime <= endDate)))).Count();
            }
        }

    }
}
