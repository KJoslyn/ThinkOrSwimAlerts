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
        public uint PurchaseId { get; set; }
        public uint PositionId { get; set; }
        public Position Position { get; set; }
        public float BuyPrice { get; set; }
        public int SecondsAfterFirstBuy { get; set; }
        public FifteenMinuteInterval Bought15MinuteInterval { get; set; }
        public DayOfWeek Day { get; set; }
        public short Quantity { get; set; }
    }
}
