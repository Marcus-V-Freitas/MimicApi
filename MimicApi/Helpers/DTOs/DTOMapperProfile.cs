﻿using AutoMapper;
using MimicApi.V1.Models;
using MimicApi.V1.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.Helpers.DTOs
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            /*AutoMapper - Conversão de objeto para outro copiando os valores

            Palavra > PalavraDTO

             */

            CreateMap<Palavra, PalavraDTO>();

            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();


            CreateMap<PalavraInputDTO, Palavra>();

        }
    }
}
