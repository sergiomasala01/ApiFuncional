using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIFuncional.Migrations;
using APIFuncional.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace APIFuncional.Controllers
{
    [ApiController]
    [Route("api/conta")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _singInManager; //Gerencia login/senha, lockout etc
        private readonly UserManager<IdentityUser> _userManager; //Gerencia dados do usuário (criar, editar, roles, claims)
        private readonly JwtSettings _jwtSettings; //Configuração do token pegando do appsettings

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<JwtSettings> jwtSettings)
        {
            _singInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value; //Pega a config já populada
        }

        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType] //É uma forma de dar um retorno para algum code que não estamos declarando
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState); //Valida se o objeto veio certo

            var user = new IdentityUser //Criando o usuário
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password); //Cria o usuário com senha

            if (result.Succeeded)
            {
                await _singInManager.SignInAsync(user, false);
                return Ok(await GerarJwt(user.Email));
            }

            return Problem("Falha ao registrar usuário");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState); //Valida se os campos foram preenchidos certinho

            var result = await _singInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true); //false = não manter logado 
                                                                                                                     //true = trava por 5 tentativas erradas
            if (result.Succeeded)
            {
                return Ok(await GerarJwt(loginUser.Email)); //Login deu certo, gera token
            }

            return Problem("Usuário ou senha incorretos");
        }

        private async Task<string> GerarJwt(string email)
        {
            var user = await _userManager.FindByNameAsync(email); //Busca o usuário pelo e-mail
            var roles = await _userManager.GetRolesAsync(user); //Pega os perfis (roles) do usuário

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName) //Adiciona o nome do usuário como claim
            };

            //Adicionando cada perfil (role) como uma claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler(); //Gerador de tokens
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo); //Transformando a chave secreta em byte[]

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //Adicionando claims no token
                Issuer = _jwtSettings.Emissor, //Quem está gerando o token
                Audience = _jwtSettings.Audiencia, //Pra quem é o token
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras), //Validade do token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature) //Assinatura com algoritmo HMAC SHA256
            });

            var encodedToken = tokenHandler.WriteToken(token); //Converte o token em string

            return encodedToken; //Retorna o JWT pronto
        }
    }
}
