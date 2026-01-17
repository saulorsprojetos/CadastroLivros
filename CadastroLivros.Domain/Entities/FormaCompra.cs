using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class FormaCompra
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a forma de compra.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public ICollection<LivroPreco> Precos { get; set; } = new List<LivroPreco>();
    }
}
