﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using MimicApi.Database;
using MimicApi.Helpers.DTOs;
using MimicApi.Helpers.Swagger;
using MimicApi.V1.Repositories;
using MimicApi.V1.Repositories.Interfaces;

namespace MimicApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /* Auto Mapper - Configuração para DTO*/
            #region AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion


            services.AddDbContext<MimicContext>(opt =>
            {
                opt.UseSqlite("Data Source=Database\\Mimic.db");
            });
            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();

            #region "Version"
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true; //Retornar a todas as versões suportadas e Obsoletas no Cabeçalho
                cfg.ApiVersionReader = new HeaderApiVersionReader("api-version"); //Versão passada por parâmetro do Cabeçalho (A chave é o nome colocado)
                cfg.AssumeDefaultVersionWhenUnspecified = true; //Quando versão não é especificada usar a padrão
                cfg.DefaultApiVersion = new ApiVersion(1, 0); //Versão Padrão de Uso (Nesse caso a 1.0)
            });
            #endregion

            services.AddSwaggerGen(cfg =>
            {
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First()); //Resolver Conflitos de Versão/Rotas (Pega a primeira)

                //Da mais nova para a mais antiga
                cfg.SwaggerDoc("v2.0", new Swashbuckle.AspNetCore.Swagger.Info() //Informações
                {
                    Title = "MimicApi - V2.0",
                    Version = "V2.0"
                });

                cfg.SwaggerDoc("v1.1", new Swashbuckle.AspNetCore.Swagger.Info() //Informações
                {
                    Title = "MimicApi - V1.1",
                    Version = "V1.1"
                });

                cfg.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info() //Informações
                {
                    Title = "MimicApi - V1.0",
                    Version = "V1.0"
                });


                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;

                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";

                var caminhoArquivoXMLComentario = Path.Combine(caminhoProjeto, nomeProjeto);

                cfg.IncludeXmlComments(caminhoArquivoXMLComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {

                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();

                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseMvc();

            app.UseSwagger(); // /swagger/v1/swagger.json - RouteTemplate
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI - V2.0"); //Localidade do Arquivo 2.0
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI - V1.1"); //Localidade do Arquivo 1.1
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI - V1.0"); //Localidade do Arquivo 1.0
                cfg.RoutePrefix = string.Empty; //Ao acessar o site será redirecionado para Swagger
            });
        }
    }
}
