using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class LivroPreco
    {
        public int Livro_Codl { get; set; }
        public int FormaCompra_Id { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        public Livro Livro { get; set; } = null;
        public FormaCompra FormaCompra { get; set; } = null;
    }
}
