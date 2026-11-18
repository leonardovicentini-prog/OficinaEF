using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OficinaEF.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da categoria.")]
    [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a descrição.")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    // Relação 1:N (uma categoria tem vários produtos)
    [ValidateNever]
    public List<Produto> Produtos { get; set; } = new();

    // O MVC e o EF usam este construtor para criar o objeto
    public Categoria()
    {
    }

    public Categoria(int id, string nome, string descricao)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
    }
}
