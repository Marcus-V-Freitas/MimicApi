using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.V1.Models.DTO
{
    public class PalavraInputDTO
    {
        [Required(ErrorMessage = "Digite o Nome")]
        [MaxLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite a pontuação")]
        [Range(1, int.MaxValue, ErrorMessage = "Digite uma pontuação maior que 0")]
        public int Pontuacao { get; set; }
    }
}
