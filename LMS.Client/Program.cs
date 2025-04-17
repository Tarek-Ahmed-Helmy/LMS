using LMS.DataAccess.DependencyInjection;
using LMS.Entities.Models;
using LMS.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LMS.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDataAccessServices(builder.Configuration);
        builder.Services.AddAuthorization(options =>
        {
            foreach (var controller in ClaimsStore.ControllerClaims)
            {
                foreach (var action in controller.Value)
                {
                    var policyName = $"{controller.Key}.{action}";
                    options.AddPolicy(policyName, policy => policy.RequireClaim(policyName));
                }
            }
        });
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Shared/Account/Login";
            options.AccessDeniedPath = "/Shared/Account/AccessDenied";
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            await RoleSeeder.SeedRolesAndAdminUserAsync(roleManager, userManager, configuration);
        }


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Shared}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "Admin",
            pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "Teacher",
            pattern: "{area=Teacher}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "Student",
            pattern: "{area=Student}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "Parent",
            pattern: "{area=Parent}/{controller=Home}/{action=Index}/{id?}");

        


        app.Run();
    }
}
