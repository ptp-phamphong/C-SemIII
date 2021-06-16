using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Status { get; set; }

        public List<Product> Products { get; set; }
    }
}
