using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShopLego.Application;
using ShopLego.Infrastructure.Persistence;
using ShopLego.Infrastructure.Services;

namespace ShopLego.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connection = config.GetConnectionString("MyDB") ?? throw new InvalidOperationException("ConnectionStrings:MyDB is required.");
        var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
        services.AddDbContext<ShopLegoDbContext>(o => o.UseNpgsql(connection));
        services.AddScoped<IProductService, ProductService>(); services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>(); services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ISettingsService, SettingsService>(); services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IOrderNotifier, LogOrderNotifier>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        });
        services.AddAuthorization();
        return services;
    }
}
