using FinWorkGBMailBox.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FinWorkGBMailBox.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password) 
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Messages");
            }

            ViewBag.ErrorMessage = "Неверная попытка входа.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Register(string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (password.Length <= 5 || !password.Any(char.IsUpper)) 
            {
                ViewBag.ErrorMessage = "Некорректный пароль. Пароль должен быть больше 5 символов и с 1 заглавной буквой.";
                return View("Login");
            }

            if (existingUser == null)
            {
                var newUser = new User { Email = email, Password = password };
                if (!_context.Users.Any())
                {
                    newUser.Role = "Admin";
                }
                else
                {
                    newUser.Role = "Standard";
                }
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                ViewBag.SuccessMessage = "Регистрация прошла успешно. Теперь вы можете войти.";
                return View("Login");
            }

            ViewBag.ErrorMessage = "Пользователь с таким email уже существует.";
            return View();
        }

        public IActionResult Messages()
        {
            // Получите все сообщения для пользователя
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messages = _context.Messages
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.DateTime)
                .ToList();

            // Передайте модель в представление
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowAllUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userToDelete = await _context.Users.FindAsync(id);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userToDelete != null && userToDelete.Id.ToString() == currentUserId)
            {
                ViewBag.ErrorMessage = "Вы не можете удалить самого себя.";
                return View();
            }


                if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ShowAllUsers");
        }

        [HttpPost]
        public IActionResult SendMessage(string receiverId, string content)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Создать новое сообщение
            var message = new Message
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = content,
                DateTime = DateTime.Now,
                IsRead = false
            };

            // Сохранить сообщение в базе данных
            _context.Messages.Add(message);
            _context.SaveChanges();


            // Получить все непрочитанные сообщения для пользователя
            var messages = _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .OrderByDescending(m => m.DateTime)
                .ToList();


            ViewBag.Messages = messages;
            ViewBag.SuccessMessage = "Сообщение успешно отправлено.";

            return RedirectToAction("Messages");
        }

    }
}
