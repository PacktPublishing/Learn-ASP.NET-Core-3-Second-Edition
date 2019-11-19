using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.Web.HAL.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicTacToe.Extensions;
using TicTacToe.Filters;
using TicTacToe.Models;
using TicTacToe.Options;
using TicTacToe.Services;
using TicTacToe.ViewEngines;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;

namespace TicTacToe
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
            services.AddLocalization(options => options.ResourcesPath =
            "Localization");
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });


            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddRazorPages();
            services.Configure<EmailServiceOptions> (Configuration.GetSection("Email"));
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IGameInvitationService,GameInvitationService>();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Learning ASP.Net Core 3.0 Rest-API",
                    Version = "v1",
                    Description = "Demonstrating auto-generated API documentation",
                    Contact = new OpenApiContact
                    {
                        Name = "Kenneth Fukizi",
                        Email = "example@example.com",
                        //Url = new Uri("https://twitter.com/afrikan_coder"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //options.IncludeXmlComments(xmlPath);
            });
            services.AddHttpContextAccessor();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IEmailTemplateRenderService, EmailTemplateRenderService>();
            services.AddTransient<IEmailViewEngine, EmailViewEngine>();
            services.AddRouting();
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(30); 
            });
            //services.AddMemoryCache();
            //services.AddSession();
            services.AddSingleton<IGameSessionService, GameSessionService>();
            var connectionString =  _configuration.GetConnectionString("DefaultConnection");
            services.AddEntityFrameworkSqlServer()
              .AddDbContext<GameDbContext>((serviceProvider, options) =>
              options.UseSqlServer(connectionString).UseInternalServiceProvider(serviceProvider)
            );

            var dbContextOptionsbuilder =
              new DbContextOptionsBuilder<GameDbContext>()
              .UseSqlServer(connectionString);
            services.AddSingleton(dbContextOptionsbuilder.Options);
            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(DetectMobileFilter));
                
            }).AddViewLocalization( LanguageViewLocationExpanderFormat.Suffix,
             options => options.ResourcesPath = "Localization").AddDataAnnotationsLocalization();

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

            app.UseSession();

            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapGet("CreateUser", context =>
            {
                var firstName = context.Request.Query["firstName"];
                var lastName = context.Request.Query["lastName"];
                var email = context.Request.Query["email"];
                var password = context.Request.Query["password"];
                var userService = context.RequestServices.GetService<IUserService>();
                userService.RegisterUser(new UserModel { FirstName = firstName, LastName = lastName, Email = email, Password = password });
                return context.Response.WriteAsync($"User {firstName} {lastName} has been sucessfully created.");
            });

            var newUserRoutes = routeBuilder.Build();
            app.UseRouter(newUserRoutes);
            
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets();

            app.UseCommunicationMiddleware();

            var supportedCultures =
          CultureInfo.GetCultures(CultureTypes.AllCultures);
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new
             CultureProviderResolverService());

            app.UseRequestLocalization(localizationOptions);
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LEARNING ASP.CORE 3.0 V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "Account",
                    pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );                 
            });
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //      name: "areas",
            //      template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //    );
            //});

            app.UseStatusCodePages("text/plain", "HTTP Error - Status Code: {0}");            
        }
    }
}
