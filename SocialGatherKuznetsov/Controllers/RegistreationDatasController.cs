using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialGatherKuznetsov.Data;
using SocialGatherKuznetsov.Models;

namespace SocialGatherKuznetsov.Controllers
{
    public class RegistreationDatasController : Controller
    {
        private readonly SocialGatherKuznetsovContext _context;

        public RegistreationDatasController(SocialGatherKuznetsovContext context)
        {
            _context = context;
        }

        // GET: RegistreationDatas
        public async Task<IActionResult> Index()
        {
              return _context.RegistreationData != null ? 
                          View(await _context.RegistreationData.ToListAsync()) :
                          Problem("Entity set 'SocialGatherKuznetsovContext.RegistreationData'  is null.");
        }

        // GET: RegistreationDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RegistreationData == null)
            {
                return NotFound();
            }

            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registreationData == null)
            {
                return NotFound();
            }

            return View(registreationData);
        }

        // GET: RegistreationDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RegistreationDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,UserId,Login,Password,token")] RegistreationData registreationData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registreationData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registreationData);
        }

        // GET: RegistreationDatas/Edit/5
        public async Task<IActionResult> Edit()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
                {
                    return NotFound();
                }

            if (registreationData.token != Request.Cookies["token"])
            {
                return Unauthorized();
            }
            return View(registreationData);
        }

        // POST: RegistreationDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,UserId,Login,Password,token")] RegistreationData registreationData)
        {
            if (id != registreationData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registreationData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistreationDataExists(registreationData.Id))
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
            return View(registreationData);
        }

        // GET: RegistreationDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RegistreationData == null)
            {
                return NotFound();
            }

            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registreationData == null)
            {
                return NotFound();
            }

            return View(registreationData);
        }

        // GET: /Account/Login
        public async Task<IActionResult> LoginPage()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> LoginPage(string login, string password)
        {
            // Здесь выполняется проверка введенных данных в базе данных
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => (m.Password == password && m.Login == login));
            if (registreationData!=null)
            {
                // Если пользователь существует и данные верны, выполните вход в аккаунт
                // Например, установите значения в сеанс или формируйте аутентификационный токен
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, registreationData.UserId) };
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                registreationData.token = encodedJwt;
                Response.Cookies.Append("token", encodedJwt, new CookieOptions
                {
                    Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(2))
                });
                Response.Cookies.Append("UserId", registreationData.UserId, new CookieOptions
                {
                    Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(2))
                });
                //Сохраняем в базу
                try
                {
                    _context.Update(registreationData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistreationDataExists(registreationData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home"); // Перенаправляем пользователя на домашнюю страницу после успешного входа
            }
            else
            {
                // Если пользователь не найден или данные неверны, установите сообщение об ошибке
                TempData["ErrorMessage"] = "Неверный логин или пароль. Пожалуйста, попробуйте снова.";

                return View();
            }
        }

        // POST: RegistreationDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RegistreationData == null)
            {
                return Problem("Entity set 'SocialGatherKuznetsovContext.RegistreationData'  is null.");
            }
            var registreationData = await _context.RegistreationData.FindAsync(id);
            if (registreationData != null)
            {
                _context.RegistreationData.Remove(registreationData);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistreationDataExists(int id)
        {
          return (_context.RegistreationData?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
