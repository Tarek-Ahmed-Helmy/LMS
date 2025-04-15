using LMS.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace LMS.Utilities;

public class RoleSeeder
{
    public static async Task SeedRolesAndAdminUserAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        var roleNames = new[]
        {
            SD.SuperAdminRole,
            SD.AdminRole,
            SD.TeacherRole,
            SD.StudentRole,
            SD.ParentRole
        };

        var existingRoles = roleManager.Roles.Select(r => r.Name).ToHashSet();

        foreach (var roleName in roleNames)
        {
            if (!existingRoles.Contains(roleName)) await roleManager.CreateAsync(new IdentityRole(roleName));
        }


        var superAdminRole = await roleManager.FindByNameAsync(SD.SuperAdminRole);
        if (superAdminRole != null)
        {
            var existingClaims = await roleManager.GetClaimsAsync(superAdminRole);
            var existingClaimTypes = existingClaims.Select(c => c.Type).ToHashSet();

            foreach (var policy in ClaimsStore.GetAllPolicies())
            {
                if (!existingClaimTypes.Contains(policy))
                {
                    await roleManager.AddClaimAsync(superAdminRole, new Claim(policy, policy));
                }
            }
        }


        var superAdminSettings = configuration.GetSection("SuperAdminUser").Get<SuperAdminUserSettings>();

        if (superAdminSettings == null)
        {
            Console.WriteLine("SuperAdminUser settings are missing in appsettings.json!");
            return;
        }

        var superAdminUser = await userManager.FindByEmailAsync(superAdminSettings.Email);
        if (superAdminUser == null)
        {
            superAdminUser = new ApplicationUser
            {
                UserName = superAdminSettings.UserName,
                Email = superAdminSettings.Email,
                EmailConfirmed = true,
                FullName = superAdminSettings.FullName,
                Address = superAdminSettings.Address
            };

            var createResult = await userManager.CreateAsync(superAdminUser, superAdminSettings.Password);
            if (createResult.Succeeded)
            {
                Console.WriteLine($"Created SuperAdmin user: {superAdminSettings.Email}");
                await userManager.AddToRoleAsync(superAdminUser, SD.SuperAdminRole);
            }
            else
            {
                Console.WriteLine("Failed to create SuperAdmin user:");
                foreach (var error in createResult.Errors)
                {
                    Console.WriteLine($" - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"SuperAdmin user '{superAdminSettings.Email}' already exists.");
        }
    }
}
