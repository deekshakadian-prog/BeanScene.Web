using BeanScene.Web.Data;
using BeanScene.Web.Models;
using BeanScene.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------- CONNECTION STRING ----------------
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ---------------- DB CONTEXTS ----------------------
// Domain context (Reservation, RestaurantTable, Area, SittingSchedule, etc.)
builder.Services.AddDbContext<BeanSceneContext>(opts =>
    opts.UseSqlServer(cs));

// Identity context (AspNetUsers, AspNetRoles, etc.)
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(cs));

// ---------------- EMAIL SENDER (SMTP + GMAIL) -----
// IEmailSender is used by Identity for register/confirm/forgot password
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
// SmtpEmailSender reads settings from appsettings.json -> "Smtp" section

// ---------------- IDENTITY ------------------------
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // require email confirmation before login
        options.SignIn.RequireConfirmedAccount = true;

        // reasonable default: unique emails
        options.User.RequireUniqueEmail = true;

        // you can customise password rules here if needed
        // options.Password.RequiredLength = 6;
        // options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// ---------------- MVC / RAZOR ---------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ---------------- MIDDLEWARE PIPELINE -------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// default route: Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ---------------- SEED IDENTITY + TABLE DATA ------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Seed roles/admin user
    await SeedIdentity.EnsureSeededAsync(services);

    // Seed Areas + RestaurantTables (Balcony/Main/Outside, B1–B10, M1–M10, O1–O10)
    await BeanSceneSeeder.EnsureSeededAsync(services);
}

app.Run();
