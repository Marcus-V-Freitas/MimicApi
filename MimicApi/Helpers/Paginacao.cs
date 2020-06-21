using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.Helpers
{
    public class Paginacao
    {
        public int NumeroPagina { get; set; }

        public int RegistrosPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas { get; set; }

        public Paginacao(double qtdeTotalRegistros, int pagRegistrosQtde)
        {
            RegistrosTotaisPorPagina(qtdeTotalRegistros, pagRegistrosQtde);
        }

        public void RegistrosTotaisPorPagina(double qtdeTotalRegistros, int pagRegistrosQtde)
        {
            TotalPaginas = (int)Math.Ceiling(qtdeTotalRegistros / pagRegistrosQtde);
        }
    }
}
