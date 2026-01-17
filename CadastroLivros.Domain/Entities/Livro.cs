using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class Livro
    {
        public int Codl { get; set; }
        
        [Required(ErrorMessage = "Informe o Título.")]
        [StringLength(40, ErrorMessage = "O Título deve ter no máximo 40 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a Editora.")]
        [StringLength(40, ErrorMessage = "A Editora deve ter no máximo 40 caracteres.")]
        public string Editora { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Edicao { get; set; }

        [Required(ErrorMessage = "Informe Ano de Publicação.")]        
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Ano de publicação deve ter 4 dígitos.")]
        public string AnoPublicacao { get; set; } = string.Empty;

        public ICollection<LivroAutor> Autores { get; set; } = new List<LivroAutor>();
        public ICollection<LivroAssunto> Assuntos { get; set; } = new List<LivroAssunto>();
        public ICollection<LivroPreco> Precos { get; set; } = new List<LivroPreco>();
    }
}
