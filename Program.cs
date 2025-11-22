using CSS.Data;
using CSS.Models;
using CSS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// DATABASE CONNECTION
// ========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ========================================
// IDENTITY CONFIGURATION
// ========================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ========================================
// SESSION (IMPORTANT FOR OTP SYSTEM)
// ========================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // OTP valid for 10 min
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========================================
// COOKIE SETTINGS
// ========================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ========================================
// EMAIL SETUP (IEmailSender)
// ========================================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// ========================================
// EXTERNAL AUTHENTICATION (Google + Facebook)
// ========================================
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        options.Scope.Add("email");
        options.Fields.Add("email");
        options.SaveTokens = true;
    });

// ========================================
// MVC
// ========================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========================================
// MIDDLEWARE PIPELINE
// ========================================
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// MUST BEFORE Authentication/Authorization
app.UseSession();            // <-- OTP will NOT work if missing

app.UseAuthentication();
app.UseAuthorization();

// ========================================
// ROUTING
// ========================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
