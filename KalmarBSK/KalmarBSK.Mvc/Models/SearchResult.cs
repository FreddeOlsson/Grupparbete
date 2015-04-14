using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KalmarBSK.DataAccess;

namespace KalmarBSK.Mvc.Models
{
    public class SearchResult
    {
        public List<Personer> Members { get; set; }
        public List<GameLocation> Meetings { get; set; }
    }
}