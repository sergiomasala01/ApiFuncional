using Microsoft.OpenApi.Models;

namespace APIFuncional.Configuration
{
    public static class SwaggerConfig
    {
        public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                //Configurando o Swagger para aceitar autenticação com JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}", //Texto de exemplo que aparece na UI do Swagger
                    Name = "Authorization", //Cabeçalho onde o token deve ser enviado
                    Scheme = "Bearer", //Tipo do esquema que vamos usar (Bearer Token)
                    BearerFormat = "JWT", //Formato do token
                    In = ParameterLocation.Header, //Token vai no cabeçalho da requisição
                    Type = SecuritySchemeType.ApiKey //Tipo do esquema é ApiKey (mesmo usando Bearer)
                });

                //Dizendo para o Swagger usar a definição de segurança acima em todas as rotas
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {           
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, //Tipo da referência
                                Id = "Bearer" //ID que bate com o definido acima
                            }
                        },
                        new string[] {}
                    }
                });
            });
            return builder;
        }
    }
}
