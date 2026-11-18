using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;

namespace OficinaEF.Controllers
{
    [Authorize(Roles = "Admin,Atendente")]
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Categoria> categorias = await _context.Categorias
                .AsNoTracking()
                .Include(c => c.Produtos)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return View(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Categoria? categoria = await _context.Categorias
                .AsNoTracking()
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao")] Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Categoria cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Categoria? categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Nome,Descricao")] Categoria categoria, int id)
        {
            if (id != categoria.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categorias.AnyAsync(c => c.Id == id))
                    return NotFound();
                throw;
            }

            TempData["Sucesso"] = "Categoria editada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Categoria? categoria = await _context.Categorias
                .AsNoTracking()
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Categoria? categoria = await _context.Categorias.FindAsync(id);
            if (categoria is null)
                return NotFound();

            // Não permite apagar categoria que ainda tem produtos
            if (await _context.Produtos.AnyAsync(p => p.CategoriaId == id))
            {
                TempData["Erro"] = "Não é possível excluir: existem produtos nesta categoria.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Categoria deletada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
