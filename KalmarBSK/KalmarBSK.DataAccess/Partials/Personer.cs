using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmarBSK.DataAccess
{
    [MetadataType(typeof(Personer.Metadata))]
    public partial class Personer
    {
        public bool Add { get; set; }

        private sealed class Metadata
        {
            [Required]
            public string Namn { get; set; }
            [Required]
            public string Adress { get; set; }
            [Required]
            public string Telefon { get; set; }
        }
    }
}
