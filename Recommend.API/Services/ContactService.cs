using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Recommend.API.Dtos;
using Resilience;

namespace Recommend.API.Services
{
    public class ContactService:IContactService
    {
        private readonly string _contactServiceUrl;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<UserService> _logger;
        public ContactService(IHttpClient httpClient, IDnsQuery dnsQuery, IOptions<ServiceDiscoveryOptions> options, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", options.Value.ContactServiceName);
            var addressList = address.First().AddressList;
            var host = address.First().AddressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _contactServiceUrl = $"http://{host}:{port}/";
        }

        public async Task<List<Contact>> GetContactsByUserId(int userId)
        {
            try
            {

                var response = await _httpClient.GetStringAsync($"{_contactServiceUrl}" + "api/contacts/" + userId);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<List<Contact>>(response);

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