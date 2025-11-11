using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Articulus.Data;
using Articulus.Filters;
using Articulus.BLL.Articles;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using Articulus.BLL.Articles.Interfaces;
using Articulus.Services;
using Articulus.BLL.Users.Interfaces;
using Articulus.BLL.Users;
using Articulus.BLL.News.Interfaces;
using Articulus.BLL.News;
using Articulus.DTOs.Env;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Needed for minimal APIs & Swagger
builder.Services.AddSwaggerGen(c =>         // Registers Swagger generator
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//---jwt DI
var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt["Key"];
if (string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}
builder.Services.AddAuthentication(option => option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                ValidateIssuer = true,
                ValidIssuer = jwt["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwt["Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<MailService>();
// BLL services
builder.Services.AddScoped<IArticleCommentVoteService, ArticleCommentVoteService>();
builder.Services.AddScoped<IArticleActionsService, ArticleActionsService>();
builder.Services.AddScoped<IArticleCommentsService, ArticleCommentsService>();
builder.Services.AddScoped<IArticleReactionService, ArticleReactionService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserActionsService, UserActionsService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<INewsService, NewsService>();

//configure serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

//add logging filter globally
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActionFilter>();
});

//add global exception filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionActionFilter>();
});

//http client 
builder.Services.AddHttpClient("NewsApiClient", client =>
{
    client.BaseAddress = new Uri("https://newsapi.org/v2/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<CustomAuthorizeFilter>();

//configure settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();      // Serves the OpenAPI JSON
    app.UseSwaggerUI();    // Serves the Swagger UI


}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
