using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using CadastroLivros.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroLivros.Web.Controllers
{
    public class AssuntosController : Controller
    {
        private readonly AppDbContext _db;
        public AssuntosController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int pagina = 1, string? q = null)
        {
            const int tamanhoPagina = 6;
            if (pagina < 1) pagina = 1;

            var query = _db.Assuntos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.Descricao.Contains(q));
            }

            var contagemTotal = await query.CountAsync();

            var totalPaginas = (int)Math.Ceiling(contagemTotal / (double)tamanhoPagina);
            if (totalPaginas == 0) totalPaginas = 1;
            if (pagina > totalPaginas) pagina = totalPaginas;

            var items = await query
                .OrderBy(x => x.Descricao)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            var vm = new PaginacaoVm<CadastroLivros.Domain.Entities.Assunto>
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
        public async Task<IActionResult> Create(Assunto model)
        {
            if (!ModelState.IsValid) return View(model);

            var descricao = model.Descricao?.Trim();

            var AssuntoExiste = await _db.Assuntos
                .AsNoTracking()
                .AnyAsync(x => x.Descricao == descricao);

            if (AssuntoExiste)
            {
                ModelState.AddModelError(nameof(model.Descricao), "Não foi possível salvar o Assunto. Nome já existente!");
                return View(model);
            }

            model.Descricao = descricao!;


            _db.Assuntos.Add(model);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Assunto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Assuntos.FindAsync(id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assunto model)
        {
            if (id != model.codAs) return BadRequest();
            if (!ModelState.IsValid) return View(model);


            var descricao = model.Descricao?.Trim();

            var AssuntoExiste = await _db.Assuntos
                .AsNoTracking()
                .AnyAsync(x => x.Descricao == descricao && x.codAs != id);

            if (AssuntoExiste)
            {
                ModelState.AddModelError(nameof(model.Descricao), "Não foi possível alterar o Assunto. Nome já existente!");
                return View(model);
            }

            model.Descricao = descricao!;


            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            TempData["msg"] = "Assunto alterado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Assuntos.AsNoTracking().FirstOrDefaultAsync(x => x.codAs == id);
            return item is null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.Assuntos.FindAsync(id);
            if (item is null) return NotFound();

            _db.Assuntos.Remove(item);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Assunto removido com sucesso!";
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível excluir. Assunto vinculado ao livro!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
