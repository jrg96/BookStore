﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreModels.DTO.Company
{
    public class CompanyForInsertDTO
    {
        [Required]
        public string Name { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsAuthorizedCompany { get; set; }
    }
}
