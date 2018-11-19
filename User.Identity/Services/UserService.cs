using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly string _userServiceUrl = "http://localhost:5001/";
        private HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
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