using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;

namespace OficinaEF.Controllers
{
    [Authorize(Roles = "Admin,Atendente")]
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoriaId)
        {
            IQueryable<Produto> consulta = _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria);

            // Filtro opcional por categoria (link "Ver produtos" na tela de Categorias)
            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
                ViewBag.CategoriaFiltro = await _context.Categorias
                    .AsNoTracking()
                    .Where(c => c.Id == categoriaId.Value)
                    .Select(c => c.Nome)
                    .FirstOrDefaultAsync();
            }

            List<Produto> produtos = await consulta.OrderBy(p => p.Nome).ToListAsync();

            return View(produtos);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Produto? produto = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto is null)
                return NotFound();

            return View(produto);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarCategoriasAsync();
            return View(new Produto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nome,Descricao,Preco,PrecoAntigo,Estoque,Selo,ImagemUrl,Arte,CategoriaId")] Produto produto)
        {
            await ValidarCategoriaAsync(produto);

            if (!ModelState.IsValid)
            {
                await CarregarCategoriasAsync(produto.CategoriaId);
                return View(produto);
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Produto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Produto? produto = await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto is null)
                return NotFound();

            await CarregarCategoriasAsync(produto.CategoriaId);
            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Nome,Descricao,Preco,PrecoAntigo,Estoque,Selo,ImagemUrl,Arte,CategoriaId")] Produto produto, int id)
        {
            if (id != produto.Id)
                return BadRequest();

            await ValidarCategoriaAsync(produto);

            if (!ModelState.IsValid)
            {
                await CarregarCategoriasAsync(produto.CategoriaId);
                return View(produto);
            }

            try
            {
                _context.Update(produto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Produtos.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }

            TempData["Sucesso"] = "Produto editado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Produto? produto = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto is null)
                return NotFound();

            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Produto? produto = await _context.Produtos.FindAsync(id);
            if (produto is null)
                return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Produto deletado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ----- auxiliares -----

        // Preenche o <select> de categorias (ViewBag.Categorias)
        private async Task CarregarCategoriasAsync(int? selecionada = null)
        {
            List<Categoria> categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();

            ViewBag.Categorias = new SelectList(categorias, nameof(Categoria.Id), nameof(Categoria.Nome), selecionada);
        }

        // Garante que a categoria escolhida existe (evita erro de chave estrangeira)
        private async Task ValidarCategoriaAsync(Produto produto)
        {
            if (produto.CategoriaId > 0 &&
                !await _context.Categorias.AnyAsync(c => c.Id == produto.CategoriaId))
            {
                ModelState.AddModelError(nameof(Produto.CategoriaId), "Categoria inválida.");
            }
        }
    }
}
