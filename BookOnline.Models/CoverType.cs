using System.ComponentModel.DataAnnotations;

namespace BookOnline.Models
{
    public class CoverType
    {

        [Key]
        public int CoverId { get; set; }
        [Required]
        [Display(Name="Cover Type ")]
        public string Name { get; set; }
    }
}
