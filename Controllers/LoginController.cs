using DreamAI.Context;
using DreamAI.Dtos;
using DreamAI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DreamAI.Controllers
{
    public class LoginController : Controller
    {
        private readonly DreamDbContext _context;
        private readonly SignInManager<User> _signIn;
        private readonly UserManager<User> _userManager;

        public LoginController(DreamDbContext context, SignInManager<User> signIn, UserManager<User> userManager)
        {
            _context = context;
            _signIn = signIn;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserDto user,CancellationToken cancellationToken)
        {
            var result = await _signIn.PasswordSignInAsync(user.UserName, user.Password, isPersistent: false, lockoutOnFailure: false);
            var userIn = await _userManager.FindByNameAsync(user.UserName);
            if (result.Succeeded)
            {
                // kullanıcı giriş yapabilir
                return RedirectToAction("New", "Dream", new
                {
                    id = userIn.Id
                });
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(userDto);
            var isUserNameAvailable = await _userManager.FindByNameAsync(userDto.UserName);
            if (isUserNameAvailable != null)
            {
                ModelState.AddModelError("", "this user name hass been used");
                return View(userDto);
            }
            User user = new User();
            
            user.UserName = userDto.UserName;
            
            var result =await _userManager.CreateAsync(user,userDto.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userDto);
            }
            //başarıllı kayıt yapıldı yazısı
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
