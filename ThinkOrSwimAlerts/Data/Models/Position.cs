using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThinkOrSwimAlerts.Enums;

namespace ThinkOrSwimAlerts.Data.Models
{
    public class Position
    {
        [Key]
        [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        public uint PositionId { get; set; }
        public string Symbol { get; set; }
        public string Underlying { get; set; }
        public PutOrCall PutOrCall { get; set; }
        public DateTimeOffset FirstBuy { get; set; }
        public DateTimeOffset? FinalSell { get; set; }
        public Indicator Indicator { get; set; }
        public short IndicatorVersion { get; set; }

        // Highest Price that this position achieved during its lifetime
        public float HighPrice { get; set; }

        // Lowest Price that this position achieved during its lifetime
        public float LowPrice { get; set; }
        public short MaxQuantity { get; set; }
        public short CurrentQuantity { get; set; }
        public float GainOrLoss { get; set; }
        public float AvgBuyPrice { get; set; }
    }
}
