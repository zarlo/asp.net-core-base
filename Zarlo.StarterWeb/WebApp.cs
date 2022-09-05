using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Zarlo.StarterWeb.ModelBinder;

namespace Zarlo.StarterWeb;

public class WebApp
{

    private IWebHostEnvironment CurrentEnvironment { get; set; }
    public IConfiguration Configuration { get; }

    public WebApp(IWebHostEnvironment env, IConfiguration configuration)
    {
        Configuration = configuration;
        CurrentEnvironment = env;

    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
         Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();

        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddDbContext<MainDbContext>(options => {
            options.UseMySQL(Configuration.GetConnectionString("MainDb"));
        });

        services.AddRazorPages();

        services.AddSwaggerGen();

        services.AddControllersWithViews();
        services.AddMvc(options =>
        {
            options
                .ModelBinderProviders
                .Insert(0, new CustomDateTimeModelBinderProvider());
        })
        .AddJsonOptions(o => {
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });


        services.AddAntiforgery(options =>
        {
            // Set Cookie properties using CookieBuilder properties†.
            options.FormFieldName = "CSRF";
            options.HeaderName = "X-CSRF";
            options.SuppressXFrameOptionsHeader = false;
        });


        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });

        return services.BuildServiceProvider();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        ILogger<WebApp> logger)
    {

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMvcWithDefaultRoute();

    }

}