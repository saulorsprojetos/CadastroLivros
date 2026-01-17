using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using CadastroLivros.Web.ViewModels;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace CadastroLivros.Web.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public RelatoriosController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new RelatorioFiltroVm
            {
                Autores = await _db.Autores.AsNoTracking()
                    .OrderBy(a => a.Nome)
                    .Select(a => new ItemVm { Id = a.CodAu, Texto = a.Nome })
                    .ToListAsync()
            };

            return View(vm);
        }
        public async Task<IActionResult> LivrosPorAutor(int? autorId)
        {
            var query = _db.VwRelatorioLivrosPorAutor.AsNoTracking();

            if (autorId.HasValue)
                query = query.Where(x => x.CodAu == autorId.Value);

            var dados = await query
                .OrderBy(x => x.AutorNome)
                .ThenBy(x => x.Titulo)
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Text("Relatório de Livros por Autor")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        string? autorAtual = null;

                        foreach (var item in dados)
                        {
                            if (autorAtual != item.AutorNome)
                            {
                                autorAtual = item.AutorNome;

                                col.Item().PaddingTop(15).Text($"Autor: {autorAtual}")
                                    .FontSize(14)
                                    .Bold();
                            }

                            col.Item().Border(1).Padding(5).Column(livro =>
                            {
                                livro.Item().Text($"Título: {item.Titulo}");
                                livro.Item().Text($"Editora: {item.Editora}");
                                livro.Item().Text($"Edição: {item.Edicao} | Ano: {item.AnoPublicacao}");
                                livro.Item().Text($"Assuntos: {item.Assuntos}");
                                livro.Item().Text($"Preços: {item.Precos}");
                            });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            var bytes = pdf.GeneratePdf();

            return File(
                bytes,
                "application/pdf",
                "Relatorio_Livros_por_Autor.pdf"
            );
        }
    }
}
