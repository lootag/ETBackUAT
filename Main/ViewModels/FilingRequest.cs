using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;
using Models.Enums;
namespace Main.ViewModels
{
    public class FilingRequest
    {
        [Required]
        public IList<int> CIKS { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateEnd { get; set; }
        [Required]
        public IList<Filing> Filings { get; set; }
        [Required]
        public IList<Item> Items { get; set; }
    }
}