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


    public class AccountsController : Controller
    {
        private readonly EshopContext _context;

        public AccountsController(EshopContext context)
        {
            _context = context;
        }
        


        // GET: Accounts
        public IActionResult Index()
        {
            HttpContext.Session.SetString("PageBeing", "Accounts");
            if (HttpContext.Session.GetString("CurrentUser") == null)
            {
                
                return View("Login");
            }
            return View(_context.Accounts.Where(acc => acc.Username == HttpContext.Session.GetString("CurrentUser")).ToList());
        }

        // GET: Accounts/Details/5
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

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
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

        // GET: Accounts/Edit/5
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

        // POST: Accounts/Edit/5
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

        // GET: Accounts/Delete/5
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

        // POST: Accounts/Delete/5
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


        //Handeling login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            bool LoginResult = _context.Accounts.Any(acc => acc.Username == username && acc.Password == password);
            if (LoginResult)
            {
                HttpContext.Session.SetString("CurrentUser", username);
                HttpContext.Session.SetInt32("Id", _context.Accounts.Where(acc => acc.Username == username).FirstOrDefault().Id);
                ViewBag.BeLogin = true;
                return RedirectToAction("Index", HttpContext.Session.GetString("PageBeing"));
            }
            else
            {
                HttpContext.Session.SetInt32("Id", Convert.ToInt32(-1));
                ViewBag.ErrorMess = "Login Fail !!!!!!!!!!!!!";
                return View();
            }
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("CurrentUser");
            HttpContext.Session.Remove("Id");
            return RedirectToAction("Index", HttpContext.Session.GetString("PageBeing"));
        }



        //handeling Register
        public  IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            account.IsAdmin = false;
            account.Status = true;

            if (ModelState.IsValid)
            {
                _context.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Index", "Accounts");
            }
            else
            {
                ViewBag.ErrorRegister = "Đăng ký thất bại";
                return View();
            }
        }

        
    }
}
