using System.Collections.Generic;

namespace Contact.API.Models
{
    public class ContactBook
    {
        public ContactBook()
        {
            Contacts=new List<Contact>();
        }

        public int UserId { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}