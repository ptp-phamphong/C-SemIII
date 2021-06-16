using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; } // Navigation reference property
        public DateTime IssuedDate { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public int Total { get; set; }
        public bool Status { get; set; }

        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
