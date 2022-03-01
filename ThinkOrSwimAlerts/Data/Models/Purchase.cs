using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThinkOrSwimAlerts.Enums;

namespace ThinkOrSwimAlerts.Data.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        public long PurhaseId { get; set; }
        public Position Position { get; set; }
        public float BuyPrice { get; set; }
        public DateTimeOffset Bought { get; set; }
        public FifteenMinuteInterval Bought15MinuteInterval { get; set; }
        public DayOfWeek Day { get; set; }
        public int Quantity { get; set; }
    }
}
