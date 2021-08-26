using CalendarDisplay.Data;
using CalendarDisplay.Options;
using CalendarDisplay.ViewModels;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace CalendarDisplay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            // Add sign-in with Microsoft
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                    // Add the possibility of acquiring a token to call a protected web API
                    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                        // Enables controllers and pages to get GraphServiceClient by dependency injection
                        // And use an in memory token cache
                        .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
                        .AddInMemoryTokenCaches();

            services.AddControllersWithViews(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddMicrosoftIdentityUI();

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });

            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            services.AddTransient<ICalendarManager, CalendarManager>();
            services.AddTransient<ITodoManager, TodoManager>();

            services.AddScoped<ICalendarViewModel, CalendarViewModel>();
            services.AddScoped<ITodoViewModel, TodoViewModel>();
            services.AddScoped<ICountdownViewModel, CountdownViewModel>();

            services.Configure<CalendarOptions>(Configuration.GetSection(CalendarOptions.Section));
            services.Configure<TodoListOptions>(Configuration.GetSection(TodoListOptions.Section));
            services.Configure<CountdownOptions>(Configuration.GetSection(CountdownOptions.Section));

            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}