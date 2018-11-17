using System;

namespace User.API.Models
{
    public class BpFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string OriginFilePath { get; set; }
        public string FromatFilePath { get; set; }
        public DateTime CreateTime { get; set; }
    }
}