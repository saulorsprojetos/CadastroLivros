using System.ComponentModel.DataAnnotations;

namespace CadastroLivros.Web.ViewModels
{
    public class LivroVm
    {
        public int? Codl { get; set; }

        [Required(ErrorMessage = "Informe o Título.")]
        [StringLength(40, ErrorMessage = "O Título deve ter no máximo 40 caracteres.")]
        public string Titulo { get; set; } = "";

        [Required(ErrorMessage = "Informe a Editora.")]
        [StringLength(40, ErrorMessage = "A Editora deve ter no máximo 40 caracteres.")]
        public string Editora { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Informe uma edição maior que zero.")]
        public int Edicao { get; set; }

        [Required(ErrorMessage = "Informe o Ano de Publicação.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Ano de publicação deve ter 4 dígitos.")]
        public string AnoPublicacao { get; set; } = "";

        [Required(ErrorMessage = "Selecione pelo menos 1 Autor(a).")]
        public List<int> AutoresSelecionados { get; set; } = new();

        [Required(ErrorMessage = "Selecione pelo menos 1 Assunto.")]
        public List<int> AssuntosSelecionados { get; set; } = new();

        public List<PrecoItemVm> Precos { get; set; } = new();

        public List<ItemVm> Autores { get; set; } = new();
        public List<ItemVm> Assuntos { get; set; } = new();
        public List<ItemVm> FormasCompra { get; set; } = new();
    }

    public class ItemVm
    {
        public int Id { get; set; }
        public string Texto { get; set; } = "";
    }

    public class PrecoItemVm
    {
        public int FormaCompraId { get; set; }
        public string FormaCompraNome { get; set; } = "";        
        public decimal? Valor { get; set; }
    }
}
