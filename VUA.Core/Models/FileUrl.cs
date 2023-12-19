using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUA.Core.Models
{
    public class FileUrl
    {
        public int id { get; set; }
        public string? UrlString { get; set; }
        public ICollection<WeekFileUrl>? weekFileUrls { get; set; }
    }
}
