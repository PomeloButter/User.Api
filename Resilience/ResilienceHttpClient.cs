using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;

namespace Resilience
{
    public class ResilienceHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;

        //根据url origin 去创建policy
        private readonly Func<string, Policy[]> _policyCreator;

        //吧policy 打包成组合policy wraper 进行本地缓存
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWraps;
        private ILogger<ResilienceHttpClient> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public ResilienceHttpClient(Func<string, Policy[]> policyCreator, 
            ILogger<ResilienceHttpClient> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _policyWraps = new ConcurrentDictionary<string, PolicyWrap>();
            _logger = logger;
            _contextAccessor = httpContextAccessor;
            _policyCreator = policyCreator;
        }

        public  Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken=null, string requestId = null, string authorizationMethod = "Bearer")
        {
            HttpRequestMessage Func() => CreateHttpRequestMessage(HttpMethod.Post, url, item);
            return  DoPostAsync(HttpMethod.Post, Func, url, authorizationToken, requestId, authorizationMethod);
        }
        public  Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> values, string authorizationToken=null, string requestId = null, string authorizationMethod = "Bearer")
        {
            HttpRequestMessage Func() => CreateHttpRequestMessage(HttpMethod.Post, url, values);
            return  DoPostAsync(HttpMethod.Post, Func, url, authorizationToken, requestId, authorizationMethod);
        }

        private  Task<HttpResponseMessage> DoPostAsync(HttpMethod method, Func<HttpRequestMessage> requestMessageFunc, string url, string authorizationToken, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("value must be either post or put.", nameof(method));
            }
            var origin = GetOriginFromUri(url);
            return HttpInvoker(origin, async () =>
            {
                var requestMessage = requestMessageFunc();
                SetAuthorizationHeader(requestMessage);
                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }
                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }
                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWraps.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWraps.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all 
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }

        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod httpMethod,string url,T item)
        {
            return new HttpRequestMessage(httpMethod,url){Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "appplication/json") };
        }
        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string url, Dictionary<string,string> item)
        {
           return  new HttpRequestMessage(httpMethod, url){Content = new FormUrlEncodedContent(item)};
        }
        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }


        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _contextAccessor?.HttpContext?.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }


    }
}