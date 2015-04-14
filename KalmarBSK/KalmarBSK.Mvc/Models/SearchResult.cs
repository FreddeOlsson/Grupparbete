using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KalmarBSK.DataAccess;

namespace KalmarBSK.Mvc.Models
{
    public class SearchResult
    {
        public List<Member> Members { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
}