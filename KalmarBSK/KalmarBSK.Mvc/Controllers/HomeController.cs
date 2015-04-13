using KalmarBSK.DataAccess;
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
            return View();
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
            return View();
        }


        public ActionResult SearchFilter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchFilter(string search)
        {
            List<Meeting> list;
            using (var ctx = new KalmarBSKEntities())
            {
                list = ctx.Meetings.Where(x => x.Location.Contains(search)).ToList();
            }
            return View(list);
        }
        


    }
}