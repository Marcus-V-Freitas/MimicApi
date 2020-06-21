using Microsoft.EntityFrameworkCore;
using MimicApi.Database;
using MimicApi.Helpers;
using MimicApi.V1.Models;
using MimicApi.V1.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.V1.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _context;

        public PalavraRepository(MimicContext context)
        {
            _context = context;
        }

        public void Atualizar(Palavra palavra)
        {
            _context.Palavras.Update(palavra);
            _context.SaveChanges();
        }

        public void Cadastrar(Palavra palavra)
        {
            _context.Palavras.Add(palavra);
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _context.Palavras.Update(palavra);
            _context.SaveChanges();
        }

        public PaginationList<Palavra> ObterTodas(UrlPalavraQuery url)
        {
            var lista = new PaginationList<Palavra>();

            //Query (Executada diretamente na base de dados)
            var item = _context.Palavras.AsNoTracking().AsQueryable();

            if (url.data.HasValue)
            {
                item = item.Where(x => x.Criado > url.data.Value || x.Atualizado > url.data.Value);

            }

            //Paginação
            if (url.pagNumero.HasValue)
            {
                var qtdeTotalRegistros = item.Count();
                item = item.Skip((url.pagNumero.Value - 1) * url.pagRegistrosQtde.Value).Take(url.pagRegistrosQtde.Value);

                lista.Paginacao = new Paginacao((double)qtdeTotalRegistros, url.pagRegistrosQtde.Value)
                {
                    NumeroPagina = url.pagNumero.Value,
                    RegistrosPorPagina = url.pagRegistrosQtde.Value,
                    TotalRegistros = qtdeTotalRegistros,
                };

            }
            lista.Results.AddRange(item.ToList());
            return lista;
        }

        public Palavra Obter(int id)
        {
            return _context.Palavras.AsNoTracking().FirstOrDefault(x => x.Id.Equals(id));
        }
    }
}
