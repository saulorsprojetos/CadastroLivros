using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using CadastroLivros.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroLivros.Web.Controllers
{
    public class AutoresController : Controller
    {
        private readonly AppDbContext _db;
        public AutoresController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int pagina = 1, string? q = null)
        {
            const int tamanhoPagina = 6;
            if (pagina < 1) pagina = 1;

            var query = _db.Autores.AsNoTracking();

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

            var vm = new PaginacaoVm<CadastroLivros.Domain.Entities.Autor>
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
        public async Task<IActionResult> Create(Autor model)
        {
            if (!ModelState.IsValid) return View(model);

            var nome = model.Nome?.Trim();

            var AutorExiste = await _db.Autores
                .AsNoTracking()
                .AnyAsync(x => x.Nome == nome);

            if (AutorExiste)
            {
                ModelState.AddModelError(nameof(model.Nome), "Não foi possível salvar o(a) Autor(a). Nome já existente!");
                return View(model);
            }

            model.Nome = nome!;

            _db.Autores.Add(model);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Autor(a) cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Autores.FindAsync(id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Autor model)
        {
            if (id != model.CodAu) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var nome = model.Nome?.Trim();

            var AutorExiste = await _db.Autores
                .AsNoTracking()
                .AnyAsync(x => x.Nome == nome && x.CodAu != id);

            if (AutorExiste)
            {
                ModelState.AddModelError(nameof(model.Nome), "Não foi possível alterar o(a) Autor(a). Nome já existente!");
                return View(model);
            }

            model.Nome = nome!;

            _db.Entry(model).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Nome do(a) Autor(a) alterado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Autores.AnyAsync(x => x.CodAu == id)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Autores.AsNoTracking().FirstOrDefaultAsync(x => x.CodAu == id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.Autores.FindAsync(id);
            if (item is null) return NotFound();

            _db.Autores.Remove(item);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Autor(a) removido com sucesso!";
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível excluir. Autor(a) vinculado ao livro!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
