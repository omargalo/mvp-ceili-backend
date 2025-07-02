using Azure;
using Azure.AI.Inference;
using CeiliApi.Data;
using CeiliApi.Services;
using CeiliApi.Business;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Conexión a base de datos
var connString = builder.Configuration.GetConnectionString("CeiliDb");
builder.Services.AddDbContext<CeiliDbContext>(options =>
    options.UseSqlServer(connString, sql => sql.EnableRetryOnFailure())
);

// Configuración Azure AI
var azureAiConfig = builder.Configuration.GetSection("AzureAI");
var endpoint = new Uri(azureAiConfig["Endpoint"]!);
var apiKey = azureAiConfig["ApiKey"]!;
builder.Services.AddSingleton(new ChatCompletionsClient(endpoint, new AzureKeyCredential(apiKey), new AzureAIInferenceClientOptions()));

// Servicios personalizados
builder.Services.AddScoped<ChatGptService>();
builder.Services.AddScoped<PasswordResetService>();
builder.Services.AddScoped<PasswordReset>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Autenticación JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
    };
});

builder.Services.AddControllers();

var app = builder.Build();

// Middleware personalizado para manejo de excepciones
app.UseMiddleware<CeiliApi.Middleware.ExceptionHandlingMiddleware>();

// CORS primero (antes de cualquier endpoint)
app.UseCors("AllowAll");

// Archivos estáticos y default files
app.UseDefaultFiles();
app.UseStaticFiles();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// API Controllers
app.MapControllers();

// Fallback para SPA (Angular router)
app.MapFallbackToFile("index.html");

app.Run();
