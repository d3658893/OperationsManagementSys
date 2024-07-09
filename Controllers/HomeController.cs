using OperationsManagementSystem.CustomAuthentication;
using OperationsManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OperationsManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private CustomRole role = new CustomRole();
        private OMSEntities dbContext = new OMSEntities();

        public ActionResult Index()
        {
            var Browser = Request.Browser.Browser;
            if (Browser == "InternetExplorer" || Browser == "IE")
            {
                return RedirectToAction("BrowserNotSupported", "Home");
            }

            string username = User.Identity.Name;
            bool isValidUser = Membership.ValidateUser(username, "");

            if (isValidUser && User.IsInRole("Initiater"))
            {
                return RedirectToAction("SearchTicket", "Initiater");
            }
            if (isValidUser && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (isValidUser && User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (isValidUser && User.IsInRole("Team"))
            {
                return RedirectToAction("Index", "Team");
            }
            if (isValidUser && User.IsInRole("InitiatorTeam"))
            {
                return RedirectToAction("TasksAssignedByManager", "Team");
            }
            if (isValidUser && User.IsInRole("InitiatorAdmin"))
            {
                return RedirectToAction("TaskAssignment", "Admin");
            }
            if (isValidUser && User.IsInRole("InitiatorTeamAdmin"))
            {
                return RedirectToAction("TaskAssignment", "Admin");
            }

            return View("UnAuthorize");
        }

        [HttpPost]
        public JsonResult GetUnassignedTicketsCount()
        {
            int count = dbContext.Tickets.Where(x => x.Status == TicketStatus.UnAssigned.ToString()).Count();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowTicketDetail(string id)
        {
            var tickets = dbContext.Tickets.Where(x => x.TicketNumber.Trim() == id.Trim()).ToList();
            return View(tickets);
        }

        public ActionResult UserFeedback(string id)
        {
            PopulateDropDowns();
            var ticket = dbContext.Tickets.Where(x => x.TicketNumber.Trim() == id.Trim()).FirstOrDefault();
            if(ticket.CreatedBy != User.Identity.Name)
            {
                return PartialView("_ErrorFeedback");
            }
            return View(ticket);
        }

        [HttpPost]
        public ActionResult UserFeedback(Ticket ticket)
        {
            ViewBag.ErrorMessage = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var oldTicket = dbContext.Tickets.Where(x => x.TicketNumber == ticket.TicketNumber).FirstOrDefault();
                    oldTicket.InitFeedback = ticket.InitFeedback;
                    oldTicket.InitComments = ticket.InitComments;
                    oldTicket.FeedbackDate = DateTime.Now;
                    dbContext.Set<Ticket>().AddOrUpdate(oldTicket);
                    dbContext.SaveChanges();
                    SendEmail_FeedbackSubmitted(oldTicket);
                    return PartialView("_FeedBackSubmitted");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.InnerException.Message;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Model State is not valid.";
            }
            PopulateDropDowns();
            return View(ticket);
        }

        public void PopulateDropDowns()
        {
            List<SelectListItem> feedback = new List<SelectListItem>() {
                    new SelectListItem {
                        Text = "Satisfied", Value = "Satisfied"
                    },
                    new SelectListItem {
                        Text = "Not Satisfied", Value = "Not Satisfied"
                    }
                        };
            ViewBag.UserFeedbackResult = feedback;
        }

        public ActionResult ShowTaskDetail(string id)
        {
            var task = dbContext.TaskAssignments.Where(x => x.TicketNo.Contains(id)).ToList();
            return View(task);
        }


        public void DownloadFile(int? ID)
        {
            string filePath = dbContext.Tickets.Where(x => x.ID == ID).Select(x => x.Attachment).FirstOrDefault();
            if (filePath != null)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                if (file.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(file.FullName);
                    Response.End();

                }
                else
                {
                    Response.Write("This file does not exist on server.");
                }
            }
            else
            {
                Response.Write("This file does not exist on server.");
            }
        }

        public void DownloadCompleterFile(int? ID)
        {
            string filePath = dbContext.Tickets.Where(x => x.ID == ID).Select(x => x.CompleterAttachment).FirstOrDefault();
            if (filePath != null)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                if (file.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(file.FullName);
                    Response.End();

                }
                else
                {
                    Response.Write("This file does not exist on server.");
                }
            }
            else
            {
                Response.Write("This file does not exist on server.");
            }
        }

        public void DownloadTaskFile(int? ID)
        {
            string filePath = dbContext.TaskAssignments.Where(x => x.TaskId == ID).Select(x => x.Attachment).FirstOrDefault();
            if (filePath != null)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                if (file.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(file.FullName);
                    Response.End();

                }
                else
                {
                    Response.Write("This file does not exist on server.");
                }
            }
            else
            {
                Response.Write("This file does not exist on server.");
            }
        }

        public void SendEmail_FeedbackSubmitted(Ticket ticket)
        {
            string cc = string.Empty;
            string subject = "Ticket Feedback Submitted";
            var completer = dbContext.Users.Where(x => x.Username == ticket.AssignedTo).FirstOrDefault();
            string body = string.Format("Dear Concern, \n \n Feedback submitted for ticket number {0} . Please check ticket details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber + " \n \n Thank you", ticket.TicketNumber);

            string To = completer.Email;
            cc = completer.TeamLead + ";awais.musaddiq@jazz.com.pk";

            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public ActionResult UnAuthorize()
        {
            return View();
        }

        public ActionResult BrowserNotSupported()
        {
            return View();
        }

        public ActionResult POCsList()
        {
            var pocList = dbContext.Users.Where(x => x.Team != "AutomationReporting").OrderBy(x => x.Team).ToList();
            return View(pocList);
        }

        #region Create New User with Role
        public ActionResult Admin()
        {
            ViewBag.CreateError = "";
            var adminUser = dbContext.Admins.Where(x => x.NTLogin == User.Identity.Name).FirstOrDefault();
            if(adminUser == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }        
        }

        [HttpPost]
        public ActionResult Admin(CreateUser user)
        {
            ViewBag.CreateError = "";
            User newUser = new User();

            if (ModelState.IsValid)
            {
                var existingUser = dbContext.Users.Where(x => x.Username == user.Username).FirstOrDefault();
                if (existingUser == null)
                {
                    newUser.Username = user.Username;
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Email = user.Email;
                    newUser.CreatedDate = DateTime.Now;
                    newUser.ModifiedDate = DateTime.Now;
                    newUser.Team = user.Team;
                    newUser.TeamLead = user.TeamLeadEmail;
                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                    int userId = newUser.EmpId;
                    dbContext.Database.ExecuteSqlCommand("INSERT INTO UserRoles VALUES({0},{1})", userId, 1); // RoleID 1 is Initiator
                    ViewBag.CreateError = "User Created Successfully.";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.CreateError = "Username already exists.";
                }
            }
            else
            {
                ViewBag.CreateError = "Error Occured!";
            }
            return View(user);
        }

        #endregion Create New User with Role

    }
}