using System;

namespace CeecDataTransfer
{
    public class Lessons
    {
        public Guid? Id { get; set; }
        public Guid? ChapterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string Type { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Url { get; set; } 
        public bool UsePopup { get; set; }
        public decimal? PassingScore { get; set; }
        public decimal? Weight { get; set; }
        public string RefId { get; set; }
    }
}

