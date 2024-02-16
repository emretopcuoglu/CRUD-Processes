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

        // Servis yapýlandýrmasý yapýlýr, servis baðýmlýlýklarýmýzý ekleriz. Dependecy Injection yapýlan yerdir.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<IProductDal, EfProductDal>();

            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICategoryDal, EfCategoryDal>();

            // INFO: Sepet, session'da tutulmaz (örnek olarak yapýldý)
            services.AddSingleton<ICartSessionService, CartSessionService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         
            // .net core identity için
            services.AddDbContext<CustomIdentityDbContext>(options => 
            options.UseSqlServer(@"Data Source=.;Initial Catalog=Northwind;Integrated Security=True"));
            services.AddIdentity<CustomIdentityUser, CustomIdentityRole>()
                .AddEntityFrameworkStores<CustomIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddSession(); // session'ý etkinleþtirmek için
            services.AddDistributedMemoryCache(); // session'ý uygulama ayaða kalktýðýnda bellekte tutmasý için

            services.AddMvc();
        }

        // Middleware(orta katman) yapýlandýrmasý gerçekleþtirilen yerdir
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            /*
             Add new item -> npm configuration file -> package.json içine istenilen sürümlerde kütüphaneler yazýlýr.
             Kaydettikten sonra node_modules altýnda görülecektir.
             Daha sonra package.json kullanarak yüklediðimiz client kütüphanelerimizi uygulamada kullanabilmemiz için aþaðýdaki middleware
             wwwroot ile birleþtirir.
             */
            app.UseStaticFiles();
            app.UseNodeModules(env.ContentRootPath);

            app.UseRouting();
            app.UseCors();

            app.UseSession(); // session için

            app.UseAuthentication();
            app.UseAuthorization();
            // identity için

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}
