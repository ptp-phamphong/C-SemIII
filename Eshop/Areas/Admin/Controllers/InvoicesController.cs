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
    public class InvoicesController : Controller
    {
        private readonly EshopContext _context;

        public InvoicesController(EshopContext context)
        {
            _context = context;
        }

        // GET: Admin/Invoices
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CurrentAdmin") == null)
            {
                HttpContext.Session.SetString("PageBeingAdmin", "Invoices");
                return RedirectToAction("LoginAdmin", "Accounts");
            }
            var eshopContext = _context.Invoices.Include(es => es.Account).Include(es => es.InvoiceDetails);
            return View(eshopContext.ToList());
        }

        // GET: Admin/Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Admin/Invoices/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            return View();
        }

        // POST: Admin/Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Admin/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // POST: Admin/Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Admin/Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Admin/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }

        public IActionResult Statistics()
        {
            var lst = _context.Invoices.Include(inv => inv.Account).Include(inv => inv.InvoiceDetails);
            List<Invoice> lstInvByDate = lst.OrderByDescending(inv => inv.IssuedDate).ToList();
            List<Invoice> lstInvByTotal = lst.OrderByDescending(inv => inv.Total).ToList();
            List<Invoice> resultsByDate = new List<Invoice>();
            List<Invoice> resultsByTotal = new List<Invoice>();
            for (int i = 0; i < 3; i++)
            {
                resultsByDate.Add(lstInvByDate[i]);
                resultsByTotal.Add(lstInvByTotal[i]);
            }
            ViewBag.SortTotal = resultsByTotal;
            return View(resultsByDate);
        }
    }
}
