using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
    }
}
