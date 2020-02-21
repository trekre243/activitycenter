using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ActivityEvent.Models;

namespace ActivityEvent.Controllers
{
    public class ActController : Controller
    {   
        private MyContext dbContext;

        public ActController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("home")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return RedirectToAction("LoginReg", "User");
            }

            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)userId);
            ViewBag.UserId = (int)userId;
            ViewBag.Name = currentUser.Name;
            ViewBag.Acts = dbContext.Acts
                .Include(a => a.Participants)
                .Include (a => a.Coordinator)
                .Where(a => new DateTime(a.Date.Year, a.Date.Month, a.Date.Day, a.Time.Hour, a.Time.Minute, a.Time.Second) > DateTime.Now)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .ToList();

            return View();
        }

        [HttpGet("new")]
        public IActionResult NewAct()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("LoginReg", "User");
            }
            return View();
        }

        [HttpPost("activity")]
        public IActionResult Create(Act act)
        {
            if(ModelState.IsValid)
            {
                DateTime eventDate = new DateTime(act.Date.Year, act.Date.Month, act.Date.Day, act.Time.Hour, act.Time.Minute, act.Time.Second);
                if(eventDate < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Activity must occur in the future.");
                    return View("NewAct");
                }
                int? userId = HttpContext.Session.GetInt32("UserId");
                act.UserId = (int)userId;
                dbContext.Add(act);
                dbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("NewAct");
            }
        }

        [HttpGet("activity/{actId}")]
        public IActionResult SpecAct(int actId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return RedirectToAction("LoginReg", "User");
            }

            Act act = dbContext.Acts
                .Include(a => a.Participants)
                .ThenInclude(ap => ap.Participant)
                .Include(a => a.Coordinator)
                .FirstOrDefault(a => a.ActId == actId);

            if(act == null)
            {
                return RedirectToAction("Dashboard");
            }

            bool isParticipant = false;
            if(act.Participants.Any(ap => ap.Participant.UserId == (int)userId))
            {
                isParticipant = true;
            }

            ViewBag.UserId = userId;
            ViewBag.IsParticipant = isParticipant;
            ViewBag.Act = act;
            return View();
        }

        [HttpGet("join/{actId}")]
        public RedirectToActionResult Join(int actId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return RedirectToAction("LoginReg", "User");
            }

            ActPart actPart = new ActPart();
            actPart.UserId = (int)userId;
            actPart.ActId = actId;
            dbContext.Add(actPart);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{actId}")]
        public RedirectToActionResult Leave(int actId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return RedirectToAction("LoginReg", "User");
            }

            ActPart actPart = dbContext.ActParts.FirstOrDefault(ap => ap.UserId == (int)userId && ap.ActId == actId);
            if(actPart != null)
            {
                dbContext.Remove(actPart);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/delete/{actId}")]
        public RedirectToActionResult Delete(int actId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return RedirectToAction("LoginReg", "User");
            }

            Act act = dbContext.Acts
            .Include(a => a.Coordinator)
            .FirstOrDefault(a => a.ActId == actId);

            if(act == null || act.Coordinator.UserId != (int)userId)
            {
                return RedirectToAction("Dashboard");
            }

            dbContext.Remove(act);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");

        }

    }

    
}
