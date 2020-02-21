using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using ActivityEvent.Models;

namespace ActivityEvent.Controllers
{
    public class UserController : Controller
    {   
        private MyContext dbContext;

        public UserController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult LoginReg()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Act");
            }
            return View();
        }

        [HttpPost("user")]
        public IActionResult Create(User newUser)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                if(userInDb != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("LoginReg");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();

                int userId = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email).UserId;

                HttpContext.Session.SetInt32("UserId", userId);

                return RedirectToAction("Dashboard", "Act");
            }
            else
            {
                return View("LoginReg");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
             if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);

                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("LoginReg");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("LoginReg");
                }

                HttpContext.Session.SetInt32("UserId", userInDb.UserId);

                return RedirectToAction("Dashboard", "Act");
            }
            else
            {
                return View("LoginReg");
            }
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginReg");
        }

    }
}
