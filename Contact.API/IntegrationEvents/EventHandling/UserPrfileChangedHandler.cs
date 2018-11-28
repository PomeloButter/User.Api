using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Data;
using Contact.API.Dots;
using Contact.API.IntegrationEvents.Events;
using DotNetCore.CAP;

namespace Contact.API.IntegrationEvents.EventHandling
{
    public class UserPrfileChangedHandler:ICapSubscribe
    {
        private readonly IContactRepository _caContactRepository;

        public UserPrfileChangedHandler(IContactRepository caContactRepository)
        {
            _caContactRepository = caContactRepository;
        }
        [CapSubscribe("finbook.userapi.userprofilechanged")]
        public async Task UpdateContactInfo(UserProfileChangedEvent @event)
        {
            var token = new CancellationToken();
            await _caContactRepository.UpdateContactInfoAsync(new UserIdentity()
            {
                Avatar = @event.Avatar,
                Company =@event.Company,
                Name = @event.Name,
                Title = @event.Title,
                UserId = @event.UserId
            },token);
        }
    }
}
