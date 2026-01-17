using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Domain.Entities
{
    public class LivroAssunto
    {
        public int Livro_Codl { get; set; }
        public int Assunto_codAs { get; set; }

        public Livro Livro { get; set; } = null;
        public Assunto Assunto { get; set; } = null;
    }
}
