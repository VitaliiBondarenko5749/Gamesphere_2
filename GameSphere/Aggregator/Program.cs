using Polly.Extensions.Http;
using Polly;
using Aggregator.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

#pragma warning disable

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    => HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, retryCount, context) => { });

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    => HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT.

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ValidateIssuerSigningKey = true
    };
});

// Add http clients to the dependency injection to communicate with APIs

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client => 
client.BaseAddress = new(builder.Configuration["APISettings:AuthenticationUrl"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IEmailService, EmailService>(client =>
client.BaseAddress = new(builder.Configuration["APISettings:EmailUrl"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<ICloudStorageService, CloudStorageService>(client =>
client.BaseAddress = new(builder.Configuration["APISettings:CloudStorageUrl"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IGameService, GameService>(client =>
client.BaseAddress = new(builder.Configuration["APISettings:CatalogOfGamesUrl"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IForumService, ForumService>(client =>
client.BaseAddress = new(builder.Configuration["APISettings:ForumUrl"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["APISettings:AuthenticationUrl"]), "JWT-Authentication.Microservice", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["APISettings:EmailUrl"]), "EmailService.API.Microservice", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["APISettings:CloudStorageUrl"]), "DropboxCloudStorage.API.Microservice", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["APISettings:CatalogOfGamesUrl"]), "CatalogOfGames.API.Microservice", HealthStatus.Degraded)
    .AddUrlGroup(new Uri(builder.Configuration["APISettings:ForumUrl"]), "Forum.API.Microservice", HealthStatus.Degraded);

// Add services in the dependency injection.
builder.Services.AddScoped<IFileService, FileService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();