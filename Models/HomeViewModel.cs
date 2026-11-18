namespace OficinaEF.Models;

// Dados que a Home recebe do HomeController
public class HomeViewModel
{
    public List<Categoria> Categorias { get; set; } = new();
    public List<Produto> Produtos { get; set; } = new();
}
