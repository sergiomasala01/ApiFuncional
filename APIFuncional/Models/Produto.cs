using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace APIFuncional.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = ("O campo {0} deve conter {2} a {1} caracteres"))]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = ("O preço deve ser maior que zero"))]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Quantidade de Estoque")]
        [Range(1, 999999, ErrorMessage = "O campo {0} deve ser maior que zero e menor que 999.999")]
        public int QuantidadeEstoque { get; set; }


        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(160, MinimumLength = 1, ErrorMessage = ("O campo {0} deve conter {2} a {1} caracteres"))]
        public string? Descricao { get; set; }
    }
}
