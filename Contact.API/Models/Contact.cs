using System.Collections.Generic;

namespace Contact.API.Models
{
    public class Contact
    {
        public Contact()
        {
            Tags=new List<string>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public List<string> Tags { get; set; }
    }
}