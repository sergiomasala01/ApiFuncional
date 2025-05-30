namespace APIFuncional.Configuration
{
    public static class CorsConfig
    {
        public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                //uma policy com acessos geral
                options.AddPolicy("Development", builder =>
                            builder
                                .AllowAnyOrigin()  //Aceita requisição de qualquer origem
                                .AllowAnyMethod()  //Aceita qualquer método HTTP (GET, POST, PUT, DELETE...)
                                .AllowAnyHeader()); //Aceita qualquer cabeçalho (ex: Authorization, Content-Type)

                //uma policy com acessos restrito
                options.AddPolicy("Production", builder =>
                            builder
                                .WithOrigins("https://localhost:9000") //Só aceita requisições dessa origem
                                .WithMethods("POST") //Só permite método POST
                                .AllowAnyHeader()); //Aceita qualquer cabeçalho
            });

            return builder;
        }

    }
}
