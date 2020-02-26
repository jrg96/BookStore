using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BookStore.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository;
using AutoMapper;
using BookStore.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookStoreUtility;

namespace BookStore
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BookStore")));
            
            /*
             * -------------------------------------------------------------------------------------
             * AGREGANDO CUSTOM IDENTITY
             * -------------------------------------------------------------------------------------
             */
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSingleton<IEmailSender, CustomEmailSender>();

            
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            // Agregar clases repositorio
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            // Agregando automapper
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
