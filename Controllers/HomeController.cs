using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DojoCenter.Models;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace DojoCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context){
            dbContext = context;
        }

       [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid){
                
                User userMatchingEmail = dbContext.Users
                    .FirstOrDefault(u => u.Email == user.Email);
                if(userMatchingEmail != null){
                    ModelState.AddModelError("Email", "Email already in use!");
                }
                else{
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("userid", user.UserId);
                    return Redirect("/dashboard");
                }
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid){
                User userMatchingEmail = dbContext.Users
                    .FirstOrDefault(u => u.Email == user.LoginEmail);
                if(userMatchingEmail == null){
                    ModelState.AddModelError("LoginEmail", "Unknown Email!");
                }
                else{
                    PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                    var result = Hasher.VerifyHashedPassword(user, userMatchingEmail.Password, user.LoginPassword);
                    if(result == 0){
                        ModelState.AddModelError("LoginPassword", "Incorrect Password!");
                    } else {
                        HttpContext.Session.SetInt32("userid", userMatchingEmail.UserId);
                        return Redirect("/dashboard");
                    }
                }
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userid");
            return Redirect("/");
        }




        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? userid = HttpContext.Session.GetInt32("userid");
            if(userid == null)
            {
                return Redirect("/");
            } 
            if(ModelState.IsValid){
                List<Party> Parties = dbContext.Parties
                    .Include(p => p.Planner)
                    .Include(p => p.Users)
                    .OrderBy(p => p.Date)
                    .ThenBy(p => p.Time)
                    .ToList();

                removePast(Parties);
                ViewBag.Parties = Parties;
                ViewBag.User = dbContext.Users.FirstOrDefault(u=> u.UserId == userid);
                ViewBag.UserId = (int)userid;
                return View("Dashboard");
            }
            return Redirect("/");
        
        }

      
        [HttpGet("Party/{PartyId}")]
        public IActionResult Party(int PartyId)
        {
            int? userid = HttpContext.Session.GetInt32("userid");
            User current= dbContext.Users.FirstOrDefault(u => u.UserId == userid);
            if (userid == null)
            {
                return Redirect("/");
            }
            Party currentParty = dbContext.Parties
            .Include(f => f.Planner)
            .FirstOrDefault(w => w.PartyId == PartyId);
            ViewBag.Party = currentParty;
            // List <User> guests = dbContext.Users
            // .Where(u => u.Parties.All(w => w.PartyId == PartyId))
            // .ToList();
            // ViewBag.Guests = guests;
           Party Atendees = dbContext.Parties
            .Include(f => f.Users)
            .ThenInclude(p => p.User)
            .FirstOrDefault(f => f.PartyId == PartyId);
            ViewBag.Attendees = Atendees;
            ViewBag.User = dbContext.Users.FirstOrDefault(u=> u.UserId == userid);
            ViewBag.UserId = (int)userid;
            return View();
        }


        [HttpGet("NewParty")]
        public IActionResult NewPartyPage(){
            int? userid = HttpContext.Session.GetInt32("userid");
            if (userid == null)
            {
                return Redirect("/");
            }
            return View("NewParty");
        }

        [HttpPost("/create")]
        public IActionResult NewParty(Party party){
            int? userid = HttpContext.Session.GetInt32("userid");
            if (userid == null)
            {
                return Redirect("/");
            }

            if(ModelState.IsValid){
                // int? userid = HttpContext.Session.GetInt32("userid");
                User current= dbContext.Users.FirstOrDefault(u => u.UserId == userid);
                dbContext.Parties.Add(party);
                party.Planner = current;
                party.PlannerId = (int)userid;
                dbContext.SaveChanges();
                // return Redirect($"Party/{party.PartyId}");
                return Redirect("Party/"+party.PartyId.ToString());
            }
           return View("NewParty", party);
        }


        [HttpGet("join/{PartyId}")]
        public IActionResult Join(int PartyId){
            int? userid = HttpContext.Session.GetInt32("userid");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userid);
            Party currentParty = dbContext.Parties.FirstOrDefault(w => w.PartyId == PartyId);

            Association newAssociation = new Association()
            {
                PartyId = PartyId,
                UserId = (int) userid
            };
            // newAssociation.User = currentUser;
            // newAssociation.Party = currentParty;
            // newAssociation.UserId = currentUser.UserId;
            // newAssociation.PartyId = currentParty.PartyId;
            dbContext.Associations.Add(newAssociation);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{PartyId}")]
        public IActionResult Leave(int PartyId){
            int? userid = HttpContext.Session.GetInt32("userid");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userid);
            Party currentParty = dbContext.Parties.FirstOrDefault(w => w.PartyId == PartyId);

            Association assoc = dbContext.Associations
            .Where(w => w.PartyId == PartyId)
            .FirstOrDefault(u => u.UserId == userid);
            dbContext.Associations.Remove(assoc);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        [HttpGet("delete/{PartyId}")]
        public IActionResult delete(int PartyId){
            Party currentParty = dbContext.Parties.FirstOrDefault(w => w.PartyId == PartyId);
            dbContext.Parties.Remove(currentParty);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

         public void removePast(List <Party> AllParties){
            DateTime now = DateTime.Now;
            foreach(var p in AllParties){
                if(now > p.Date){
                    dbContext.Parties.Remove(p);
                }
            }
        }

    }
}