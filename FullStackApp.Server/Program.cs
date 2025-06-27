using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FullStackApp.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using FullStackApp.Server.Models.User;
using FullStackApp.Server.Controllers;
using System.Configuration;
using Microsoft.AspNetCore.HttpOverrides;

using FullStackApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

//Identity authentication

//builder.Services.AddDbContext<ApplicationDbContext>(
//   options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddDbContext<NewIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NewIdentityDbContext") ??
    throw new InvalidOperationException("Connection string 'NewIdentityDbContext' not found."))
    );


builder.Services.AddRazorPages();

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 10;
    opt.Password.RequireDigit = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = false; // All logins require email confirmation if enabled

}).AddDefaultUI()
    .AddEntityFrameworkStores<NewIdentityDbContext>().AddDefaultTokenProviders();


// Add razor pages and cookie for Identity pages

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();


// Load keys from .env file
DotNetEnv.Env.Load();
var GOOGLE_CLIENT_ID = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
var GOOGLE_CLIENT_SECRET = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");



// Add Google authentication option to login page
builder.Services.AddAuthentication()
    .AddGoogle("google", opt =>
    {
        
        opt.ClientId = GOOGLE_CLIENT_ID;
        opt.ClientSecret = GOOGLE_CLIENT_SECRET;
        //opt.SignInScheme = IdentityConstants.ExternalScheme;
    });


builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    //options.Cookie.Expiration 

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    //options.ReturnUrlParameter=""
});

/* builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<NewIdentityDbContext>();
*/



// Add services to the container.

builder.Services.AddScoped<IUserDataService, UserDataService>();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Weather API",
        Description = "API for managing a list of fruit and their stock status.",
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//For identity
builder.Services.AddAuthorization();

var app = builder.Build();

//Identity
app.MapIdentityApi<IdentityUser>();


// For reverse proxy requests
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseDefaultFiles();
app.MapStaticAssets();
//app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();




//
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});


app.MapControllers();


app.MapFallbackToFile("/index.html");

app.Run();
