using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class LivroAutor
    {
        public int Livro_Codl { get; set; }
        public int Autor_CodAu { get; set; }

        public Livro Livro { get; set; } = null;
        public Autor Autor { get; set; } = null;
    }
}
