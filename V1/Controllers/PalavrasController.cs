using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicApi.Database;
using MimicApi.Helpers;
using MimicApi.V1.Models;
using MimicApi.V1.Models.DTO;
using MimicApi.V1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;
        private const string ObterPalavra = "ObterPalavra";
        private const string AtualizarPalavra = "AtualizarPalavra";
        private const string DeletarPalavra = "DeletarPalavra";
        private const string ObterTodasPalavras = "ObterTodas";

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //APP --/api/palavras?data=2019-10-10
        //[Route("")]

        /// <summary>
        /// Operação que retorna todas as palavras
        /// </summary>
        /// <param name="url"> Filtros de pesquisa </param>
        /// <returns> Listagem de Palavras </returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("", Name = ObterTodasPalavras)]
        public IActionResult ObterTodas([FromQuery] UrlPalavraQuery url)
        {
            var item = _repository.ObterTodas(url);

            //Número da página passado maior que o total
            if (item.Results.Count == 0)
                return NotFound();

            PaginationList<PalavraDTO> lista = CriarLinksPalavras(url, item);

            return Ok(lista);
        }



        // --/api/palavras/1
        //[Route("{id}")] //Usar o  template no lugar da rota no caso de adotar o Url.Link
        /// <summary>
        /// Operação que retorna uma palavra de acordo com o código
        /// </summary>
        /// <param name="id"> Identificador da palavra </param>
        /// <returns> Palavra correspondente ao código </returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet(template: "{id}", Name = ObterPalavra)]
        public IActionResult Obter(int id)
        {
            var palavra = _repository.Obter(id);

            if (palavra == null)
                return NotFound();
            //return StatusCode(404);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links.AddRange(new List<LinkDTO>()
            {
                //Adota o Dominio Correto
                new LinkDTO("self", Url.Link(ObterPalavra,new { id=palavraDTO.Id }), "GET"),
                new LinkDTO("update",Url.Link(AtualizarPalavra,new {id=palavraDTO.Id }),"PUT"),
                new LinkDTO("delete",Url.Link(DeletarPalavra,new {id=palavraDTO.Id }),"DELETE")
            });

            return Ok(palavraDTO);
        }

        // --/api/palavras(Post: id, nome, pontuacao, criacao)
        /// <summary>
        /// Operação que realiza o cadastro da palavra
        /// </summary>
        /// <param name="palavraInput"> Objeto a ser cadastrado </param>
        /// <returns> Retorna o objeto criado com o Identificador </returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody] PalavraInputDTO palavraInput)
        {
            if (palavraInput == null)
            {
                //return StatusCode(400);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                //return StatusCode(422);
                return UnprocessableEntity(ModelState);
            }

            var palavra = _mapper.Map<PalavraInputDTO, Palavra>(palavraInput);

            palavra.Criado = DateTime.Now;
            palavra.Ativo = true;

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);


            palavraDTO.Links.Add(new LinkDTO("self", Url.Link(ObterPalavra, new { id = palavraDTO.Id }), "GET"));
            //return Ok();
            return Created($"api/palavras/{palavraDTO.Id}", palavraDTO);
        }

        // --/api/palavras/1 (Put: id, nome, pontuacao, criacao)
        //[Route("{id}")]
        /// <summary>
        /// Operação que atualiza o registro de acordo com seu identificador
        /// </summary>
        /// <param name="id"> Identificador do registro </param>
        /// <param name="palavraInput"> objeto com os dados para alteração </param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPut(template: "{id}", Name = AtualizarPalavra)]
        public IActionResult Atualizar(int id, [FromBody] PalavraInputDTO palavraInput)
        {
            if (palavraInput == null)
            {
                //return StatusCode(400);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                //return StatusCode(422);
                return UnprocessableEntity(ModelState);
            }

            var obj = _repository.Obter(id);

            if (obj == null)
                return StatusCode(404);

            var palavra = _mapper.Map<PalavraInputDTO, Palavra>(palavraInput);

            palavra.Id = id;
            palavra.Atualizado = DateTime.Now;
            palavra.Criado = obj.Criado;
            palavra.Ativo = obj.Ativo;

            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link(ObterPalavra, new { id = palavraDTO.Id }), "GET"));


            return Ok();
        }

        // --/api/palavras/1 (DELETE)
        //[Route("{id}")]]
        /// <summary>
        /// Operação que desativa uma palavra
        /// </summary>
        /// <param name="id"> Identificador da palavra </param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [HttpDelete(template: "{id}", Name = DeletarPalavra)]
        public IActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);

            if (palavra == null)
                return StatusCode(404);

            _repository.Deletar(id);
            //return Ok();
            return NoContent();
        }

        private PaginationList<PalavraDTO> CriarLinksPalavras(UrlPalavraQuery url, PaginationList<Palavra> item)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach (var palavra in lista.Results)
            {
                palavra.Links.Add(new LinkDTO("self", Url.Link(ObterPalavra, new { id = palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO("self", Url.Link(ObterTodasPalavras, url), "GET"));
            //Formato mais popular - (new JsonResult)


            if (item.Paginacao != null)
            {
                //Adicionar no Header
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (url.pagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var queryString = new UrlPalavraQuery()
                    {
                        pagNumero = url.pagNumero + 1,
                        pagRegistrosQtde = url.pagRegistrosQtde,
                        data = url.data
                    };
                    lista.Links.Add(new LinkDTO("next", Url.Link(ObterTodasPalavras, queryString), "GET"));
                }

                if (url.pagNumero - 1 > 0)
                {
                    var queryString = new UrlPalavraQuery()
                    {
                        pagNumero = url.pagNumero - 1,
                        pagRegistrosQtde = url.pagRegistrosQtde,
                        data = url.data
                    };
                    lista.Links.Add(new LinkDTO("prev", Url.Link(ObterTodasPalavras, queryString), "GET"));
                }
            }

            return lista;
        }
    }
}
