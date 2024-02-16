using Abc.Northwind.Business.Abstract;
using Abc.Northwind.Business.Concrete;
using Abc.Northwind.DataAccess.Abstract;
using Abc.Northwind.DataAccess.Concrete.EntityFramework;
using Abc.Northwind.MvcWebUI.Entities;
using Abc.Northwind.MvcWebUI.Middlewares;
using Abc.Northwind.MvcWebUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Abc.Northwind.MvcWebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Servis yap�land�rmas� yap�l�r, servis ba��ml�l�klar�m�z� ekleriz. Dependecy Injection yap�lan yerdir.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<IProductDal, EfProductDal>();

            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICategoryDal, EfCategoryDal>();

            // INFO: Sepet, session'da tutulmaz (�rnek olarak yap�ld�)
            services.AddSingleton<ICartSessionService, CartSessionService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         
            // .net core identity i�in
            services.AddDbContext<CustomIdentityDbContext>(options => 
            options.UseSqlServer(@"Data Source=.;Initial Catalog=Northwind;Integrated Security=True"));
            services.AddIdentity<CustomIdentityUser, CustomIdentityRole>()
                .AddEntityFrameworkStores<CustomIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddSession(); // session'� etkinle�tirmek i�in
            services.AddDistributedMemoryCache(); // session'� uygulama aya�a kalkt���nda bellekte tutmas� i�in

            services.AddMvc();
        }

        // Middleware(orta katman) yap�land�rmas� ger�ekle�tirilen yerdir
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            /*
             Add new item -> npm configuration file -> package.json i�ine istenilen s�r�mlerde k�t�phaneler yaz�l�r.
             Kaydettikten sonra node_modules alt�nda g�r�lecektir.
             Daha sonra package.json kullanarak y�kledi�imiz client k�t�phanelerimizi uygulamada kullanabilmemiz i�in a�a��daki middleware
             wwwroot ile birle�tirir.
             */
            app.UseStaticFiles();
            app.UseNodeModules(env.ContentRootPath);

            app.UseRouting();
            app.UseCors();

            app.UseSession(); // session i�in

            app.UseAuthentication();
            app.UseAuthorization();
            // identity i�in

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}
