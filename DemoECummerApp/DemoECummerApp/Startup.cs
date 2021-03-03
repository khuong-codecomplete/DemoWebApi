using DemoECummerApp.Security.OAuth;
using DemoECummerApp.Models;
using DemoECummerApp.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoECummerApp
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
            services.AddControllers();
            services.AddMvc();
            var connection = @"Server=DESKTOP-D1SN2MM\SQLEXPRESS;Database=DemoDatabase;Trusted_Connection=True";
            services.AddDbContext<DemoDatabaseContext>(options => options.UseSqlServer(connection));
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.WithOrigins("https://localhost:44339"));
                options.AddPolicy("mypolicy", builder =>
                    builder.WithOrigins("https://localhost:44339")
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });



            services.AddIdentityServer()
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddProfileService<ProfileService>()
                    .AddDeveloperSigningCredential();

            //services.AddAuthentication("Basic")
            //    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
            //services.AddTransient<IAuthenticationHandler, BasicAuthenticationHandler>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = "https://localhost:44383";
                o.Audience = "DemoDatabase.ReadAccess";
                o.RequireHttpsMetadata = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("mypolicy");
        }
    }
}
