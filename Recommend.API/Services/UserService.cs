using System;
using System.Linq;
using System.Threading.Tasks;
using Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Recommend.API.Dtos;
using DnsClient;
namespace Recommend.API.Services
{
    public class UserService:IUserService
    {
        private readonly string _userServiceUrl;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClient httpClient, IDnsQuery dnsQuery, IOptions<ServiceDiscoveryOptions> options, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = address.First().AddressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}/";
        }
        public async Task<UserIdentity> GetBaseUserInfoAsync(int userId)
        {
           
            try
            {
              
                var response = await _httpClient.GetStringAsync($"{_userServiceUrl}" + "api/users/baseinfo/"+userId);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<UserIdentity>(response);                  
                 
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("checkOrCreate在重试之后失败" + e.Message + e.StackTrace);
                throw e;
            }
            return null;
        }
    }
}