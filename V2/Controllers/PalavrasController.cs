using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.V2.Controllers
{
    /*
     O versionamento pode ser feito de 2 formas:
        Como parte da Url: Ex: api/v1/Palavras (Usar route com apiVersion)
        Como QueryStrin: Ex: api/Palavras&api-version=1 (Comentar Route com apiVersion)

     */
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    //Podem ter várias versões para se especificar
    [ApiVersion("2.0", Deprecated = true)]
    public class PalavrasController : ControllerBase
    {
        private const string ObterTodasPalavras = "ObterTodas";

        [MapToApiVersion("2.0")] //Mapeia Método (Funciona somente nessa versão)
        [HttpGet("", Name = ObterTodasPalavras)]
        public string ObterTodas()
        {
            return "Ola Mundo";
        }


    }
}
