using System;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Data;
using Contact.API.Models;
using Contact.API.Services;
using Contact.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    [Route("api/contacts")]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository, IUserService userService,
            IContactRepository contactRepository)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
            _contactRepository = contactRepository;
        }

        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyRequests(CancellationToken cancellationToken)
        {
            var request =
                await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyRequest(int userId, CancellationToken cancellationToken)
        {
            var baseUserInfo = await _userService.GetBaseUserInfoAsync(userId);
            if (baseUserInfo == null) throw new Exception("用户参数错误");
            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                UserId = userId,
                ApplierId = UserIdentity.UserId,
                Name = baseUserInfo.Name,
                Company = baseUserInfo.Company,
                Title = baseUserInfo.Title,
                ApplyTime = DateTime.Now,
                Avatar = baseUserInfo.Avatar
            }, cancellationToken);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPut]
        [Route("apply-requests")]
        public async Task<IActionResult> ApprovalApplyRequest(int applierId, CancellationToken cancellationToken)
        {
            var result =
                await _contactApplyRequestRepository.ApprovalAsync(UserIdentity.UserId, applierId, cancellationToken);
            if (!result) return BadRequest();
            var userBaseInfo = await _userService.GetBaseUserInfoAsync(applierId);
            var userInfo = await _userService.GetBaseUserInfoAsync(UserIdentity.UserId);
            await _contactRepository.AddContactAsync(UserIdentity.UserId, userInfo, cancellationToken);
            await _contactRepository.AddContactAsync(applierId, userBaseInfo, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _contactRepository.GetContactAsync(UserIdentity.UserId, cancellationToken));
        }

        [HttpGet]
        [Route("tag")]
        public async Task<IActionResult> TagContact([FromBody] TagContactInputViewModel model,
            CancellationToken cancellationToken)
        {
            var result = await _contactRepository.GetTagContactAsync(UserIdentity.UserId, model.ContactId, model.Tags,
                cancellationToken);
            if (result) return Ok();

            return BadRequest();
        }
    }
}