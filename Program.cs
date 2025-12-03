using CSS.Data;
using CSS.Models;
using CSS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// DATABASE
// =====================================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================================================================
// IDENTITY (Relaxed Password Rules)
// =====================================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// =====================================================================
// SESSION
// =====================================================================
builder.Services.AddSession();

// =====================================================================
// EMAIL SERVICE
// =====================================================================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// =====================================================================
// WEB PUSH NOTIFICATION (VAPID SETTINGS)
// =====================================================================
builder.Services.Configure<VapidSettings>(builder.Configuration.GetSection("Vapid"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<VapidSettings>>().Value);

builder.Services.AddScoped<INotificationService, WebPushNotificationService>();

// =====================================================================
// PAYMENT SERVICE (AamarPay)
// =====================================================================
builder.Services.Configure<AamarPaySettings>(builder.Configuration.GetSection("AamarPay"));
builder.Services.AddHttpClient();
builder.Services.AddTransient<AamarPayService>();


// =====================================================================
// AUTHENTICATION - OAUTH LOGIN
// =====================================================================
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
    });

// =====================================================================
// MVC SUPPORT
// =====================================================================
builder.Services.AddControllersWithViews();

// =====================================================================
// CORS SETTINGS
// =====================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// =====================================================================
// MIDDLEWARE PIPELINE
// =====================================================================
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =====================================================================
// DEFAULT ROUTE
// =====================================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =====================================================================
// SEED ADMIN + ROLES
// =====================================================================
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// =====================================================================
// RUN APPLICATION
// =====================================================================
app.Run();
