using KalmarBSK.DataAccess;
using KalmarBSK.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KalmarBSK.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KalmarBSKEntities())
            {
                sr.Meetings = ctx.Meetings.Take(20).ToList();
                sr.Members = ctx.Members.Take(20).ToList();
            }

            return View(sr);
        }

        public ActionResult Meetings()
        {
            List<Meeting> list;
            using (var ctx = new KalmarBSKEntities())
            {
                list = ctx.Meetings.Take(20).ToList();
            }
            return View(list);
        }


        public ActionResult Members()
        {
            List<Member> list;
            using (var ctx = new KalmarBSKEntities())
            {
                list = ctx.Members.Take(20).ToList();
            }
            return View(list);
        }


        public ActionResult SearchFilter(string search, bool members = false, bool meetings = false)
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KalmarBSKEntities())
            {
                if (meetings)
                {
                    sr.Meetings = ctx.Meetings.ToList(); 
                }
                if (members)
                {
                    sr.Members = ctx.Members.ToList(); 
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                if (meetings)
                {

                    sr.Meetings = sr.Meetings.Where(x => x.Location.Contains(search)).ToList();
                    
                }
                if (members)
                {

                    sr.Members = sr.Members.Where(x => x.FirstName.Contains(search)).ToList();
                    
                }
            }




            return PartialView("~/Views/Shared/_SearchFilter.cshtml", sr);
        }

        public ActionResult GetParticipants(int number)
        {
            List<Member> list;
            using (var ctx = new KalmarBSKEntities())
            {
                list = ctx.Members.Take(number).ToList();
            }
            return PartialView("~/Views/Shared/_Participants.cshtml", list);
        }

    }
}