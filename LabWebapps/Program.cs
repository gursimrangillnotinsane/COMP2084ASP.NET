using LabWebapps.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddControllersWithViews();

// Add a third-party authentication provider
builder.Services.AddAuthentication()
      .AddGitHub(o =>
      {
          o.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
          o.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
          o.CallbackPath = "/signin-github";
          // Grants access to read a user's profile data.
          // https://docs.github.com/en/developers/apps/building-oauth-apps/scopes-for-oauth-apps
          o.Scope.Add("read:user");
          // Optional
          // if you need an access token to call GitHub Apis
          o.Events.OnCreatingTicket += context =>
          {
              if (context.AccessToken is { })
              {
                  context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
              }
              return Task.CompletedTask;
          };
      });

// enables session states
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Authentication must be called before authorization to enable third party services
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
