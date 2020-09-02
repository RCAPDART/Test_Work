using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Test_Work.Helpers;
using Test_Work.Models;
using Test_Work.Repository;

namespace Test_Work
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
            services.AddSingleton<INHibernateHelper, NHibernateHelper>();
            services.AddSingleton<IUrlMinificationHelper, UrlMinificationHelper>();

            services.AddSingleton<IRepository<Url>, Repository<Url>>();

            services.AddControllersWithViews();
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
            }

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("RedirectMin", "Min/{minUrl}",
                new { controller = "Home", action = "RedirectMin" });

                endpoints.MapControllerRoute("Homepage", "{controller=Home}/{action=Index}",
                    new { controller = "Home", action = "Index" });
            });
            // I don't have experience with NHibernate, but I believe, that it should be done by migration, not by hardcoded thing in the startup.cs
            var nhHelper = new NHibernateHelper();
            nhHelper.InitTable<Models.Url>();


        }
    }
}
