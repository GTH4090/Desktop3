using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSRSim3.Models
{
    public partial class Project
    {
        public string DisplayTitle { get
            {
                if(this.FullTitle.Split(' ').Count() >= 2)
                {
                    return this.FullTitle.Split(' ')[0][0] + " " + this.FullTitle.Split(' ')[1][0];
                }
                else
                {
                    return this.FullTitle[0] + " " + this.FullTitle[1];
                }
            } }
    }
}
