using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Resilience;

namespace User.Identity.Intrastructure
{
    public class ResilienceClientFactory
    {
        private readonly ILogger<ResilienceHttpClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _retryCount;
        private readonly int _exceptionCountAllowedBeforeBreaking;

        public ResilienceClientFactory(IHttpContextAccessor httpContextAccessor, ILogger<ResilienceHttpClient> logger, int retryCount, int exceptionCountAllowedBeforeBreaking)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _retryCount = retryCount;
            _exceptionCountAllowedBeforeBreaking = exceptionCountAllowedBeforeBreaking;
        }

        public ResilienceHttpClient GetResilienceHttpClient()=>
            new ResilienceHttpClient(CreatePolicy, _logger,_httpContextAccessor );

        private Policy[] CreatePolicy(string origin)
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    _retryCount,
                    retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            var ms = $"Retry {retryCount} implementd with poly's RetryPolicy" +
                                     $"of {context.PolicyKey}" +
                                     $"at {context.ExecutionKey}" +
                                     $"due to :{exception}";
                            _logger.LogWarning(ms);
                            _logger.LogDebug(ms);
                        }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    _exceptionCountAllowedBeforeBreaking,TimeSpan.FromMinutes(1),
                    (exception, duration) =>
                    {
                        _logger.LogTrace("熔断器打开");
                    },
                    () =>
                    {
                       _logger.LogTrace("熔断器关闭");
                    })
            };
        }
    }
}