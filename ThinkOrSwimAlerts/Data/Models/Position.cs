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
        public long PositionId { get; set; }
        public string Symbol { get; set; }
        public string Underlying { get; set; }
        public PutOrCall PutOrCall { get; set; }
        public DateTimeOffset FirstBuy { get; set; }
        public DateTimeOffset FinalSell { get; set; }
        public Indicator Indicator { get; set; }
        public string IndicatorVersion { get; set; }
    }
}
