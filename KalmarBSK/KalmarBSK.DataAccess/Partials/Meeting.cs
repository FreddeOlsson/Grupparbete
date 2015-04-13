using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmarBSK.DataAccess
{
    public partial class Meeting
    {
        public bool IsUpcoming { get { return this.Date > DateTime.Now; } }
    }
}