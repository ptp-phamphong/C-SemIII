using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;

namespace Eshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly EshopContext _context;

        public CartsController(EshopContext context)
        {
            _context = context;
        }


        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["ProductId"] = new SelectList(_context.Prodcuts, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Prodcuts, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Prodcuts, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Prodcuts, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)

        {
            return _context.Carts.Any(e => e.Id == id);
        }

        //Xử lý thêm vào giỏ hàng
        [HttpPost]
        public IActionResult Add(int id, int quantity)
        {
            HttpContext.Session.SetString("PageBeing", "Products");
            //Kiêm tra xem có đăng nhập chưa
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                
                return RedirectToAction("Login", "Accounts");
            }

            var tempCart = _context.Carts.Where(ca => ca.ProductId == id).FirstOrDefault();
            if (tempCart != null)
            {//Nếu trong giỏ đã có món này thì cộng thêm
                tempCart.Quantity += quantity;
                _context.Carts.Update(tempCart);
            }
            else
            {//Nếu chưa có thì thêm món đó vào
                Cart c = new Cart();
                c.ProductId = id;
                c.Quantity = quantity;
                //Tạm thời để mặc định sau này quay lại vào sửa, quên là chetcondime
                //Không có id = 1 các bạn ạ, cái ditconme cho bằng 2 thôi
                c.AccountId = (int)HttpContext.Session.GetInt32("Id");
                _context.Add(c);
            }

            //Lưu lại thông tin
            _context.SaveChanges();
            var lst = _context.Carts.Include(ca => ca.Account).Include(pro =>pro.Product).ToList();
            return View("Index",lst);
        }

        //Phương thức index cho cart
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                HttpContext.Session.SetString("PageBeing", "Carts");
                return RedirectToAction("Login","Accounts");
            }
            var all = _context.Carts.Include(c => c.Account).Include(c => c.Product).Where(c=>c.Id == HttpContext.Session.GetInt32("Id")).ToList();
            //Chỉ hiển thị những thông tin của người dùng, ta sẽ xét biến session ở đây
            //Tạm thời xét Session thì lâu quá nên tạm cứ để mặc định đi
            return View(all);
        }

        public IActionResult DeleteAll()
        {
            if (_context.Carts.Include(ca => ca.Account).Include(ca => ca.Product).Count() != 0)
            {
                List<Cart> temp = _context.Carts.ToList();
                foreach (Cart c in temp)
                {
                    _context.Carts.Remove(c);
                    _context.SaveChanges();
                }

                var allButNull = _context.Carts.ToList();
                return View("Index", allButNull);
            }
            else
            {
                return View("Index",null);
            }
        }


        public IActionResult Payment(int sum)
        {
            var RecentCart = _context.Carts.Include(ca => ca.Account).Include(ca => ca.Product).ToList();
            //Lấy ra người dùng hiện tại
            Account account = _context.Accounts.Where(ac => ac.Id == HttpContext.Session.GetInt32("Id")).FirstOrDefault();

            if(RecentCart.Count != 0)
            {//Xử lý thêm vào invoice, invoicedetails và xóa cart hiện tại
                DateTime da = DateTime.Now;
                Invoice inv = new Invoice();
                inv.Code = da.ToString();
                inv.AccountId = account.Id;
                inv.IssuedDate = da;
                inv.ShippingAddress = account.Address;
                inv.ShippingPhone = account.Phone;
                inv.Total = sum;
                inv.Status = true;
                _context.Invoices.Add(inv);
                _context.SaveChanges();

                //Sắp xếp lại danh sách hóa đơn để lấy được hóa đơn cuối cùng, từ đó lấy được id
                //Cũng phải lấy theo id người dùng vì có thể có người xài chung, web to mà
                //Sau đó lấy id ra thôi
                int RecentInvoiceId = _context.Invoices.Where(inv => inv.AccountId == HttpContext.Session.GetInt32("Id")).OrderBy(inv => inv.Id).LastOrDefault().Id;
                

                //Xử lý thêm từng hàng trong invoiceDetail
                foreach(Cart ca in RecentCart)
                {
                    //Thêm 1 dòng detail
                    InvoiceDetail invd = new InvoiceDetail();
                    invd.InvoiceId = RecentInvoiceId;
                    invd.ProductId = ca.ProductId;
                    invd.Quantity = ca.Quantity;
                    _context.InvoiceDetails.Add(invd);

                    //Xóa 1 dòng trong cart
                    _context.Carts.Remove(ca);
                    
                    //Lưu lại
                    _context.SaveChanges();
                }
            }
            else
            {//Nếu không có gì trong cart mà người dùng vẫn nhấn nút, hoặc cố tình nhấn nút, hay thật đấy
                ViewBag.Error = "Nothing in your cart";
                return View("Index");
            }

            //Truy vấn lại để hiển thị ra cart null
            var cartButNull = _context.Carts;
            return RedirectToAction("Index","Invoices", _context.Invoices.Include(inv=>inv.Account).Include(inv=>inv.InvoiceDetails));
        }

    }
}
