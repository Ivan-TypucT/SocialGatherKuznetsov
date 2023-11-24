using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialGatherKuznetsov.Models;
using System.Diagnostics;
using SocialGatherKuznetsov.Models;
using SocialGatherKuznetsov.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialGatherKuznetsov.Data;
using SocialGatherKuznetsov.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace SocialGatherKuznetsov.Controllers
{
    public class HomeController : Controller
    {

        private readonly SocialGatherKuznetsov2Context _context;
        private readonly SocialGatherKuznetsovContext _context2;

        public HomeController(SocialGatherKuznetsov2Context context, SocialGatherKuznetsovContext context2)
        {
            _context = context;
            _context2 = context2;
        }


        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {

            return _context.Card != null ?
                          View(await _context.Card.ToListAsync()) :
                          Problem("Entity set 'SocialGatherKuznetsovContext.Card'  is null.");
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string SearchString)
        {
            if (_context.Card == null)
                return Problem("Entity set 'SocialGatherKuznetsovContext.Card'  is null.");
            var cards = from m in _context.Card
                        select m;
            if (!String.IsNullOrEmpty(SearchString))
            {
                cards = cards.Where(s => s.Name!.Contains(SearchString));
            }
            return View(await cards.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(card);
        }


        // GET: RegistreationDatas/Edit/5
        public async Task<IActionResult> Edit()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context2.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }
            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.UserId == registreationData.UserId);
            if (card == null)
                return NotFound();
            return View(card);
        }

        // POST: RegistreationDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("Id,Name,Text,Place")] Card card)
        {
            if (id != card.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var any = await _context.Card
                .FirstOrDefaultAsync(m => ( m.Id == card.Id));
                if (any == null)
                {
                    //Логин уже существует
                    return View(card);
                }
                any.Name = card.Name;
                any.Text = card.Text;
                any.Place = card.Place;
                try
                {
                    _context.Update(any);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        public async Task<IActionResult> Delete()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context2.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }
            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.UserId == registreationData.UserId);
            if (card == null)
                return NotFound();
            return View(card);
        }

        // POST: RegistreationDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Card == null)
            {
                return Problem("Entity set 'SocialGatherKuznetsovContext.RegistreationData'  is null.");
            }
            var card = await _context.Card.Include(c => c.GuestsList).FirstOrDefaultAsync(m => m.Id == id);
            if (card != null)
            {
               // _context.GuestsLists.RemoveRange(card.GuestsList);
                _context.Card.Remove(card);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Chats()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        
        public async Task<IActionResult> Sub(int id)
        {

            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context2.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (_context.Card == null)
                return Problem("Entity set 'SocialGatherKuznetsovContext.Card'  is null.");
            var card = await _context.Card
                .Include(c => c.GuestsList)
                .FirstOrDefaultAsync(m => m.Id == id); 
            if (card == null) { return NotFound(); }
            if (card.GuestsList.Any(g => g.UserId == registreationData.UserId) || card.GuestsNumCurrent==card.GuestsNumMax)
                return Content("Вы уже записаны на мероприятие или число гостей превышено!");
            card.GuestsList.Add(new Guest { UserId = Userid }) ;
            card.GuestsNumCurrent++;
            try
            {
                _context.Update(card);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}