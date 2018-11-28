using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Data;
using Contact.API.Dots;
using Contact.API.IntegrationEvents.EventHandling;
using Contact.API.Intrastructure;
using Contact.API.Services;
using DnsClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;

namespace Contact.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(x =>
            {
                x.MongoConnectionString = Configuration["MongoConnectionString"].ToString();
                x.MongoContactDatabase = Configuration["MongoContactDatabase"].ToString();
            });
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            //注册全局 单例
            services.AddSingleton(typeof(ResilienceClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilienceHttpClient>>();
                var httpcontextAccesser = sp.GetRequiredService<IHttpContextAccessor>();
                var retryCount = 5;
                var exceptionCountAllowedBeforeBreaking = 5;
                return new ResilienceClientFactory(httpcontextAccesser, logger, retryCount, exceptionCountAllowedBeforeBreaking);
            });

            services.AddSingleton<IHttpClient>(sp =>
            {
                var resilienceClientFactory = sp.GetRequiredService<ResilienceClientFactory>();
                return resilienceClientFactory.GetResilienceHttpClient();
            });
            services.AddScoped<IContactRepository, MongoContactRepository>();
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserPrfileChangedHandler>();
            services.AddSingleton(typeof(ContactContext));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.RequireHttpsMetadata=false;
                    option.Audience = "contact_api";
                    option.Authority = "http://localhost:8080";
                    option.SaveToken = true;
                });
           
            services.AddMvc();

            services.AddCap(option =>
            {
                option.UseMySql(Configuration.GetConnectionString("mysql"));
                option.UseRabbitMQ("localhost");
                option.UseDashboard();
                option.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5801;
                    d.NodeId = 2;
                    d.NodeName = "CAP NO.2 Node";
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCap();
            app.UseMvc();
        }
    }
}
