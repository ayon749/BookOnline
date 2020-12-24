using System.ComponentModel.DataAnnotations;

namespace BookOnline.Models

{
    public class Catagory
    {
        [Key]
        public int CatagoryId { get; set; }
        [Required]
        [Display(Name="Catagory Name")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
