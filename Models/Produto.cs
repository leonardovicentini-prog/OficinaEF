using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OficinaEF.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    // ----- Campos usados na vitrine da Home -----
    [StringLength(120, ErrorMessage = "A descrição curta deve ter no máximo 120 caracteres.")]
    [Display(Name = "Descrição curta")]
    public string? Descricao { get; set; }

    // Obs.: o Range com doubles evita o problema de "0.01" virar 1 em servidor pt-BR
    [Range(0.01, 99999999.99, ErrorMessage = "O preço deve ser maior que zero.")]
    [Display(Name = "Preço")]
    public decimal Preco { get; set; }

    [Range(0.01, 99999999.99, ErrorMessage = "O preço antigo deve ser maior que zero.")]
    [Display(Name = "Preço antigo (opcional)")]
    public decimal? PrecoAntigo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }

    // Selo exibido no card da Home (NOVO, PROMO, ÚLTIMAS...)
    [StringLength(20)]
    public string? Selo { get; set; }

    [StringLength(300, ErrorMessage = "A URL deve ter no máximo 300 caracteres.")]
    [Display(Name = "URL da imagem")]
    public string? ImagemUrl { get; set; }

    // Arte SVG provisória usada na Home enquanto o produto não tem ImagemUrl
    [RegularExpression("^(keyboard|chair|headset|mouse|game|desk)?$", ErrorMessage = "Arte inválida.")]
    [Display(Name = "Arte provisória")]
    public string? Arte { get; set; }

    // Chave Estrangeira para Categoria
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }

    [ValidateNever]
    public Categoria? Categoria { get; set; }

    public Produto()
    {
    }

    public Produto(int id, string nome, decimal preco, int estoque, int categoriaId)
    {
        Id = id;
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
        CategoriaId = categoriaId;
    }
}
