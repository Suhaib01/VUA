

namespace VUA.Core.Models
{
    public class ViduoUrl
    {
        public int id { get; set; }
        public string? UrlString { get; set; }
        public ICollection<WeekVideoUrls>? WeekVideoUrls { get; set; }
    }
}
