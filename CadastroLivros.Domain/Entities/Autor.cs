using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class Autor
    {
        public int CodAu { get; set; }

        [Required(ErrorMessage = "Informe o nome do(a) autor(a).")]
        [StringLength(40, ErrorMessage = "O nome deve ter no máximo 40 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public ICollection<LivroAutor> Livros { get; set; } = new List<LivroAutor>();
    }
}
