using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUA.Core.Models
{
    public class WeekFileUrl
    {
        public int id { get; set; }
        public FileUrl? Url { get; set; }

        // Foreign key to Week entity
        public int WeekId { get; set; }
        public Week? Week { get; set; }
    }
}
