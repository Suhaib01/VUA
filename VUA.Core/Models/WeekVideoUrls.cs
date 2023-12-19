using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUA.Core.Models
{
    public class WeekVideoUrls
    {
        public int id { get; set; }
        public ViduoUrl? Url { get; set; }

        // Foreign key to Week entity
        public int WeekId { get; set; }
        public Week? Week { get; set; }
    }
}
