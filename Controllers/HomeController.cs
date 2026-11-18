using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;

namespace OficinaEF.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        HomeViewModel modelo = new()
        {
            Categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync(),

            // Vitrine: só produtos com estoque, os mais recentes primeiro
            Produtos = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Where(p => p.Estoque > 0)
                .OrderByDescending(p => p.Id)
                .Take(12)
                .ToListAsync()
        };

        return View(modelo);
    }

    [Authorize(Roles = "Atendente")]
    public IActionResult Atendente()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Roles = "Cliente")]
    public IActionResult Cliente()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}