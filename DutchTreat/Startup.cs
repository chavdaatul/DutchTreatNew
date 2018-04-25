using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace DutchTreat
{
  public class Startup
  {
    private readonly IConfiguration _config;
    private readonly IHostingEnvironment _env;

    public Startup(IConfiguration config, IHostingEnvironment env)
    {
      this._config = config;
      this._env = env;
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      //Identity 
      services.AddIdentity<StoreUser, IdentityRole>(cfg =>
       {
         cfg.User.RequireUniqueEmail = true;
       }).AddEntityFrameworkStores<DutchContext>();

      

      services.AddDbContext<DutchContext>(cfg =>
      {
        cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
      });
      services.AddTransient<IMailService, NullMailService>();
      //Support for real mail service
      services.AddScoped<IDutchRepository, DutchRepository>();
      services.AddTransient<DutchSeeder>();

      services.AddMvc(opt => {
        if (_env.IsProduction()) {
          opt.Filters.Add(new RequireHttpsAttribute());
        }
      })
        .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.AddAuthentication()
        .AddCookie()
        .AddJwtBearer(cfg => 
        cfg.TokenValidationParameters = new TokenValidationParameters()
        {
          ValidIssuer = _config["Tokens:Issuer"],
          ValidAudience = _config["Tokens:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))

    });

      services.AddAutoMapper();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      //If you use app.UserDefaultFiles() will directly run defaul index file from wroot no need to describ in Url
      // app.UseDefaultFiles();
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/error");
      }

      if (env.IsDevelopment())
      {
        using (var scope = app.ApplicationServices.CreateScope())
        {
          var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
          seeder.Seed().Wait();
        }
      }
      app.UseStaticFiles();
      app.UseAuthentication();
      //You able to run any static html file from Wroot directry


      app.UseMvc(cfg =>
      {
        cfg.MapRoute("Default", "{controller}/{action}/{id?}",
          new { controller = "App", Action = "Index" });
      });
    }
  }
}
