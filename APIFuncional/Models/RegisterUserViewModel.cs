using System.ComponentModel.DataAnnotations;

namespace APIFuncional.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "O campo {0}  é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O campo {0}  é obrigatório")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O campo {0} presisa ter entre {2} e {1} caracteres")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "O campo {0}  é obrigatório")]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string? ConfirmPassword { get; set; }
    }
}
