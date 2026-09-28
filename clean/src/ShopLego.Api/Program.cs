using Microsoft.OpenApi.Models;
using ShopLego.Api;
using ShopLego.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); builder.Services.AddProblemDetails(); builder.Services.AddHealthChecks();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopLego API", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = [] });
});
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>(); app.UseSwagger(); app.UseSwaggerUI(); app.UseCors();
app.UseAuthentication(); app.UseAuthorization(); app.MapHealthChecks("/health"); app.MapControllers(); app.Run();
public partial class Program;
