﻿using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace StkMS.Data.Models
{
    internal class Sale
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }
    }
}