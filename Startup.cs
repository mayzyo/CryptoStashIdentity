// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System;
using Duende.IdentityServer;
using CryptoStashIdentity.Data;
using IdentityServerHost.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace CryptoStashIdentity
{
    public class Startup
    {
        public IWebHostEnvironment HostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            HostEnvironment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Setup Entity Core connection to PostgreSQL.
            NpgsqlConnectionStringBuilder connBuilder;
            // Get connection string from user secrets.
            connBuilder = new NpgsqlConnectionStringBuilder(Configuration.GetConnectionString("IdentityDb"));
            if (Configuration["IdentityDb"] != null) connBuilder.Password = Configuration["IdentityDb"];

            //services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connBuilder.ConnectionString));
            //services.AddDbContext<ApplicationDbContext>(options => 
            //    options.UseNpgsql(connBuilder.ConnectionString, 
            //        o => o.MigrationsAssembly(typeof(Startup).Assembly.FullName).MigrationsHistoryTable("MyConfigurationMigrationTable", "myConfigurationSchema")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connBuilder.ConnectionString,
                    o => o.MigrationsAssembly(typeof(Startup).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(new Config(Configuration).Clients)
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Setup CORS policy based on environment variable.
            var origins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "*";
            // CORS setting with CorsPolicyBuilder.
            app.UseCors(builder =>
            {
                builder
                .WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}