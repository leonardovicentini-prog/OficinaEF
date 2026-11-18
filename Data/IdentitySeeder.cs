using Microsoft.AspNetCore.Identity;

namespace OficinaEF.Data;

// Garante que os 3 perfis existam no banco (só cria os que faltam; não mexe nos usuários)
public static class IdentitySeeder
{
    public static readonly string[] Perfis = { "Admin", "Atendente", "Cliente" };

    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (string perfil in Perfis)
        {
            if (!await roleManager.RoleExistsAsync(perfil))
                await roleManager.CreateAsync(new IdentityRole(perfil));
        }
    }
}
