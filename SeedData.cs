// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using CryptoStashIdentity.Data;
using IdentityServerHost.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Npgsql;

namespace CryptoStashIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlite(connectionString, o => o.MigrationsAssembly(typeof(Startup).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var michael = userMgr.FindByNameAsync("michael").Result;
                    if (michael == null)
                    {
                        michael = new ApplicationUser
                        {
                            UserName = "michael",
                            Email = "michael@email.com",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(michael, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(michael, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Michael May"),
                            new Claim(JwtClaimTypes.GivenName, "Michael"),
                            new Claim(JwtClaimTypes.FamilyName, "May")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("michael created");
                    }
                    else
                    {
                        Log.Debug("michael already exists");
                    }

                    var james = userMgr.FindByNameAsync("james").Result;
                    if (james == null)
                    {
                        james = new ApplicationUser
                        {
                            UserName = "james",
                            Email = "james@email.com",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(james, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(james, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "James Mei"),
                            new Claim(JwtClaimTypes.GivenName, "James"),
                            new Claim(JwtClaimTypes.FamilyName, "Mei")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("james created");
                    }
                    else
                    {
                        Log.Debug("james already exists");
                    }
                }
            }
        }
    }
}
