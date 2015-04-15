using KalmarBSK.DataAccess;
using KalmarBSK.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Threading;
using System.Web.UI;

namespace KalmarBSK.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities())
            {
                sr.Meetings = ctx.GameLocations.Include(x => x.MeetingParticipants).ToList();
                sr.Members = ctx.Personers.ToList();
            }

            return View(sr);
        }

        public ActionResult Meetings()
        {
            List<GameLocation> list;
            using (var ctx = new KlubbdatabasEntities())
            {
                list = ctx.GameLocations.Include(x => x.MeetingParticipants).Take(20).ToList();
            }
            return View(list);
        }


        public ActionResult Members()
        {
            List<Personer> list;
            using (var ctx = new KlubbdatabasEntities())
            {
                list = ctx.Personers.Take(20).ToList();
            }
            return View(list);
        }


        public ActionResult SearchFilter(string search, bool members = true, bool meetings = true, bool oldMeetings = true)
        {
            search = search.ToUpper();
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities())
            {
                ctx.Configuration.ProxyCreationEnabled = false;
                if (meetings)
                {
                    sr.Meetings = ctx.GameLocations.Include(x => x.MeetingParticipants).ToList();
                    if (!oldMeetings)
                    {
                        sr.Meetings = sr.Meetings.Where(x => x.IsUpcoming).ToList();
                    }
                }
                if (members)
                {
                    sr.Members = ctx.Personers.ToList();
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                if (meetings)
                {

                    sr.Meetings = sr.Meetings.Where(x => x.Adress.ToUpper().Contains(search)).ToList();

                }
                if (members)
                {

                    sr.Members = sr.Members.Where(x => x.Namn.ToUpper().Contains(search) || x.Adress.ToUpper().Contains(search)).ToList();

                }
            }




            return PartialView("~/Views/Shared/_SearchFilter.cshtml", sr);
        }

        public ActionResult GetParticipants(string JSONModel)
        {
            List<MeetingParticipant> list = JsonConvert.DeserializeObject<List<MeetingParticipant>>(JSONModel);

            List<Personer> participants;
            using (var ctx = new KlubbdatabasEntities())
            {

                var pers = ctx.Personers.ToList();

                participants = pers.Join<Personer, MeetingParticipant, int, Personer>(list
                    , x => x.ID
                    , y => y.PersonId
                    , (x, y) => new Personer { Adress = x.Adress, ID = x.ID, Namn = x.Namn, Telefon = x.Telefon }).ToList();
            }

            return PartialView("~/Views/Shared/_Participants.cshtml", participants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember(Personer person)
        {
            if (!ModelState.IsValid)
            {
                return View(person);
            }
            using (var ctx = new KlubbdatabasEntities())
            {
                ctx.Personers.Add(person);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult AddMember()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMember(Personer person)
        {
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            using (var ctx = new KlubbdatabasEntities())
            {
                var model = ctx.Personers.Find(person.ID);
                if (TryUpdateModel(model))
                {
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult EditMember(int id)
        {
            Personer person;
            using (var ctx = new KlubbdatabasEntities())
            {
                person = ctx.Personers.Where(x => x.ID == id).FirstOrDefault();
            }
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMember(Personer person)
        {
            using (var ctx = new KlubbdatabasEntities())
            {
                var model = ctx.Personers.Find(person.ID);
                model.Active = false;
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteMember(int id)
        {
            Personer person;
            using (var ctx = new KlubbdatabasEntities())
            {
                person = ctx.Personers.Where(x => x.ID == id).FirstOrDefault();
            }
            return View(person);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMeeting(GameLocation meeting)
        {
            if (!ModelState.IsValid)
            {
                return View(meeting);
            }
            using (var ctx = new KlubbdatabasEntities())
            {
                ctx.GameLocations.Add(meeting);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult NewMeeting()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMeeting(GameLocation meeting, int[] removeId)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(meeting);
            //}

            using (var ctx = new KlubbdatabasEntities())
            {
                var model = ctx.GameLocations.Find(meeting.ID);
                if (TryUpdateModel(model))
                {
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult EditMeeting(int id)
        {
            GameLocation meeting;
            using (var ctx = new KlubbdatabasEntities())
            {
                //meeting = ctx.GameLocations.Where(x => x.ID == id).Include(y => y.MeetingParticipants.Select(p => p.Personer)).FirstOrDefault();
                meeting = (GameLocation)ctx.GameLocations.Where(x => x.ID == id).Include("MeetingParticipants.Personer").FirstOrDefault();
            }
            return View(meeting);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMeeting(GameLocation meeting)
        {
            using (var ctx = new KlubbdatabasEntities())
            {
                var model = ctx.GameLocations.Find(meeting.ID);
                ctx.GameLocations.Remove(model);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteMeeting(int id)
        {
            GameLocation meeting;
            using (var ctx = new KlubbdatabasEntities())
            {
                meeting = ctx.GameLocations.Where(x => x.ID == id).FirstOrDefault();
            }
            return View(meeting);
        }

    }
}