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

namespace SocialGatherKuznetsov.Controllers
{
    public class RegistreationDatasController : Controller
    {
        private readonly SocialGatherKuznetsovContext _context;
        private readonly SocialGatherKuznetsov2Context _context2;


        public RegistreationDatasController(SocialGatherKuznetsovContext context, SocialGatherKuznetsov2Context context2)
        {
            _context = context;
            _context2 = context2;
        }

        

        
       // public RegistreationDatasController(SocialGatherKuznetsov2Context context)
       // {
       //     _context2 = context;
       // }
        
        // GET: RegistreationDatas
        public async Task<IActionResult> Index()
        {
              return _context.RegistreationData != null ? 
                          View(await _context.RegistreationData.ToListAsync()) :
                          Problem("Entity set 'SocialGatherKuznetsovContext.RegistreationData'  is null.");
        }

        // GET: RegistreationDatas/Details/5
        public async Task<IActionResult> MyProfile()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
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
        public async Task<IActionResult> Create([Bind("Id,Email,UserId,Login,Password,salt,token")] RegistreationData registreationData, string password)
        {
            if (ModelState.IsValid)
            {
                var any = await _context.RegistreationData
                .FirstOrDefaultAsync(m => (m.Login == registreationData.Login));
                if(any!=null)
                {
                    //Логин уже существует
                    return View();
                }
                byte[] salt = RSACrypt.GenerateSalt(16);
                registreationData.salt = salt;
                registreationData.Password = RSACrypt.GeneratePasswordHash(password, salt);
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
                    return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }
            return View(registreationData);
        }

        // POST: RegistreationDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,UserId,Login,Password,salt,token")] RegistreationData registreationData, string password)
        {
            if (id != registreationData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var any = await _context.RegistreationData
                .FirstOrDefaultAsync(m => (m.Login == registreationData.Login && m.Id != registreationData.Id));
                if (any != null)
                {
                    //Логин уже существует
                    return View();
                }
                var find = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.Id == id);
                var change = registreationData;
                registreationData = find;
                registreationData.Login = change.Login;
                registreationData.Email = change.Email;
                registreationData.UserId = change.UserId;
                registreationData.salt = RSACrypt.GenerateSalt(16);
                registreationData.Password = RSACrypt.GeneratePasswordHash(password, registreationData.salt);
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
        public async Task<IActionResult> Delete()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            return View(registreationData);
        }

        // GET: /Account/Login
        public async Task<IActionResult> LoginPage()
        {
            string oKey;
            string sKey;
            RSACrypt.GenerateKeys(out oKey, out sKey);

            TempData.Add(oKey,sKey); // Сохраняем закрытый ключ в переменную класса

            Response.Cookies.Append("OpenKey", oKey, new CookieOptions
            {
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(20))
            });
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> LoginPage(string login, string password, string encryptedPassword)
        {
            if(encryptedPassword == "")
            {
                TempData["ErrorMessage"] = "Ошибка шифрования пароля. Попробуйте снова.";
                return View();
            }
            // Здесь выполняется проверка введенных данных в базе данных
            var oKey = Request.Cookies["OpenKey"];
            var data = RSACrypt.Decrypt((string)TempData[oKey], encryptedPassword);
            if (data == null)
                return RedirectToAction("LoginPage", "RegistreationDatas"); 

            byte[] base64 = Convert.FromBase64String(data);
            password = Encoding.UTF8.GetString(base64);
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => (m.Login == login));
            if (registreationData != null)
            {
                byte[] hash = RSACrypt.GeneratePasswordHash(password, registreationData.salt);
                if (Convert.ToBase64String(hash) != Convert.ToBase64String(registreationData.Password))
                {
                    registreationData = null;

                }
                else
                TempData[oKey] = null;
            }
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

        public async Task<IActionResult> CreateCard()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }
            return View();
        }


        [HttpPost, ActionName("CreateCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard([Bind("Name,Text,Date,GuestsNumMax,Place,UserId")] Card card )
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }
            card.UserId=Userid;
            if (ModelState.IsValid)
            {
                _context2.Add(card);
                await _context2.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        public async Task<IActionResult> MyCards()
        {
            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (_context2.Card == null)
                return Problem("Entity set 'SocialGatherKuznetsovContext.Card'  is null.");
            var cards = from m in _context2.Card
                        select m;
            if (true)
            {
                cards = cards.Where(s => s.UserId == registreationData.UserId);
            }
            return View(await cards.ToListAsync());
        }
        public async Task<IActionResult> MyCards2()
        {

            var Userid = Request.Cookies["UserId"];
            var registreationData = await _context.RegistreationData
                .FirstOrDefaultAsync(m => m.UserId == Userid);
            if (registreationData == null)
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (registreationData.token != Request.Cookies["token"])
            {
                return RedirectToAction("LoginPage", "RegistreationDatas");
            }

            if (_context2.Card == null)
                return Problem("Entity set 'SocialGatherKuznetsovContext.Card'  is null.");
            var cards = from m in _context2.Card
                        select m;
            if (true)
            {
                var code = cards.Where(s => s.GuestsList.Any(g => g.UserId == registreationData.UserId)).FirstOrDefault()?.GuestsList;
                cards = cards.Where(s => s.GuestsList.Any(g => g.UserId == registreationData.UserId));
            }
            return View(await cards.ToListAsync());
        }


    }
}
