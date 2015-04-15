using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmarBSK.DataAccess
{
    [MetadataType(typeof(GameLocation.Metadata))]
    public partial class GameLocation
    {
        public bool IsUpcoming { get { return this.Datum > DateTime.Now; } }

        private sealed class Metadata
        {
            [Required]
            [Display(Name="Plats")]
            public string Adress { get; set; }
            [Required]
            public Nullable<System.DateTime> Datum { get; set; }
            [Display(Name = "Deltagare")]
            public ICollection<MeetingParticipant> MeetingParticipants { get; set; }
        }
    }
}
