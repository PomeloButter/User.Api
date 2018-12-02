using Consul;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Project.API.Applications.Queries;
using Project.API.Applications.Service;
using Project.API.Dtos;
using Project.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using Project.Domain.AggregatesModel;
using Project.Infrastructure.Repositories;

namespace Project.API
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
            services.AddMediatR();
            services.AddDbContext<ProjectContext>(option =>
            {
                option.UseMySQL(Configuration.GetConnectionString("mysql"),p =>
                    {
                        p.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    });
            });
            services.AddScoped<IRecommendService, RecommendService>();
            services.AddScoped<IProjectQueries, ProjectQueries>();
            services.AddScoped<IProjectRepository, ProjectRepository>(p =>            
                new ProjectRepository(p.GetRequiredService<ProjectContext>())
            );
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            //注册consul
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

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
                    option.Audience = "project_api";
                    option.Authority = "http://localhost:8080";
                });
            services.AddCap(option =>
            {
                option.UseEntityFramework<ProjectContext>();
                option.UseRabbitMQ("localhost");
                option.UseDashboard();
                option.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5003;
                    d.NodeId = 3;
                    d.NodeName = "CAP Project Node";
                });
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, IOptions<ServiceDiscoveryOptions> serviceOptions, IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            applicationLifetime.ApplicationStarted.Register(() => { RegisterService(app, serviceOptions, consul); });
            applicationLifetime.ApplicationStopped.Register(() => { DeRegisterService(app, serviceOptions, consul); });
            app.UseAuthentication();
            app.UseCap();
            app.UseMvc();
        }
        private void RegisterService(IApplicationBuilder app, IOptions<ServiceDiscoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            if (features != null)
            {
                var addresses = features.Get<IServerAddressesFeature>()
                    .Addresses
                    .Select(p => new Uri(p));

                foreach (var address in addresses)
                {
                    var serviceId = $"{serviceOptions.Value.UserServiceName}_{address.Host}:{address.Port}";

                    var httpCheck = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                        Interval = TimeSpan.FromSeconds(30),
                        HTTP = new Uri(address, "HealthCheck").OriginalString
                    };

                    var registration = new AgentServiceRegistration()
                    {
                        Checks = new[] { httpCheck },
                        Address = address.Host,
                        ID = serviceId,
                        Name = serviceOptions.Value.UserServiceName,
                        Port = address.Port
                    };

                    consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
                }
            }
        }

        private void DeRegisterService(IApplicationBuilder app, IOptions<ServiceDiscoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            if (features != null)
            {
                var addresses = features.Get<IServerAddressesFeature>()
                    .Addresses
                    .Select(p => new Uri(p));

                foreach (var address in addresses)
                {
                    var serviceId = $"{serviceOptions.Value.UserServiceName}_{address.Host}:{address.Port}";
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                }
            }
        }
    }
}
