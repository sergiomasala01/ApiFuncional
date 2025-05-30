namespace APIFuncional.Configuration
{
    public static class ApiConfig 
    {

        public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            //builder.Services.AddControllers()
            //    .ConfigureApiBehaviorOptions(options =>
            //    {
            //        options.SuppressModelStateInvalidFilter = true; <--- Tirar a validação do asp.net
            //    });

            return builder;
        }

    }
}
