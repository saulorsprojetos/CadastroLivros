using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using CadastroLivros.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroLivros.Web.Controllers
{
    public class FormasCompraController : Controller
    {
        private readonly AppDbContext _db;
        public FormasCompraController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int pagina = 1, string? q = null)
        {
            const int tamanhoPagina = 6;
            if (pagina < 1) pagina = 1;

            var query = _db.FormasCompra.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.Nome.Contains(q));
            }

            var contagemTotal = await query.CountAsync();

            var totalPaginas = (int)Math.Ceiling(contagemTotal / (double)tamanhoPagina);
            if (totalPaginas == 0) totalPaginas = 1;
            if (pagina > totalPaginas) pagina = totalPaginas;

            var items = await query
                .OrderBy(x => x.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            var vm = new PaginacaoVm<CadastroLivros.Domain.Entities.FormaCompra>
            {
                Items = items,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                ContagemTotal = contagemTotal,
                TotalPaginas = totalPaginas,
                Query = q
            };

            return View(vm);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormaCompra model)
        {
            if (!ModelState.IsValid) return View(model);

            _db.FormasCompra.Add(model);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Forma de compra cadastrada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Nome), "Não foi possível salvar a Forma de Compra. Nome já existente!");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.FormasCompra.FindAsync(id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormaCompra model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _db.Entry(model).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Forma de compra alterada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Nome), "Não foi possível alterar a Forma de Compra. Nome já existente!");
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.FormasCompra.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.FormasCompra.FindAsync(id);
            if (item is null) return NotFound();

            _db.FormasCompra.Remove(item);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Forma de compra removida!";
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível excluir a Forma de Compra. Pode estar em uso em preços de livros!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
