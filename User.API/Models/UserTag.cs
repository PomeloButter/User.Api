using System;

namespace User.API.Models
{
    public class UserTag
    {
        public int UserId { get; set; }
        public string Tag { get; set; }
        public DateTime CreateTime { get; set; }
    }
}