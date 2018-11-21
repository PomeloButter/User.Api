using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Resilience
{
    public interface IHttpClient
    {
         Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken, string requestId = null, string authorizationMethod = "Bearer");
         Task<HttpResponseMessage> PostAsync(string url, Dictionary<string,string> values , string authorizationToken=null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
