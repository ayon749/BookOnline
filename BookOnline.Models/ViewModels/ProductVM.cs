using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookOnline.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CatagoryList { get; set; }
        public IEnumerable<SelectListItem> CoverList { get; set; }
    }
}
