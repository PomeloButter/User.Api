using System.Collections.Generic;

namespace Contact.API.Models
{
    public class ContactBook
    {
        public int UserId { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}