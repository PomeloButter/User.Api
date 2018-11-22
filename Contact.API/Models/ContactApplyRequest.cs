using System;

namespace Contact.API.Models
{
    public class ContactApplyRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public int  ApplyerId { get; set; }

        /// <summary>
        ///     是否通过 0未通过 1通过
        /// </summary>
        public int Approvaled { get; set; }

        public DateTime HandledTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}