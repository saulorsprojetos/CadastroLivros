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
    public class LivroTests
    {
        [Fact]
        public async Task Deve_criar_livro_com_campos_obrigatorios()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("db_test_1")
                .Options;

            using var db = new AppDbContext(options);

            var livro = new Livro
            {
                Titulo = "Teste",
                Editora = "Editora",
                Edicao = 1,
                AnoPublicacao = "2020"
            };

            db.Livros.Add(livro);
            await db.SaveChangesAsync();

            Assert.Equal(1, await db.Livros.CountAsync());
        }
    }
}
