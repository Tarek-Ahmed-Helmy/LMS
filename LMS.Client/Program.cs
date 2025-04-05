using LMS.DataAccess.DependencyInjection;

namespace LMS.Client;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDataAccessServices(builder.Configuration);

        var app = builder.Build();

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
            pattern: "{controller=Home}/{action=Index}/{id?}")
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
