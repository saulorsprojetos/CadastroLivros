using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class Assunto
    {
        public int codAs { get; set; }

        [Required(ErrorMessage = "Informe o assunto.")]
        [StringLength(20, ErrorMessage = "O nome deve ter no máximo 20 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        public ICollection<LivroAssunto> Livros { get; set; } = new List<LivroAssunto>();
    }
}
