namespace CadastroLivros.Web.ViewModels
{
    public class PaginacaoVm<T>
    {
        public List<T> Items { get; set; } = new();

        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }

        public int ContagemTotal { get; set; }
        public int TotalPaginas { get; set; }

        public string? Query { get; set; }

        public int From => ContagemTotal == 0 ? 0 : ((Pagina - 1) * TamanhoPagina) + 1;
        public int To => Math.Min(Pagina * TamanhoPagina, ContagemTotal);

        public bool HasPrev => Pagina > 1;
        public bool HasNext => Pagina < TotalPaginas;
    }
}
