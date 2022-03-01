using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.Data.Models
{
    public class PositionUpdate
    {
        [Key]
        [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        public long PositionUpdateId { get; set; }
        public Position Position { get; set; }
        public int SecondsAfterPurchase { get; set; }
        public float Mark { get; set; }
        public float GainOrLossPct { get; set; }
        public bool IsNewHigh { get; set; }
        public bool IsNewLow { get; set; }
    }
}
