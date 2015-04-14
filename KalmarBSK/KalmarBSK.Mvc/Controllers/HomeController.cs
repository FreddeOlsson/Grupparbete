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

namespace KalmarBSK.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities())
            {
                sr.Meetings = ctx.GameLocations.Include(x => x.MeetingParticipants).Take(20).ToList();
                sr.Members = ctx.Personers.Take(20).ToList();
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


        public ActionResult SearchFilter(string search, bool members = false, bool meetings = false, bool oldMeetings = false)
        {
            SearchResult sr = new SearchResult();
            using (var ctx = new KlubbdatabasEntities())
            {
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

                    sr.Meetings = sr.Meetings.Where(x => x.Adress.Contains(search)).ToList();
                    
                }
                if (members)
                {

                    sr.Members = sr.Members.Where(x => x.Namn.Contains(search)).ToList();
                    
                }
            }




            return PartialView("~/Views/Shared/_SearchFilter.cshtml", sr);
        }

        public ActionResult GetParticipants(string JSONModel)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ICollection<MeetingParticipant>));
            var yourobject = (ICollection<MeetingParticipant>)ser.ReadObject(GenerateStreamFromString(JSONModel));


            List<Personer> participants;
            //using (var ctx = new KlubbdatabasEntities())
            //{
            //    participants = ctx.Personers.Join<Personer, MeetingParticipant, int, Personer>(list
            //        , x => x.ID
            //        , y => y.PersonId
            //        , (x, y) => new Personer { Adress = x.Adress, ID = x.ID, Namn = x.Namn, Telefon = x.Telefon }).ToList();
            //}
            return PartialView("~/Views/Shared/_Participants.cshtml");
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}