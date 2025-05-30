using System.Text;
using APIFuncional.Data;
using APIFuncional.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace APIFuncional.Configuration
{
    public static class IdentityConfig
    {

        public static WebApplicationBuilder AddIdentityConfig(this WebApplicationBuilder builder)
        {
            //Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>() //Adicionando identity no asp.net com usuário e perfil do usuário
                            .AddRoles<IdentityRole>()                  //Adicionando que vamos utilizar perfils
                            .AddEntityFrameworkStores<ApiDbContext>(); //Identificando qual store estamos usando e apontando para o contexto do banco

            //JWT
            var JwtSettingsSection = builder.Configuration.GetSection("JwtSettings"); //Pegando os dados especificados na appsettings
            builder.Services.Configure<JwtSettings>(JwtSettingsSection); //Populando a classe (JwtSettings) com os dados da appsettings no inicio da aplicação

            var jwtSettings = JwtSettingsSection.Get<JwtSettings>(); //Pegando uma instancia da classe (JwtSettings) populada
            var key = Encoding.ASCII.GetBytes(jwtSettings.Segredo); //Transformando o campo segredo configurado na appsettings em uma chave encodada em ASCII

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Configurando a aplicação asp.net que vai usar o JWT para autenticação
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //Verificando se o token é valido (fazendo um desafio para provar que o token é valido)
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true; //Trabalha somente utilizando https
                options.SaveToken = true; //Permitir que o token seja salvo apos a autenticação mas somente no http, ele vai estar diponível naquela requisicao
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key), //É a chave que vai ser usada para emissão do token
                    ValidateIssuer = true, //Validar quem foi o Emissor
                    ValidateAudience = true, //Validar se a minha audiencia é compativel com a audiencia do token
                    ValidAudience = jwtSettings.Audiencia, //Informando a minha audiencia
                    ValidIssuer = jwtSettings.Emissor //Informando o meu emissor
                };
            });

            return builder;
        }

    }
}
