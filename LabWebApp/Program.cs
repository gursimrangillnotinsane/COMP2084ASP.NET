using LabWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23)))); // Specify the version of your MySQL server

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
