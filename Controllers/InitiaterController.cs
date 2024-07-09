using OperationsManagementSystem.CustomAuthentication;
using OperationsManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OperationsManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Initiater,InitiatorTeam,InitiatorAdmin,InitiatorTeamAdmin")]
    public class InitiaterController : Controller
    {
        private OMSEntities dbContext = new OMSEntities();
        private string DOMAIN = "@jazz.com.pk";

        // GET: Initiater       
        [HttpGet]
        public ActionResult SearchTicket()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchTicket(string TicketNumber)
        {
            var tickets = dbContext.Tickets.Where(x=>x.TicketNumber.Trim() == TicketNumber.Trim()).ToList();
            if (tickets != null && tickets.Count > 0)
            {
                if (tickets.Any(x=>x.Status == TicketStatus.UnAssigned.ToString()))
                {
                    return PartialView("_SearchUnassigned", tickets);
                }
            }
            return PartialView("_SearchByInitiator", tickets);
        }

        [HttpGet]
        public ActionResult SearchTicketByStatus()
        {
            var statusTypes = (dbContext.LkpStatusTypes.Select(x => new SelectListItem
            { Value = x.Status, Text = x.Status }).Distinct());
            ViewBag.StatusTypesList = statusTypes;
            return View();
        }

        [HttpPost]
        public ActionResult SearchTicketByStatus(string Status)
        {
            var tickets = dbContext.Tickets.Where(x => x.Status.ToUpper() == Status.ToUpper() && x.CreatedBy == User.Identity.Name).ToList();
            if (tickets != null)
            {
                if (tickets.Any(x=>x.Status == TicketStatus.UnAssigned.ToString()))
                {
                    return PartialView("_UnassignedTickets", tickets.OrderByDescending(x=>x.CreatedDate));
                }
            }
            return PartialView("_SearchByInitiator", tickets.OrderByDescending(x=>x.StatusDate));
        }

        [HttpGet]
        public ActionResult CreateProcessTicket()
        {
            PopulateProcessDropDowns();
            return View();
        }

        [HttpPost]
        public JsonResult ProcessNameAutoComplete(string prefix)
        {
            var namesList = dbContext.LkpProcessNames.Where(x => x.ProcessName.StartsWith(prefix)).Select(x => x.ProcessName);
            return Json(namesList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateProcessTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ProcessTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Change Request";
                    ticket.Type = ProcessType.Process.ToString();
                    ticket.TicketNumber = "CR" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Change Request", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ProcessTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ProcessTicketError = "Model State is not valid.";
            }
            PopulateProcessDropDowns();
            return View(ticket);
        }

        [HttpGet]
        public ActionResult CreateApplicationTicket()
        {
            PopulateApplicationDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateApplicationTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ApplicationTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Change Request";
                    ticket.Type = ProcessType.Application.ToString();
                    ticket.TicketNumber = "CR" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Change Request", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ApplicationTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ApplicationTicketError = "Model State is not valid.";
            }
            PopulateApplicationDropDowns();
            return View(ticket);
        }

        [HttpGet]
        public ActionResult CreateReportTicket()
        {
            PopulateReportDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateReportTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ReportTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Change Request";
                    ticket.Type = ProcessType.Report.ToString();
                    ticket.TicketNumber = "CR" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Change Request", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ReportTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ReportTicketError = "Model State is not valid.";
            }
            PopulateReportDropDowns();
            return View(ticket);
        }

        public ActionResult CreateChangeRequest()
        {
            PopulateTypes();
            return View();
        }

        [HttpGet]
        public ActionResult CreateNewReportTicket()
        {
            PopulateNewReportDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewReportTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.NewReportTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                 && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "New Report";
                    ticket.Type = ProcessType.Report.ToString();
                    ticket.TicketNumber = "NR" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("New Report", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.NewReportTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.NewReportTicketError = "Model State is not valid.";
            }
            PopulateNewReportDropDowns();
            return View(ticket);
        }

        public void PopulateTypes()
        {
            var listItems = (dbContext.LkpTypes.Select(x => new SelectListItem
            { Value = x.TypeName, Text = x.TypeName }).Distinct());
            ViewBag.listTypes = listItems;
        }

        public void PopulateProcessDropDowns()
        {
            var processNames = (dbContext.LkpProcessNames.Select(x => new SelectListItem
            { Value = x.ProcessName, Text = x.ProcessName }).Distinct());
            ViewBag.ProcessNamesList = processNames;

            var teamName = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x=>x.Team).FirstOrDefault();
            ViewBag.UserTeamName = teamName;
        }

        public void PopulateNewReportDropDowns()
        {
            var reportTypes = (dbContext.LkpReportTypes.Select(x => new SelectListItem
            { Value = x.ReportType, Text = x.ReportType }).Distinct());
            ViewBag.ReportTypesList = reportTypes;

            var teamName = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x => x.Team).FirstOrDefault();
            ViewBag.UserTeamName = teamName;
        }

        public void PopulateApplicationDropDowns()
        {
            var appNames = (dbContext.LkpApplicationNames.Select(x => new SelectListItem
            { Value = x.ApplicationName, Text = x.ApplicationName }).Distinct());
            ViewBag.ApplicationNamesList = appNames;

            var teamName = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x => x.Team).FirstOrDefault();
            ViewBag.UserTeamName = teamName;
        }

        public void PopulateReportDropDowns()
        {
            var teamName = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x => x.Team).FirstOrDefault();
            ViewBag.UserTeamName = teamName;
        }

        public ActionResult ReportABug()
        {
            PopulateTypes();
            return View();
        }

        [HttpGet]
        public ActionResult CreateProcessBugTicket()
        {
            PopulateProcessDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateProcessBugTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ProcessBugTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Report a Bug";
                    ticket.Type = ProcessType.Process.ToString();
                    ticket.TicketNumber = "RaB/" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Report a Bug", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ProcessBugTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ProcessBugTicketError = "Model State is not valid.";
            }
            PopulateProcessDropDowns();
            return View(ticket);
        }

        [HttpGet]
        public ActionResult CreateApplicationBugTicket()
        {
            PopulateApplicationDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateApplicationBugTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ApplicationBugTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Report a Bug";
                    ticket.Type = ProcessType.Application.ToString();
                    ticket.TicketNumber = "RaB" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Report a Bug", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ApplicationBugTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ApplicationBugTicketError = "Model State is not valid.";
            }
            PopulateApplicationDropDowns();
            return View(ticket);
        }

        [HttpGet]
        public ActionResult CreateReportBugTicket()
        {
            PopulateReportDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult CreateReportBugTicket(Ticket ticket, HttpPostedFileBase Attachment)
        {
            ViewBag.ReportBugTicketError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    #region Save Uploaded File
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + Attachment.FileName);
                        ticket.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Tciket
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.Tickets.Where(x => x.CreatedDate.Value.Year == todayDate.Year && x.CreatedDate.Value.Month == todayDate.Month
                                                                && x.CreatedDate.Value.Day == todayDate.Day).Count() + 1;
                    ticket.CreatedDate = DateTime.Now;
                    ticket.DateOfInitiation = DateTime.Now;
                    ticket.CreatedBy = currentUser;
                    ticket.TicketType = "Report a Bug";
                    ticket.Type = ProcessType.Report.ToString();
                    ticket.TicketNumber = "RaB" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    ticket.Status = TicketStatus.UnAssigned.ToString();
                    dbContext.Tickets.Add(ticket);
                    dbContext.SaveChanges();
                    SendEmail_StatusUnassigned("Report a Bug", ticket);
                    return PartialView("_SuccessMessage", ticket);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.ReportBugTicketError = ex.Message;
                }
            }
            else
            {
                ViewBag.ReportBugTicketError = "Model State is not valid.";
            }
            PopulateReportDropDowns();
            return View(ticket);
        }

        public ActionResult Feedback()
        {
            var tickets = dbContext.Tickets.Where(x => x.CreatedBy == User.Identity.Name && x.Status == "Completed" && String.IsNullOrEmpty(x.InitFeedback) && string.IsNullOrEmpty(x.InitComments))
                                            .OrderByDescending(x=>x.StatusDate).ToList();
            return View(tickets);
        }

        public void SendEmail_StatusUnassigned(string ticketType, Ticket ticket)
        {
            string cc = string.Empty;
            string subject = ticketType + " Ticket Created";
            var initiator = dbContext.Users.Where(x => x.Username == ticket.CreatedBy).FirstOrDefault();
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been received. Please assign to team through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber+ " \n \n Thank you \n \n", ticket.TicketNumber);
            
            string To = "awais.musaddiq@jazz.com.pk;moazzam.zaka@jazz.com.pk;ahmad.ali@jazz.com.pk";
            if (initiator.TeamLead != null)
            {
                cc = initiator.Email + ";" + initiator.TeamLead;
            }
            else if(initiator.TeamLead == null && initiator.HOD != null)
            {
                cc = initiator.Email + ";" + initiator.HOD;
            }
            else
            {
                cc = initiator.Email;
            }
                 
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }
    }
}