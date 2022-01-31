using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Population.Models
{
    public class PopulationDto
    {
        [Key]
        public int PopulationID { get; set; }
        [Required(ErrorMessage = "Country name is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2900, ErrorMessage = "Year should be within the range(1900 - 2900)")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Total Male count is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Male count must be positive")]
        public decimal TotalMale { get; set; }
        [Required(ErrorMessage = "Total Female count is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Female count must be positive")]
        public decimal TotalFemale { get; set; }
        public decimal Summary { get; set; }        
    }
}
