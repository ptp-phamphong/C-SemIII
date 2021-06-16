using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class Account
    {
        
        public int Id { get; set; }

        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} Tên đăng nhập không được bỏ trống")]
        public string Username { get; set; }


        [DisplayName("Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
        public string Avatar { get; set; }
        public bool Status { get; set; }

        public List<Invoice> Invoices { get; set; }
        public List<Cart> Carts { get; set; }
    }
}
