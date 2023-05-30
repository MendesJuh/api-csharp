using DevIO.Api.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "7748fd1785794fba82d05847e1a93cf1";
                o.LogId = new Guid("fac20960-ac71-4c2e-b9f4-9a0be7f902b5");
            });

            //services.AddLogging(builder =>
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "7748fd1785794fba82d05847e1a93cf1";
            //        o.LogId = new Guid("fac20960-ac71-4c2e-b9f4-9a0be7f902b5");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);

            //});

            services.AddHealthChecks()
                .AddElmahIoPublisher(apiKey: "7748fd1785794fba82d05847e1a93cf1", new Guid("fac20960-ac71-4c2e-b9f4-9a0be7f902b5"), application: "API Fornecedores")
               .AddCheck(name: "Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
               .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            services.AddHealthChecksUI();

            return services;
        }

        public static IApplicationBuilder UseLogginConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            app.UseHealthChecks("/api/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(options => { options.UIPath = "/api/hc-ui"; });
            return app;
        }
    }
}
