using FinWorkGBMailBox.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinWorkGBMailBox.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}

