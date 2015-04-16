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
using KalmarBSK.Mvc.Models;
using System.Data.Entity.Validation;

namespace KalmarBSK.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities2())
            {
                sr.Meetings = ctx.GameLocations.Include(x => x.MeetingParticipants).ToList();
                sr.Members = ctx.Personers.ToList();
            }

            return View(sr);
        }

        public ActionResult Meetings()
        {
            List<GameLocation> list;
            using (var ctx = new KlubbdatabasEntities2())
            {
                list = ctx.GameLocations.Include(x => x.MeetingParticipants).Take(20).ToList();
            }
            return View(list);
        }

        public ActionResult Members()
        {
            List<Personer> list;
            using (var ctx = new KlubbdatabasEntities2())
            {
                list = ctx.Personers.Take(20).ToList();
            }
            return View(list);
        }


        public ActionResult SearchFilter(string search, bool members = true, bool meetings = true, bool oldMeetings = true)
        {
            search = search.ToUpper();
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
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

            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
            {
                person = ctx.Personers.Where(x => x.ID == id).FirstOrDefault();
            }
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMember(Personer person)
        {
            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
            {
                ctx.GameLocations.Add(meeting);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult NewMeeting()
        {
            GameLocation model = new GameLocation();
            using (var ctx = new KlubbdatabasEntities2())
            {
                model.ID = ctx.GameLocations.Max(x => x.ID) + 1;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMeeting(GameLocation meeting)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(meeting);
            //}

            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
            {
                //meeting = ctx.GameLocations.Where(x => x.ID == id).Include(y => y.MeetingParticipants.Select(p => p.Personer)).FirstOrDefault();
                meeting = (GameLocation)ctx.GameLocations.Where(x => x.ID == id).Include("MeetingParticipants.Personer").FirstOrDefault();
                //meeting = ctx.GameLocations.Where(x => x.ID == id).Include(x => x.MeetingParticipants).FirstOrDefault();

            }
            return View(meeting);
        }
        public ActionResult DeleteParticipant(int participantId, int meetingId)
        {
            ICollection<MeetingParticipant> collection;
            using (var ctx = new KlubbdatabasEntities2())
            {
                var participant = ctx.MeetingParticipants.Find(participantId);
                ctx.MeetingParticipants.Remove(participant);
                ctx.SaveChanges();

                collection = ctx.MeetingParticipants.Where(x => x.GameLocationId == meetingId).Include(y => y.Personer).ToList();
            }

            return PartialView("~/Views/Shared/_Participants.cshtml", collection);
        }
        public ActionResult GetAvaibleMembers(int meetingId)
        {
            AddMember model = new AddMember();
            model.CurrentMeetingId = meetingId;
            using (var ctx = new KlubbdatabasEntities2())
            {
                model.AvailableMembers = ctx.Personers.Where(x => x.MeetingParticipants.All(y => y.GameLocationId != meetingId)).Where(z => z.Active).ToList();

            }

            return PartialView("~/Views/Shared/_Members.cshtml", model);
        }
        public ActionResult AddParticipants(AddMember model)
        {
            string errors = "";
            ICollection<MeetingParticipant> collection;
            using (var ctx = new KlubbdatabasEntities2())
            {
                GameLocation meeting = ctx.GameLocations.Where(x => x.ID == model.CurrentMeetingId).SingleOrDefault();
                if (meeting == null)
                {
                    meeting = ctx.GameLocations.Add(new GameLocation { ID = model.CurrentMeetingId, Datum = DateTime.Now, Adress="blä" });
                }
                foreach (var item in model.AvailableMembers.Where(x => x.Add))
                {
                    //Personer person = ctx.Personers.Where(x => x.ID == item.ID).SingleOrDefault();
                    meeting.MeetingParticipants.Add(new MeetingParticipant { PersonId = item.ID });
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            errors += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                errors += string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"\n",
                                    ve.PropertyName,
                                    eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                    ve.ErrorMessage);
                            }
                        }
                        //return errors;
                    }
                }

                collection = ctx.MeetingParticipants.Where(x => x.GameLocationId == model.CurrentMeetingId).Include(y => y.Personer).ToList();
            }
            //return errors;
            return PartialView("~/Views/Shared/_Participants.cshtml", collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMeeting(GameLocation meeting)
        {
            using (var ctx = new KlubbdatabasEntities2())
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
            using (var ctx = new KlubbdatabasEntities2())
            {
                meeting = ctx.GameLocations.Where(x => x.ID == id).FirstOrDefault();
            }
            return View(meeting);
        }

    }
}