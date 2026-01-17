using CadastroLivros.Domain.Entities;
using CadastroLivros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Tests
{
    public class InsercaoRelacionamentosTests
    {
        private static AppDbContext CreateDb(string name)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Salvar_livro_com_autor_assunto_e_preco()
        {
            using var db = CreateDb("db_rel_1");

            var autor = new Autor { Nome = "Autor 1" };
            var assunto = new Assunto { Descricao = "Assunto 1" };
            var forma = new FormaCompra { Nome = "Internet" };

            db.Autores.Add(autor);
            db.Assuntos.Add(assunto);
            db.FormasCompra.Add(forma);
            await db.SaveChangesAsync();

            var livro = new Livro
            {
                Titulo = "Livro 1",
                Editora = "Editora",
                Edicao = 1,
                AnoPublicacao = "2024",
                Autores = new List<LivroAutor> { new LivroAutor { Autor_CodAu = autor.CodAu } },
                Assuntos = new List<LivroAssunto> { new LivroAssunto { Assunto_codAs = assunto.codAs } },
                Precos = new List<LivroPreco> { new LivroPreco { FormaCompra_Id = forma.Id, Valor = 10.50m } }
            };

            db.Livros.Add(livro);
            await db.SaveChangesAsync();

            var salvo = await db.Livros
                .Include(l => l.Autores)
                .Include(l => l.Assuntos)
                .Include(l => l.Precos)
                .FirstAsync();

            Assert.Single(salvo.Autores);
            Assert.Single(salvo.Assuntos);
            Assert.Single(salvo.Precos);
        }
    }
}
