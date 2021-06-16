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

namespace Eshop.Areas.Admin.Controllers
{

    

    [Area("Admin")]
    public class ProductsController : Controller
    {
        //public const DateTime deff = DateTime.Now;
        private readonly EshopContext _context;

        public ProductsController(EshopContext context)
        {
            
            _context = context;
        }

        // GET: Admin/Products
        public  IActionResult Index()
        {
            if (HttpContext.Session.GetString("CurrentAdmin") == null)
            {
                HttpContext.Session.SetString("PageBeingAdmin", "Products");
                return RedirectToAction("LoginAdmin", "Accounts");
            }
            var eshopContext = _context.Prodcuts.Include(p => p.ProductType);
            return View(eshopContext.ToList());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Prodcuts
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Id");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,sku,Name,Price,Stock,ProductTypeId,Image,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Id", product.ProductTypeId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Prodcuts.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Id", product.ProductTypeId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,sku,Name,Price,Stock,ProductTypeId,Image,Status")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Id", product.ProductTypeId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Prodcuts
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Prodcuts.FindAsync(id);
            _context.Prodcuts.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Prodcuts.Any(e => e.Id == id);
        }

        public IActionResult Statistics()
        {

            List<Product> AllListSortes = _context.Prodcuts.Include(p => p.ProductType).OrderByDescending(p => p.Stock).ToList();
            List<Product> results = new List<Product>();
            for (int i = 0; i < 3; i++)
            {
                results.Add(AllListSortes[i]);
            }

            var test = _context.InvoiceDetails.AsEnumerable().GroupBy(invd => invd.ProductId);

            //Khai báo 2 dictionary với key là Product và value là số lượng đã bán
            Dictionary<Product, int> fristResult = new Dictionary<Product, int>();
            Dictionary<Product, int> finalResult = new Dictionary<Product, int>();


            foreach (var item in test)
            {
                int sumTotal = 0;
                foreach (InvoiceDetail smalItem in item)
                {
                    sumTotal += smalItem.Quantity;
                }
                fristResult.Add(_context.Prodcuts.Where(pro => pro.Id == item.Key).FirstOrDefault(), sumTotal);
            }

            //Dùng vòng lặp để đưa dictionary final vào dữ liệu của ViewBag
            int flag = fristResult.Count();
            for (int i = 0; i < flag; i++)
            {
                int maxTotal = 0;
                Product pro = null;
                foreach (KeyValuePair<Product, int> item in fristResult)
                {
                    //Tìm ra phần tử lớn nhất rồi thêm vào và cứ lần lượt như vậy
                    if (maxTotal <= item.Value)
                    {
                        maxTotal = item.Value;
                        pro = item.Key;
                    }
                }
                //Thêm vào cái final Result đây là cái mình đưa vào viewbag và xóa cái frist kia đi để duyệt tiếp
                fristResult.Remove(pro);
                finalResult.Add(pro, maxTotal);
            }
            ViewBag.finalResult1 = results;
            ViewBag.finalResult = finalResult;
            return View();
        }



        [HttpPost]
        public IActionResult Statistics(DateTime begin, DateTime end)
        {

            DateTime def = new DateTime();
            if (DateTime.Compare(end, def) == 0)
            {
                end = DateTime.Now;
            }

            List<Product> AllListSortes = _context.Prodcuts.Include(p => p.ProductType).OrderByDescending(p=>p.Stock).ToList();
            List<Product> results = new List<Product>();
            for(int i = 0; i < _context.Prodcuts.Count(); i++)
            {
                results.Add(AllListSortes[i]);
            }

            var test = _context.InvoiceDetails.Where(inv => inv.Invoice.IssuedDate >= begin && inv.Invoice.IssuedDate <= end).AsEnumerable().GroupBy(invd => invd.ProductId);

            //Khai báo 2 dictionary với key là Product và value là số lượng đã bán
            Dictionary<Product, int> fristResult = new Dictionary<Product, int>();
            Dictionary<Product, int> finalResult = new Dictionary<Product, int>();
            
            
            foreach (var item in test)
            {
                int sumTotal = 0;
                foreach (InvoiceDetail smalItem in item)
                {
                    sumTotal += smalItem.Quantity;
                }
                fristResult.Add(_context.Prodcuts.Where(pro => pro.Id == item.Key).FirstOrDefault(), sumTotal);
            }

            //Dùng vòng lặp để đưa dictionary final vào dữ liệu của ViewBag
            int flag = fristResult.Count();
            for(int i=0;i<flag; i++)
            {
                int maxTotal = 0;
                Product pro = null;
                foreach(KeyValuePair<Product, int> item in fristResult)
                {
                    //Tìm ra phần tử lớn nhất rồi thêm vào và cứ lần lượt như vậy
                    if(maxTotal <= item.Value)
                    {
                        maxTotal = item.Value;
                        pro = item.Key;
                    }
                }
                //Thêm vào cái final Result đây là cái mình đưa vào viewbag và xóa cái frist kia đi để duyệt tiếp
                fristResult.Remove(pro);
                finalResult.Add(pro, maxTotal);
            }
            ViewBag.finalResult1 = results;
            ViewBag.finalResult = finalResult;
            return View();
        }
    }
}
