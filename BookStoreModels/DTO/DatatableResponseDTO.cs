using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreModels.DTO
{
    public class DatatableResponseDTO
    {
        public int draw { get; set; }

        public int iTotalRecords { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public object aaData { get; set; }
    }
}
