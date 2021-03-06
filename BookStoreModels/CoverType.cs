﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreModels
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [Display(Name = "Cover Type Name")]
        public string Name { get; set; }
    }
}
