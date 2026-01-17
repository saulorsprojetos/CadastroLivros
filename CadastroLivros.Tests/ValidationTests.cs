using CadastroLivros.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void Livro_deve_rejeitar_ano_invalido()
        {
            var livro = new Livro
            {
                Titulo = "Teste",
                Editora = "Editora",
                Edicao = 1,
                AnoPublicacao = "20A0"
            };

            var ctx = new ValidationContext(livro);
            var results = new List<ValidationResult>();

            var ok = Validator.TryValidateObject(livro, ctx, results, validateAllProperties: true);

            Assert.False(ok);
            Assert.Contains(results, r => r.ErrorMessage!.Contains("4 dígitos"));
        }

        [Fact]
        public void Livro_deve_rejeitar_edicao_menor_que_1()
        {
            var livro = new Livro
            {
                Titulo = "Teste",
                Editora = "Editora",
                Edicao = 0,
                AnoPublicacao = "2020"
            };

            var ctx = new ValidationContext(livro);
            var results = new List<ValidationResult>();

            var ok = Validator.TryValidateObject(livro, ctx, results, true);

            Assert.False(ok);
        }
    }
}
