using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Persistence.Interfaces;
using Export;
using Export.Interfaces;
using Authentication;
using Authentication.Interfaces;
using AutoMapper;

namespace Main
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
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Main.ViewModels.User, Models.User>();
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            })
                .AddJwtBearer("JwtBearer", JwtBeareOptions =>
                {
                    JwtBeareOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetSection("Secrets").GetSection("SecretKey").Value)),
                        ValidateIssuer = false, 
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
            services.AddSingleton(mapper);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IFilingRepository, FilingRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<Authentication.Interfaces.IAuthenticationService, Authentication.AuthenticationService>();
            services.AddSingleton<IExportService, ExportService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
