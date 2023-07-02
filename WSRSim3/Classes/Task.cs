using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.TextFormatting;

namespace WSRSim3.Models
{
    public partial class Task
    {
        public int SortNum { get
            {
                if(this.StatusId == 2 && this.Deadline <= DateTime.Now)
                {
                    return 1;
                }
                if (this.StatusId == 1 && this.Deadline <= DateTime.Now)
                {
                    return 2;
                }
                if (this.StatusId == 2 && this.Deadline > DateTime.Now)
                {
                    return 3;
                }
                if (this.StatusId == 1 && this.Deadline > DateTime.Now)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            } }


        /// <summary>
        /// Поле для данных, отображающихся во всплывающем окне на окне ганта
        /// </summary>
        public string SpecialTime { get
            {
                if(this.StatusId == 3)
                {
                    try
                    {
                        return "Фактически потраченное время: " + (this.FinishActualTime - this.StartActualTime).ToString();
                    }
                    catch (Exception)
                    {

                        return "Фактически потраченное время: нет данных";
                    }
                    
                }
                if(this.StatusId == 2)
                {
                    return "Время до дедлайна: " + (this.Deadline - DateTime.Now).ToString();
                }
                if( this.StatusId == 1)
                {
                    DateTime start = new DateTime();
                    if(this.StartActualTime != null)
                    {
                        start = (DateTime)this.StartActualTime;
                    }
                    else
                    {
                        start = (DateTime)this.CreatedTime;
                    }
                    return "Планируемое время на выполнение: " + (this.Deadline - start).ToString();
                }
                else
                {
                    return "Данных нет";
                }
            } }
    }
}
