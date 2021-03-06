﻿using AutoMapper;
using BookStoreModels;
using BookStoreModels.DTO.Company;
using BookStoreModels.DTO.CoverType;
using BookStoreModels.DTO.Product;
using BookStoreModels.DTO.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CoverTypeForUpdateDTO, CoverType>();
            CreateMap<CoverTypeForInsertDTO, CoverType>();
            CreateMap<ProductForInsertDTO, Product>();
            CreateMap<ProductForUpdateDTO, Product>();
            CreateMap<CompanyForInsertDTO, Company>();
            CreateMap<CompanyForUpdateDTO, Company>();
            CreateMap<AppUserForCommonInsertDTO, ApplicationUser>();
        }
    }
}
