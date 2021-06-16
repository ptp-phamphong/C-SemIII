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
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;

        public AccountsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CurrentAdmin") == null)
            {
                HttpContext.Session.SetString("PageBeingAdmin", "Accounts");
                return RedirectToAction("LoginAdmin", "Accounts");
            }
            var eshopContext = _context.Accounts;
            return View(eshopContext.ToList());
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }

        public IActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginAdmin(string username, string password)
        {
            bool checkAccount = _context.Accounts.Any(acc => acc.Username == username && acc.Password == password && acc.IsAdmin);
            if (checkAccount)
            {
                HttpContext.Session.SetString("CurrentAdmin", username);
                HttpContext.Session.SetInt32("IdAdmin", _context.Accounts.Where(acc => acc.Username == username && acc.Password == password).FirstOrDefault().Id);
                return RedirectToAction("Index", HttpContext.Session.GetString("PageBeingAdmin"),new {area = "Admin" });
            }
            else
            {
                ViewBag.LoginAdminFail = "Fail to Login Admin Area";
                return View();
            }
        }


        public IActionResult Statistics()
        {
            if(_context.Accounts.Count() <= 3)
            {
                return View("Index", _context.Accounts.ToList());
            }


            var test = _context.Invoices.AsEnumerable().GroupBy(inv => inv.AccountId);
            //Khai báo dictionary với key là account và value là tổng tiền người đó đã mua
            Dictionary<Account, int> fristResult = new Dictionary<Account, int>();
            Dictionary<Account, int> finalResult = new Dictionary<Account, int>();
            

            foreach (var item in test)
            {
                int sumTotal = 0;
                foreach(Invoice smalItem in item)
                {
                    sumTotal += smalItem.Total;
                }
                fristResult.Add(_context.Accounts.Where(acc => acc.Id == item.Key).FirstOrDefault(), sumTotal);
            }

            //Dùng vòng lặp đưa dữ liệu vào dictionary của viewbag
            int flag = fristResult.Count();
            for (int i = 0; i < flag; i++)
            {
                int maxTotal = 0;
                Account acc = null;
                foreach (KeyValuePair<Account, int> item in fristResult)
                {
                    //tìm ra phần tử có total lớn nhất
                    if (maxTotal <= item.Value)
                    {
                        maxTotal = item.Value;
                        acc = item.Key;
                    }
                }
                //Thêm vào cái final Result đây là cái mình đưa vào viewbag và xóa cái frist kia đi để duyệt tiếp
                fristResult.Remove(acc);
                finalResult.Add(acc, maxTotal);
            }
            ViewBag.finalResult = finalResult;
            return View();
        }
    }
}
