using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Options;
using User.Identity.Dtos;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private  string _userServiceUrl;
        private HttpClient _httpClient;

        public UserService(HttpClient httpClient,IDnsQuery dnsQuery,IOptions<ServiceDiscoveryOptions> options)
        {
            _httpClient = httpClient;
            var address=dnsQuery.ResolveService("service.consul",options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = address.First().AddressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}/";
        }

        public async Task<int> CheckOrCreate(string phone)
        {
            var form = new Dictionary<string, string>() {{"phone", phone}};
            var content = new FormUrlEncodedContent(form);
            var response = await _httpClient.PostAsync(_userServiceUrl + "api/users/check-or-create", content);
            if (response.StatusCode==HttpStatusCode.OK)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int intUserId);
                return intUserId;
            }
            return 0;
        }
    }
}