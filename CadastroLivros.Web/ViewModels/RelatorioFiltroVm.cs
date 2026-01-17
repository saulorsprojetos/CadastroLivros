namespace CadastroLivros.Web.ViewModels
{
    public class RelatorioFiltroVm
    {
        public int? AutorId { get; set; }
        public List<ItemVm> Autores { get; set; } = new();
    }
}
