using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using CadastroLivros.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroLivros.Web.Controllers
{
    public class LivrosController : Controller
    {
        private readonly AppDbContext _db;
        public LivrosController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int pagina = 1, string? q = null)
        {
            const int tamanhoPagina = 6;
            if (pagina < 1) pagina = 1;

            var query = _db.Livros.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(l =>
                    l.Titulo.Contains(q) ||
                    l.Autores.Any(a => a.Autor.Nome.Contains(q)) ||
                    l.Assuntos.Any(s => s.Assunto.Descricao.Contains(q))
                );
            }

            var contagemTotal = await query.CountAsync();

            var totalPaginas = (int)Math.Ceiling(contagemTotal / (double)tamanhoPagina);
            if (totalPaginas == 0) totalPaginas = 1;
            if (pagina > totalPaginas) pagina = totalPaginas;

            var items = await query
                .OrderBy(l => l.Titulo)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)

                .Include(l => l.Autores).ThenInclude(la => la.Autor)
                .Include(l => l.Assuntos).ThenInclude(la => la.Assunto)                

                .ToListAsync();

            var vm = new PaginacaoVm<CadastroLivros.Domain.Entities.Livro>
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


        public async Task<IActionResult> Details(int id)
        {
            var livro = await _db.Livros
                .AsNoTracking()
                .Include(l => l.Autores).ThenInclude(la => la.Autor)
                .Include(l => l.Assuntos).ThenInclude(la => la.Assunto)
                .Include(l => l.Precos).ThenInclude(p => p.FormaCompra)
                .FirstOrDefaultAsync(l => l.Codl == id);

            return livro is null ? NotFound() : View(livro);
        }

        public async Task<IActionResult> Create()
        {
            var vm = await BuildFormVmForCreate();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LivroVm vm)
        {
            await LoadLists(vm);

            if (!ModelState.IsValid) return View(vm);

            if (!vm.AutoresSelecionados.Any())
                ModelState.AddModelError(nameof(vm.AutoresSelecionados), "Selecione ao menos 1 autor!");

            if (!vm.AssuntosSelecionados.Any())
                ModelState.AddModelError(nameof(vm.AssuntosSelecionados), "Selecione ao menos 1 assunto!");

            if (!ModelState.IsValid) return View(vm);

            var livro = new Livro
            {
                Titulo = vm.Titulo.Trim(),
                Editora = vm.Editora.Trim(),
                Edicao = vm.Edicao,
                AnoPublicacao = vm.AnoPublicacao.Trim()
            };

            livro.Autores = vm.AutoresSelecionados.Distinct()
                .Select(id => new LivroAutor { Autor_CodAu = id })
                .ToList();

            livro.Assuntos = vm.AssuntosSelecionados.Distinct()
                .Select(id => new LivroAssunto { Assunto_codAs = id })
                .ToList();

            livro.Precos = vm.Precos
                .Where(p => p.Valor.HasValue)
                .Select(p => new LivroPreco
                {
                    FormaCompra_Id = p.FormaCompraId,
                    Valor = p.Valor!.Value
                })
                .ToList();

            _db.Livros.Add(livro);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Livro cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível salvar o livro. Verifique as informações!";
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var livro = await _db.Livros
                .Include(l => l.Autores)
                .Include(l => l.Assuntos)
                .Include(l => l.Precos)
                .FirstOrDefaultAsync(l => l.Codl == id);

            if (livro is null) return NotFound();

            var vm = await BuildFormVmBase();
            vm.Codl = livro.Codl;
            vm.Titulo = livro.Titulo;
            vm.Editora = livro.Editora;
            vm.Edicao = livro.Edicao;
            vm.AnoPublicacao = livro.AnoPublicacao;

            vm.AutoresSelecionados = livro.Autores.Select(x => x.Autor_CodAu).ToList();
            vm.AssuntosSelecionados = livro.Assuntos.Select(x => x.Assunto_codAs).ToList();
            
            vm.Precos = vm.FormasCompra.Select(f =>
            {
                var existente = livro.Precos.FirstOrDefault(p => p.FormaCompra_Id == f.Id);
                return new PrecoItemVm
                {
                    FormaCompraId = f.Id,
                    FormaCompraNome = f.Texto,
                    Valor = existente?.Valor
                };
            }).ToList();

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LivroVm vm)
        {
            await LoadLists(vm);

            if (vm.Codl != id) return BadRequest();

            if (!ModelState.IsValid) return View(vm);

            if (!vm.AutoresSelecionados.Any())
                ModelState.AddModelError(nameof(vm.AutoresSelecionados), "Selecione ao menos 1 autor!");

            if (!vm.AssuntosSelecionados.Any())
                ModelState.AddModelError(nameof(vm.AssuntosSelecionados), "Selecione ao menos 1 assunto!");

            if (!ModelState.IsValid) return View(vm);

            var livro = await _db.Livros
                .Include(l => l.Autores)
                .Include(l => l.Assuntos)
                .Include(l => l.Precos)
                .FirstOrDefaultAsync(l => l.Codl == id);

            if (livro is null) return NotFound();

            livro.Titulo = vm.Titulo.Trim();
            livro.Editora = vm.Editora.Trim();
            livro.Edicao = vm.Edicao;
            livro.AnoPublicacao = vm.AnoPublicacao.Trim();
            
            livro.Autores.Clear();
            foreach (var autorId in vm.AutoresSelecionados.Distinct())
                livro.Autores.Add(new LivroAutor { Livro_Codl = livro.Codl, Autor_CodAu = autorId });

            livro.Assuntos.Clear();
            foreach (var assuntoId in vm.AssuntosSelecionados.Distinct())
                livro.Assuntos.Add(new LivroAssunto { Livro_Codl = livro.Codl, Assunto_codAs = assuntoId });

            livro.Precos.Clear();
            foreach (var p in vm.Precos.Where(x => x.Valor.HasValue))
            {
                livro.Precos.Add(new LivroPreco
                {
                    Livro_Codl = livro.Codl,
                    FormaCompra_Id = p.FormaCompraId,
                    Valor = p.Valor!.Value
                });
            }

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Livro alterado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível salvar as alterações. Verifique as informações!";
                return View(vm);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var livro = await _db.Livros.AsNoTracking().FirstOrDefaultAsync(l => l.Codl == id);
            return livro is null ? NotFound() : View(livro);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livro = await _db.Livros
                .Include(l => l.Autores)
                .Include(l => l.Assuntos)
                .Include(l => l.Precos)
                .FirstOrDefaultAsync(l => l.Codl == id);

            if (livro is null) return NotFound();

            if (livro.Autores.Any())
                _db.LivroAutores.RemoveRange(livro.Autores);

            if (livro.Assuntos.Any())
                _db.LivroAssuntos.RemoveRange(livro.Assuntos);

            if (livro.Precos.Any())
                _db.LivroPrecos.RemoveRange(livro.Precos);

            _db.Livros.Remove(livro);

            try
            {
                await _db.SaveChangesAsync();
                TempData["msg"] = "Livro removido com sucesso!";
            }
            catch (DbUpdateException)
            {
                TempData["erro"] = "Não foi possível excluir o livro!";
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<LivroVm> BuildFormVmBase()
        {
            var vm = new LivroVm();
            await LoadLists(vm);
            return vm;
        }

        private async Task<LivroVm> BuildFormVmForCreate()
        {
            var vm = await BuildFormVmBase();

            vm.Precos = vm.FormasCompra.Select(f => new PrecoItemVm
            {
                FormaCompraId = f.Id,
                FormaCompraNome = f.Texto,
                Valor = null
            }).ToList();

            return vm;
        }

        private async Task LoadLists(LivroVm vm)
        {
            vm.Autores = await _db.Autores.AsNoTracking()
                .OrderBy(a => a.Nome)
                .Select(a => new ItemVm { Id = a.CodAu, Texto = a.Nome })
                .ToListAsync();

            vm.Assuntos = await _db.Assuntos.AsNoTracking()
                .OrderBy(a => a.Descricao)
                .Select(a => new ItemVm { Id = a.codAs, Texto = a.Descricao })
                .ToListAsync();

            vm.FormasCompra = await _db.FormasCompra.AsNoTracking()
                .OrderBy(f => f.Nome)
                .Select(f => new ItemVm { Id = f.Id, Texto = f.Nome })
                .ToListAsync();
        }
    }
}
