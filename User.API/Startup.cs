using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using User.API.Data;
using User.API.Dots;
using User.API.Filter;

namespace User.API
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
            services.AddDbContext<UserContext>(option =>
            {
                option.UseMySQL(Configuration.GetConnectionString("mysql"));
            });

            //拿consul配置
            services.Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            //注册consul
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.RequireHttpsMetadata = false;
                    option.Audience = "user_api";
                    option.Authority = "http://localhost:8080";
                });
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));
            });

            services.AddCap(option =>
            {
                option.UseEntityFramework<UserContext>()
                .UseRabbitMQ("localhost")
                .UseDashboard();
                option.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5800;
                    d.NodeId = 1;
                    d.NodeName = "CAP NO.1 Node";
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启动时注册服务
            applicationLifetime.ApplicationStarted.Register(() => { RegisterService(app, serviceOptions, consul); });
            applicationLifetime.ApplicationStopped.Register(() => { DeRegisterService(app, serviceOptions, consul); });
            app.UseCap();
            app.UseAuthentication();
            app.UseMvc();
        }

        private void RegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            if (features != null)
            {
                var addresses = features.Get<IServerAddressesFeature>()
                    .Addresses
                    .Select(p => new Uri(p));

                foreach (var address in addresses)
                {
                    var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                    var httpCheck = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                        Interval = TimeSpan.FromSeconds(30),
                        HTTP = new Uri(address, "HealthCheck").OriginalString
                    };

                    var registration = new AgentServiceRegistration()
                    {
                        Checks = new[] {httpCheck},
                        Address = address.Host,
                        ID = serviceId,
                        Name = serviceOptions.Value.ServiceName,
                        Port = address.Port
                    };

                    consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
                }
            }
        }

        private void DeRegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            if (features != null)
            {
                var addresses = features.Get<IServerAddressesFeature>()
                    .Addresses
                    .Select(p => new Uri(p));

                foreach (var address in addresses)
                {
                    var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                }
            }
        }
    }
}