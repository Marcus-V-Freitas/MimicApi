﻿using MimicApi.V1.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicApi.Helpers
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; }

        public Paginacao Paginacao { get; set; }

        public List<LinkDTO> Links { get; set; }

        public PaginationList()
        {
            Links = new List<LinkDTO>();
            Results = new List<T>();
        }
    }
}
