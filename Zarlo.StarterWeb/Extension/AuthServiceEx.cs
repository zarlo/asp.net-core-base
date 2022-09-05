using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Zarlo.StarterWeb.Model;
using Zarlo.StarterWeb.Settings;

namespace Zarlo.StarterWeb.Extension;

public static class AuthServiceEx {
    public static void AddAuthServiceExtension(this IServiceCollection services, IConfiguration configuration) {

        var bindJwtSettings = new JwtSettings();
        configuration.Bind("jwt", bindJwtSettings);

        services.AddIdentity<User, UserRole>(options =>
        {

            options.Lockout.AllowedForNewUsers = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.User.AllowedUserNameCharacters += @"\*";

        })
        .AddEntityFrameworkStores<MainDbContext>()
        .AddDefaultTokenProviders();

        services.AddSingleton(bindJwtSettings);
        
        services.AddAuthentication(options => {
            options.DefaultScheme = "JWT_OR_COOKIE";
            options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        })
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/login";
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer("Bearer", options => {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters() {
                ValidateIssuerSigningKey = bindJwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(bindJwtSettings.IssuerSigningKey)),
                ValidateIssuer = bindJwtSettings.ValidateIssuer,
                ValidIssuer = bindJwtSettings.ValidIssuer,
                ValidateAudience = bindJwtSettings.ValidateAudience,
                ValidAudience = bindJwtSettings.ValidAudience,
                RequireExpirationTime = bindJwtSettings.RequireExpirationTime,
                ValidateLifetime = bindJwtSettings.RequireExpirationTime,
                ClockSkew = TimeSpan.FromHours(3),
            };
        })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        {
            // runs on each request
            options.ForwardDefaultSelector = context =>
            {
                // filter by auth type
                string authorization = context.Request.Headers[HeaderNames.Authorization];
                if ((
                        !string.IsNullOrEmpty(authorization) && 
                        authorization.StartsWith("Bearer "))
                    )
                    return "Bearer";

                // otherwise always check for cookie auth
                return "Cookies";
            };
        });

    }
}