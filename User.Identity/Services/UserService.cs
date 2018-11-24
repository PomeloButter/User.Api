using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using User.Identity.Dtos;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly string _userServiceUrl;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClient httpClient,IDnsQuery dnsQuery,IOptions<ServiceDiscoveryOptions> options,ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var address=dnsQuery.ResolveService("service.consul",options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = address.First().AddressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}/";
        }

        public async Task<Dtos.UserInfo> CheckOrCreate(string phone)
        {
            var form = new Dictionary<string, string>() {{"phone", phone}};
            try
            {
                var response = await _httpClient.PostAsync($"{_userServiceUrl}" + "api/users/check-or-create", form);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
//                    _logger.LogError($"completed Checkorcreate with userid:{userInfo.Id}");
                    return userInfo;
                }
            }
            catch (Exception e)
            {
               _logger.LogError("checkOrCreate在重试之后失败"+e.Message+e.StackTrace);
                throw e;
            }
         
           
            return null;
        }
    }
}