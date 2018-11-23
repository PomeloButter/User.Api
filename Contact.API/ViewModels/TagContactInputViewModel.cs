using System.Collections.Generic;

namespace Contact.API.ViewModels
{
    public class TagContactInputViewModel
    {
        public int ContactId { get; set; }
        public List<string> Tags { get; set; }
    }
}