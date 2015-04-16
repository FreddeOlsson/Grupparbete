using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KalmarBSK.DataAccess;

namespace KalmarBSK.Mvc.Models
{
    public class AddMember
    {
        public List<Personer> AvailableMembers { get; set; }
        public int CurrentMeetingId { get; set; }
    }
}