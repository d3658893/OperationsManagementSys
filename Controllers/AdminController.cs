using OperationsManagementSystem.CustomAuthentication;
using OperationsManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace OperationsManagementSystem.Controllers
{

    [Authorize(Roles = "Admin,SuperAdmin,InitiatorAdmin,InitiatorTeamAdmin")]
    public class AdminController : Controller
    {
        private OMSEntities dbContext = new OMSEntities();
        private string DOMAIN = "@jazz.com.pk";

        // GET: Admin      
        public ActionResult Index()
        {
            PopulateTilesSummary();
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult Index(SearchTicket ticket)
        {
            ViewBag.SearchMessage = "";
            ViewBag.SearchResult = "";
            List<Ticket> tickets = new List<Ticket>();

            if (ticket.TicketType != null && ticket.AssignedTo != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus) && x.TicketType.Equals(ticket.TicketType)
                                                     && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                     && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                     && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.TicketType != null && ticket.AssignedTo != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && x.Status == ticket.TicketStatus && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.TicketType != null && ticket.AssignedTo != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && x.Status == ticket.TicketStatus && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.TicketType != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType) && x.Status == ticket.TicketStatus
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)).ToList();
            }
            else if (ticket.AssignedTo != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.TicketType != null && ticket.AssignedTo != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType) && x.Status == ticket.TicketStatus
                                                       && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.TicketType != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType) && x.Status == ticket.TicketStatus
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)).ToList();
            }
            else if (ticket.TicketType != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType) && x.Status == ticket.TicketStatus
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)).ToList();
            }
            else if (ticket.AssignedTo != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus) && x.AssignedTo == ticket.AssignedTo
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)).ToList();
            }
            else if (ticket.AssignedTo != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus) && x.AssignedTo == ticket.AssignedTo
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)).ToList();
            }
            else if (ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                       && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                       && x.Status == ticket.TicketStatus).ToList();
            }
            else if (ticket.TicketType != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType == ticket.TicketType && x.Status == ticket.TicketStatus).ToList();
            }
            else if (ticket.AssignedTo != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status == ticket.TicketStatus && x.AssignedTo == ticket.AssignedTo).ToList();
            }
            else if (ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                        && x.Status == ticket.TicketStatus).ToList();
            }
            else if (ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                          && x.Status == ticket.TicketStatus).ToList();
            }
            else
            {
                tickets = dbContext.Tickets.Where(x => x.Status == ticket.TicketStatus).ToList();
            }

            if (tickets == null || tickets.Count == 0)
            {
                ViewBag.SearchMessage = "No Record Found";
            }

            if (tickets.Any(x => x.Status == TicketStatus.UnAssigned.ToString()))
            {
                return PartialView("_SearchAdminUnassigned", tickets.OrderByDescending(x => x.CreatedDate));
            }
            if (tickets.Any(x => x.Status == TicketStatus.Open.ToString()) || tickets.Any(x => x.Status == TicketStatus.Hold.ToString()))
            {
                return PartialView("_OpenTicketsByAdmin", tickets.OrderByDescending(x => x.CreatedDate));
            }

            return PartialView("_SearchAdmin", tickets.OrderByDescending(x => x.StatusDate));
        }

        [HttpGet]
        public ActionResult SearchTicket()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchTicket(string TicketNumber)
        {
            var tickets = dbContext.Tickets.Where(x => x.TicketNumber.Trim() == TicketNumber.Trim()).ToList();
            if (tickets.Any(x => x.Status == TicketStatus.UnAssigned.ToString()))
            {
                return PartialView("_SearchAdminUnassigned", tickets.OrderByDescending(x => x.CreatedDate));
            }
            if (tickets.Any(x => x.Status == TicketStatus.Open.ToString()) || tickets.Any(x => x.Status == TicketStatus.Hold.ToString()))
            {
                return PartialView("_OpenTicketsByAdmin", tickets.OrderByDescending(x => x.CreatedDate));
            }

            return PartialView("_SearchAdmin", tickets.OrderByDescending(x => x.StatusDate));
        }

        public ActionResult ShowTicketDetails(int ID)
        {
            Ticket model = new Ticket();
            PopulateDropDowns();

            if (ID > 0)
            {
                Ticket ticket = dbContext.Tickets.SingleOrDefault(x => x.ID == ID);
                model.ID = ticket.ID;
                model.Name = ticket.Name;
                model.Status = ticket.Status;
                model.TicketNumber = ticket.TicketNumber;
                model.TicketType = ticket.TicketType;
                model.Type = ticket.Type;
                model.TypeOfReport = ticket.TypeOfReport;
                model.DateOfInitiation = ticket.DateOfInitiation;
                model.Description = ticket.Description;
                model.Team = ticket.Team;
                model.Attachment = ticket.Attachment;
                model.CreatedBy = ticket.CreatedBy;
                model.CreatedDate = ticket.CreatedDate;
                model.AssignedBy = ticket.AssignedBy;
                model.StatusDate = ticket.StatusDate;
                model.CompleterAttachment = ticket.CompleterAttachment;
            }

            return PartialView("_AssignTicket", model);
        }

        public ActionResult ReassignTicketDetails(int ID)
        {
            Ticket model = new Ticket();
            PopulateDropDowns();

            if (ID > 0)
            {
                Ticket ticket = dbContext.Tickets.SingleOrDefault(x => x.ID == ID);
                model.ID = ticket.ID;
                model.Name = ticket.Name;
                model.Status = ticket.Status;
                model.TicketNumber = ticket.TicketNumber;
                model.TicketType = ticket.TicketType;
                model.Type = ticket.Type;
                model.TypeOfReport = ticket.TypeOfReport;
                model.DateOfInitiation = ticket.DateOfInitiation;
                model.Description = ticket.Description;
                model.Team = ticket.Team;
                model.Attachment = ticket.Attachment;
                model.CreatedBy = ticket.CreatedBy;
                model.CreatedDate = ticket.CreatedDate;
                model.AssignedBy = ticket.AssignedBy;
                model.StatusDate = ticket.StatusDate;
                model.CompleterAttachment = ticket.CompleterAttachment;
            }

            return PartialView("_ReassignTicket", model);
        }

        public ActionResult UpdateTicket(Ticket ticket)
        {
            DateTime dueDate = Convert.ToDateTime(ticket.DueDate).AddDays(1).AddSeconds(-1);
            ticket.DueDate = dueDate;
            ticket.AssignedDate = DateTime.Now;
            ticket.AssignedBy = User.Identity.Name;
            ticket.Status = TicketStatus.Open.ToString();
            ticket.StatusDate = DateTime.Now;
            dbContext.Set<Ticket>().AddOrUpdate(ticket);
            dbContext.SaveChanges();
            SendEmail_OpenStatus(ticket);
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ReassignTicket(Ticket ticket)
        {
            string previousUserEmail = dbContext.Tickets.Where(x => x.ID == ticket.ID).FirstOrDefault().AssignedTo;
            DateTime dueDate = Convert.ToDateTime(ticket.DueDate).AddDays(1).AddSeconds(-1);
            ticket.DueDate = dueDate;
            ticket.AssignedDate = DateTime.Now;
            ticket.AssignedBy = User.Identity.Name;
            ticket.Status = TicketStatus.Open.ToString();
            ticket.StatusDate = DateTime.Now;
            dbContext.Set<Ticket>().AddOrUpdate(ticket);
            dbContext.SaveChanges();
            SendEmail_ReassignOpenStatus(ticket, previousUserEmail);
            return RedirectToAction("Index", "Admin");
        }

        public void PopulateTilesSummary()
        {
            var totalTickets = dbContext.Tickets.ToList();
            ViewBag.UnAssignedTickets = totalTickets.Where(x => x.Status == "UnAssigned").Count();
            ViewBag.OpenTickets = totalTickets.Where(x => x.Status == "Open").Count();
            ViewBag.HoldTickets = totalTickets.Where(x => x.Status == "Hold").Count();
            ViewBag.CompletedTickets = totalTickets.Where(x => x.Status == "Completed").Count();
            ViewBag.TotalTickets = totalTickets.Count();

            var ticketType = (dbContext.LkpTicketTypes.Select(x => new SelectListItem
            { Value = x.TicketType, Text = x.TicketType }).Distinct().ToList());
            ViewBag.TicketTypes = ticketType;

            var statusType = (dbContext.LkpStatusTypes.Select(x => new SelectListItem
            { Value = x.Status, Text = x.Status }).Distinct().OrderByDescending(x => x.Value).ToList());
            ViewBag.StatusTypes = statusType;
        }

        [HttpGet]
        public ActionResult TaskAssignment()
        {
            string team = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x => x.Team).FirstOrDefault();
            var totalTasks =
                        from task in dbContext.TaskAssignments
                        join user in dbContext.Users on task.AssignedBy equals user.Username
                        where user.Team == team && task.AssignedBy == User.Identity.Name
                        select task;
            ViewBag.NotAcceptedTasks = totalTasks.Where(x => x.Status == "Not Accepted").Count();
            ViewBag.AcceptedTasks = totalTasks.Where(x => x.Status == "Accepted").Count();
            ViewBag.CompletedTasks = totalTasks.Where(x => x.Status == "Completed").Count();
            ViewBag.ExtensionRequestedTasks = totalTasks.Where(x => x.Status == "Extension Requested").Count();
            ViewBag.ExtensionAcknowledgedTasks = totalTasks.Where(x => x.Status == "Extension Acknowledged").Count();
            ViewBag.ExtensionRejectedTasks = totalTasks.Where(x => x.Status == "Extension Rejected").Count();

            PopulateTaskDropdowns();

            return View();
        }

        [HttpPost]
        public ActionResult TaskAssignment(LkpTaskStatu status)
        {
            ViewBag.SearchMessage = "";
            // List<TaskAssignment> tasks = new List<TaskAssignment>();
            string team = dbContext.Users.Where(x => x.Username == User.Identity.Name).Select(x => x.Team).FirstOrDefault();
            var tasks =
                        from task in dbContext.TaskAssignments
                        join user in dbContext.Users on task.AssignedBy equals user.Username
                        where user.Team == team && task.AssignedBy == User.Identity.Name && task.Status.ToUpper() == status.Name.ToUpper()
                        select task;
            tasks = tasks.OrderByDescending(x => x.StatusDate);
            //tasks = dbContext.TaskAssignments.Where(x => x.Status.ToUpper() == status.Name.ToUpper()).OrderByDescending(x => x.StatusDate).ToList();
            if (tasks == null || tasks.Count() == 0)
                ViewBag.SearchMessage = "No Record Found";
            if (tasks.Any(x => x.Status == "Not Accepted"))
            {
                return PartialView("_TasksNotAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Accepted"))
            {
                return PartialView("_TasksAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Completed"))
            {
                return PartialView("_TasksCompleted", tasks);
            }
            if (tasks.Any(x => x.Status == "Extension Requested"))
            {
                return PartialView("_TasksExtensionRequired", tasks);
            }

            return PartialView("_TasksExtensionResponse", tasks);
        }

        [HttpGet]
        public ActionResult CreateNewTask()
        {
            PopulateDropDowns();
            return View();
        }
        [HttpGet]
        public ActionResult CreateUser()
        {
            var model = dbContext.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                var loggedInUser = User.Identity.Name.Split('\\')[1] + DOMAIN;
                var team = (dbContext.Users.Where(x =>x.Team!=null).Select(x => new SelectListItem
                { Value = x.Team, Text = x.Team }).Distinct().OrderBy(x => x.Value).ToList());
                ViewBag.Teams = team;
                var Roles = (dbContext.Roles.Where(x => x.RoleName != null).Select(x => new SelectListItem
                { Value = Convert.ToString(x.RoleId), Text = x.RoleName }).Distinct().OrderBy(x => x.Value).ToList());
                ViewBag.Roles = Roles;
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateNewTask(TaskAssignment task, HttpPostedFileBase Attachment)
        {
            ViewBag.NewTaskError = "";
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
                        task.Attachment = fullPath;
                        Attachment.SaveAs(fullPath);
                    }
                    #endregion

                    #region Save Task
                    DateTime dueDate = Convert.ToDateTime(task.DueDate).AddDays(1).AddSeconds(-1);
                    task.DueDate = dueDate;
                    DateTime todayDate = DateTime.Now;
                    int rowCount = dbContext.TaskAssignments.Where(x => x.AssignedDate.Value.Year == todayDate.Year && x.AssignedDate.Value.Month == todayDate.Month
                                                                && x.AssignedDate.Value.Day == todayDate.Day).Count() + 1;
                    task.AssignedBy = currentUser;
                    task.AssignedDate = DateTime.Now;
                    task.Status = "Not Accepted";
                    task.StatusDate = DateTime.Now;
                    task.TicketNo = "T" + todayDate.ToString("ddMMyy") + "/" + rowCount;
                    dbContext.TaskAssignments.Add(task);
                    dbContext.SaveChanges();
                    SendEmail_NotAcceptedStatus(task);
                    return PartialView("_SuccessTask", task);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.NewTaskError = ex.Message;
                }
            }
            else
            {
                ViewBag.NewTaskError = "Model State is not valid.";
            }

            return View(task);
        }
        [HttpPost]
        public ActionResult CreateNewUser(User user)
        {
            //Userrol model = new UserRoles();
            ViewBag.NewTaskError = "";
            if (ModelState.IsValid)
            {
                try
                {
                    string currentUser = User.Identity.Name;
                    //UserRole role = new UserRole();

                    //UserRole  model = new userRoles();
                    #region Save User
                    user.CreatedBy = currentUser;
                    user.CreatedDate = DateTime.Now;
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    //dbContext.UserRoles1.
                    //dbContext.UserRoles1.Add(user.Roles);
                    dbContext.SaveChanges();
                    //SendEmail_NotAcceptedStatus(task);
                    return PartialView("_SuccessUser");
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.NewTaskError = ex.Message;
                }
            }
            else
            {
                ViewBag.NewTaskError = "Model State is not valid.";
            }

            return View(user);
        }

        public void PopulateDropDowns()
        {
            var model = dbContext.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("Admin") || User.IsInRole("InitiatorAdmin") || User.IsInRole("InitiatorTeamAdmin"))
            {
                if (model.TeamLead != null)
                {
                    var loggedInUser = User.Identity.Name.Split('\\')[1] + DOMAIN;
                    var users = dbContext.Users.Where(x => x.TeamLead == loggedInUser && x.Team == model.Team).Select(x => new SelectListItem
                    { Value = x.Username, Text = (x.FirstName + " " + x.LastName) }).Distinct().ToList();
                    ViewBag.MembersList = users;
                }
                if (model.TeamLead == null && model.HOD != null)
                {
                    var loggedInUser = User.Identity.Name.Split('\\')[1] + DOMAIN;
                    var users = dbContext.Users.Where(x => x.TeamLead == loggedInUser && x.Team == model.Team).Select(x => new SelectListItem
                    { Value = x.Username, Text = (x.FirstName + " " + x.LastName) }).Distinct().ToList();
                    ViewBag.MembersList = users;
                }
                if (model.TeamLead == null && model.HOD == null)
                {
                    var currentUser = User.Identity.Name.Split('\\')[1] + DOMAIN;
                    var usersList = dbContext.Users.Where(x => x.HOD == currentUser && x.Team == model.Team).Select(x => new SelectListItem
                    { Value = x.Username, Text = (x.FirstName + " " + x.LastName) }).Distinct().ToList();
                    ViewBag.MembersList = usersList;
                }
                if (model.TeamLead != null && User.IsInRole("Admin"))
                {
                    var teamMembers = (dbContext.Users.Where(x => x.Username != User.Identity.Name && x.Team == model.Team).Select(x => new SelectListItem
                    { Value = x.Username, Text = (x.FirstName + " " + x.LastName) }).Distinct().ToList());
                    ViewBag.MembersList = teamMembers;
                }
            }
            if (User.IsInRole("SuperAdmin"))
            {
                var teamMembers = (dbContext.Users.Where(x => x.IsActive != true).Select(x => new SelectListItem
                { Value = x.Username, Text = (x.FirstName + " " + x.LastName) }).Distinct().ToList());
                ViewBag.MembersList = teamMembers;
            }

        }

        public ActionResult UpdateTaskStatus(int? ID)
        {
            TaskAssignment model = new TaskAssignment();
            List<SelectListItem> extensionStatus = new List<SelectListItem>()
            {
              new SelectListItem {Text="Extension Acknowledged",Value="Extension Acknowledged",Selected=true },
              new SelectListItem {Text="Extension Rejected",Value="Extension Rejected" },
            };

            ViewBag.ExtensionRequestStatus = extensionStatus;

            if (ID > 0)
            {
                TaskAssignment task = dbContext.TaskAssignments.SingleOrDefault(x => x.TaskId == ID);
                model.TaskId = task.TaskId;
                model.AssignedBy = task.AssignedBy;
                model.AssignedTo = task.AssignedTo;
                model.AssignedDate = task.AssignedDate;
                model.Attachment = task.Attachment;
                model.Counter = task.Counter;
                model.Description = task.Description;
                model.DueDate = task.DueDate;
                model.EmployeeComments = task.EmployeeComments;
                model.ManagerComments = task.ManagerComments;
                model.Status = task.Status;
                model.StatusDate = task.StatusDate;
                model.ExtensionDate = task.ExtensionDate;
                model.TicketNo = task.TicketNo;
                model.IsExtensionRequested = task.IsExtensionRequested;
                model.ExtensionStatus = task.ExtensionStatus;
            }

            return PartialView("_UpdateExtensionRequestStatus", model);
        }

        public ActionResult UpdateExtensionRequestStatus(TaskAssignment task)
        {
            if (task.Status == "Extension Acknowledged")
            {
                DateTime dueDate = Convert.ToDateTime(task.DueDate).AddDays(1).AddSeconds(-1);
                task.DueDate = dueDate;
                task.ExtensionStatus = "Acknowledged";
            }
            else
            {
                task.ExtensionStatus = "Rejected";
            }
            task.StatusDate = DateTime.Now;
            dbContext.Set<TaskAssignment>().AddOrUpdate(task);
            dbContext.SaveChanges();
            if (task.Status == "Extension Acknowledged")
            {
                SendEmail_ExtensionAcknowledgedStatus(task);
            }
            if (task.Status == "Extension Rejected")
            {
                SendEmail_ExtensionRejectedStatus(task);
            }
            return RedirectToAction("TaskAssignment", "Admin");
        }

        [HttpGet]
        public ActionResult SearchTaskByTicket()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchTaskByTicket(string TicketNumber)
        {
            OMSEntities db = new OMSEntities();
            var tasks = db.TaskAssignments.Where(y => y.TicketNo == TicketNumber).ToList();
            if (tasks.Any(x => x.Status == "Not Accepted"))
            {
                return PartialView("_TasksNotAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Accepted"))
            {
                return PartialView("_TasksAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Completed"))
            {
                return PartialView("_TasksCompleted", tasks);
            }
            if (tasks.Any(x => x.Status == "Extension Requested"))
            {
                return PartialView("_TasksExtensionRequired", tasks);
            }

            return PartialView("_TasksExtensionResponse", tasks);

        }

        public void SendEmail_OpenStatus(Ticket ticket)
        {
            string cc = string.Empty;
            var model = dbContext.Users.Where(x => x.Username.Equals(ticket.AssignedTo)).FirstOrDefault();
            var initiator = dbContext.Users.Where(x => x.Username.Equals(ticket.CreatedBy)).FirstOrDefault();
            string subject = ticket.TicketType + " Ticket Assigned";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been assigned to you with due date {1} . You can check the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber + "\n \n Thank you", ticket.TicketNumber, ticket.DueDate);
            string To = model.Email;
            if (model.TeamLead != null)
            {
                cc = model.TeamLead + ";" + initiator.Email;
            }
            else if (model.TeamLead == null && model.HOD != null)
            {
                cc = model.HOD + ";" + initiator.Email;
            }
            else
            {
                cc = initiator.Email;
            }
            //if(model.TeamLead == "moazzam.zaka@jazz.com.pk")
            //{
            //    cc = cc + ";" + "CE-DEVELOPMENTTEAM@jazz.com.pk";
            //}
            //if (model.TeamLead == "ahmad.ali@jazz.com.pk")
            //{
            //    cc = cc + ";" + "CC-REPORTINGTEAM@jazz.com.pk";
            //}
            if (initiator.TeamLead != null)
            {
                cc = cc + ";" + initiator.TeamLead;
            }
            if (initiator.TeamLead == null && initiator.HOD != null)
            {
                cc = cc + ";" + initiator.HOD;
            }
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_ReassignOpenStatus(Ticket ticket, string previousUserEmail)
        {
            string cc = string.Empty;
            previousUserEmail = previousUserEmail.Split('\\')[1] + DOMAIN;
            var model = dbContext.Users.Where(x => x.Username.Equals(ticket.AssignedTo)).FirstOrDefault();
            var initiator = dbContext.Users.Where(x => x.Username.Equals(ticket.CreatedBy)).FirstOrDefault();
            string subject = ticket.TicketType + " Ticket Assigned";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been assigned to you with due date {1} . You can check the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber + "\n \n Thank you", ticket.TicketNumber, ticket.DueDate);
            string To = model.Email;
            if (model.TeamLead != null)
            {
                cc = model.TeamLead + ";" + initiator.Email + ";" + previousUserEmail;
            }
            else if (model.TeamLead == null && model.HOD != null)
            {
                cc = model.HOD + ";" + initiator.Email + ";" + previousUserEmail;
            }
            else
            {
                cc = initiator.Email + ";" + previousUserEmail;
            }
            //if (model.TeamLead == "moazzam.zaka@jazz.com.pk")
            //{
            //    cc = cc + ";" + "CE-DEVELOPMENTTEAM@jazz.com.pk";
            //}
            //if (model.TeamLead == "ahmad.ali@jazz.com.pk")
            //{
            //    cc = cc + ";" + "CC-REPORTINGTEAM@jazz.com.pk";
            //}
            if (initiator.TeamLead != null)
            {
                cc = cc + ";" + initiator.TeamLead;
            }
            if (initiator.TeamLead == null && initiator.HOD != null)
            {
                cc = cc + ";" + initiator.HOD;
            }
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_NotAcceptedStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            string subject = "New Task Assigned";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been assigned to you with due date {1} by {2}. You can view the details and accept it through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", task.TicketNo, task.DueDate, currentUser);
            string To = task.AssignedTo.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_ExtensionAcknowledgedStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            string subject = "Extension Acknowledged";
            string body = string.Format("Dear Concern, \n \nYour extension request against ticket no {0} has been acknowledged. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", task.TicketNo);
            string To = task.AssignedTo.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_ExtensionRejectedStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            var model = dbContext.Users.Where(x => x.Username.Equals(task.AssignedTo)).FirstOrDefault();
            string subject = "Extension Rejected";
            string body = string.Format("Dear Concern, \n \nYour extension request with ticket no {0} has been rejected. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", task.TicketNo);
            string To = task.AssignedTo.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        [HttpGet]
        public ActionResult ExtractTicketsReport()
        {
            PopulateDropDowns();
            PopulateTicketTypes();
            return View();
        }

        [HttpPost]
        public ActionResult ExtractTicketsReport(TicketsReportDTO dto)
        {
            ViewBag.ExportError = "";
            try
            {
                List<VMTicketReport> tickets = new List<VMTicketReport>();
                tickets = dbContext.Tickets.OrderByDescending(x => x.DateOfInitiation).Select(x => new VMTicketReport
                {
                    TicketNumber = x.TicketNumber,
                    Type = x.Type,
                    TicketType = x.TicketType,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedBy = x.CreatedBy,
                    DateOfInitiation = x.DateOfInitiation,
                    AssignedTo = x.AssignedTo ?? "",
                    AssignedDate = x.AssignedDate,
                    AssignedBy = x.AssignedBy ?? "",
                    Status = x.Status,
                    Team = x.Team,
                    StatusDate = x.StatusDate,
                    DueDate = x.DueDate,
                    Comments = x.Comments ?? "",
                    InitFeedback = x.InitFeedback,
                    InitComments = x.InitComments ?? "",
                    FeedbackDate = x.FeedbackDate,
                    IsReassigned = "No",
                    ReassignCount = 0,
                    ReassignedBy = "",
                    ReassignedDate = null
                }).ToList();

                if (!string.IsNullOrEmpty(dto.TicketStatus))
                    tickets = tickets.Where(x => x.Status.ToUpper() == dto.TicketStatus.ToUpper()).ToList();
                if (!string.IsNullOrEmpty(dto.TicketType))
                    tickets = tickets.Where(x => x.TicketType.ToUpper() == dto.TicketType.ToUpper()).ToList();
                if (!string.IsNullOrEmpty(dto.AssignedTo))
                    tickets = tickets.Where(x => x.AssignedTo.ToUpper() == dto.AssignedTo.ToUpper()).ToList();
                if (!string.IsNullOrEmpty(dto.Team))
                    tickets = tickets.Where(x => x.Team.ToUpper() == dto.Team.ToUpper()).ToList();
                if (dto.StartDate.HasValue)
                    tickets = tickets.Where(x => x.DateOfInitiation.Value.Date >= dto.StartDate.Value.Date).ToList();
                if (dto.EndDate.HasValue)
                    tickets = tickets.Where(x => x.DateOfInitiation.Value.Date <= dto.EndDate.Value.Date).ToList();
                if (tickets != null && tickets.Count > 0)
                {
                    string[] columns = { };
                    tickets = AddReassigmentComments(tickets);
                    if (tickets.Any(x => x.IsReassigned == "Yes"))
                    {
                        string[] columnss = {"TicketNumber",
                                           "TicketType",
                                           "Type",
                                           "Name",
                                           "Description",
                                           "Team",
                                           "CreatedBy",
                                           "DateOfInitiation",
                                           "AssignedTo",
                                           "AssignedDate",
                                           "AssignedBy",
                                           "DueDate",
                                           "Status",
                                           "StatusDate",
                                           "Comments",
                                           "InitFeedback",
                                           "InitComments",
                                           "FeedbackDate",
                                           "IsReassigned",
                                           "ReassignCount",
                                           "ReassignedBy",
                                           "ReassignedDate"};

                        byte[] filecontent = Helpers.ExportToExcel.ExportExcel("Ticket", tickets, "Tickets Report", true, columnss);
                        string fileName = "Tickets Report-" + DateTime.Now.Date.ToShortDateString() + ".xlsx";
                        ViewBag.ExportError = "";
                        return File(filecontent, Helpers.ExportToExcel.ExcelContentType, fileName);

                    }
                    else
                    {
                        string[] colmns = {"TicketNumber",
                                           "TicketType",
                                           "Type",
                                           "Name",
                                           "Description",
                                           "Team",
                                           "CreatedBy",
                                           "DateOfInitiation",
                                           "AssignedTo",
                                           "AssignedDate",
                                           "AssignedBy",
                                           "DueDate",
                                           "Status",
                                           "StatusDate",
                                           "Comments",
                                           "InitFeedback",
                                           "InitComments",
                                           "FeedbackDate"};

                        byte[] filecontent = Helpers.ExportToExcel.ExportExcel("Ticket", tickets, "Tickets Report", true, colmns);
                        string fileName = "Tickets Report-" + DateTime.Now.Date.ToShortDateString() + ".xlsx";
                        ViewBag.ExportError = "";
                        return File(filecontent, Helpers.ExportToExcel.ExcelContentType, fileName);
                    }
                }
                else
                {
                    ViewBag.ExportError = "No Record Found to Export!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ExportError = ex.InnerException.Message ?? "An exception has occured during execution!";
            }

            PopulateDropDowns();
            PopulateTicketTypes();
            return View(dto);
        }

        public List<VMTicketReport> AddReassigmentComments(List<VMTicketReport> tickets)
        {
            foreach (var ticket in tickets)
            {
                var log = dbContext.Tickets_Log.Where(x => x.TicketNumber == ticket.TicketNumber && x.Status == TicketStatus.Open.ToString()).ToList();
                int reAssignCount = log.Count();
                if(reAssignCount > 1)
                {
                    ticket.IsReassigned = "Yes";
                    ticket.ReassignCount = reAssignCount - 1;
                    ticket.AssignedBy = log.OrderBy(x => x.LogDate).Select(x => x.AssignedBy).Take(1).FirstOrDefault();
                    ticket.AssignedDate = log.OrderBy(x => x.LogDate).Select(x => x.AssignedDate).Take(1).FirstOrDefault();
                    List<string> reAssigners = log.OrderByDescending(x => x.LogDate).Select(x => x.AssignedBy).Take(reAssignCount - 1).ToList();
                    ticket.ReassignedBy = string.Join(" ", reAssigners.ToArray());
                    ticket.ReassignedDate = log.OrderByDescending(x => x.LogDate).Select(x => x.AssignedDate).Take(1).FirstOrDefault();
                }
            }
            return tickets;
        }

        [HttpGet]
        public ActionResult ExtractTaskReport()
        {
            PopulateDropDowns();
            PopulateTaskDropdowns();
            return View();
        }

        [HttpPost]
        public ActionResult ExtractTaskReport(TaskReportDTO taskDTO)
        {
            ViewBag.TaskError = "";

            try
            {
                List<VMTaskReport> tasks = new List<VMTaskReport>();
                tasks = dbContext.TaskAssignments.OrderByDescending(x => x.AssignedDate).Select(x => new VMTaskReport
                {
                    TicketNo = x.TicketNo,
                    Description = x.Description,
                    Status = x.Status,
                    StatusDate = x.StatusDate,
                    AssignedTo = x.AssignedTo,
                    AssignedDate = x.AssignedDate,
                    AssignedBy = x.AssignedBy,
                    DueDate = x.DueDate,
                    ExtensionDate = x.ExtensionDate,
                    ExtensionStatus = x.ExtensionStatus,
                    IsExtensionRequested = x.IsExtensionRequested,
                    EmployeeComments = x.EmployeeComments,
                    ManagerComments = x.ManagerComments,
                    SL = x.SL,
                }).ToList();

                if (!string.IsNullOrEmpty(taskDTO.Status))
                    tasks = tasks.Where(x => x.Status.ToUpper() == taskDTO.Status.ToUpper()).ToList();
                if (taskDTO.StartDate.HasValue)
                    tasks = tasks.Where(x => x.AssignedDate.Value.Date >= taskDTO.StartDate.Value.Date).ToList();
                if (taskDTO.EndDate.HasValue)
                    tasks = tasks.Where(x => x.AssignedDate.Value.Date <= taskDTO.EndDate.Value.Date).ToList();
                if (!string.IsNullOrEmpty(taskDTO.AssignedTo))
                    tasks = tasks.Where(x => x.AssignedTo.ToUpper() == taskDTO.AssignedTo.ToUpper()).ToList();
                if (!string.IsNullOrEmpty(taskDTO.AssignedBy))
                    tasks = tasks.Where(x => x.AssignedBy.ToUpper() == taskDTO.AssignedBy.ToUpper()).ToList();

                if (tasks != null && tasks.Count > 0)
                {
                    string[] columns = {"TicketNo",
                                           "Description",
                                           "Status",
                                           "StatusDate",
                                           "AssignedTo",
                                           "AssignedDate",
                                           "AssignedBy",
                                           "DueDate",
                                           "ExtensionDate",
                                           "ExtensionStatus",
                                           "IsExtensionRequested",
                                           "EmployeeComments",
                                           "ManagerComments",
                                            "SL"};

                    byte[] filecontent = Helpers.ExportToExcel.ExportExcel("Task", tasks, "Tasks Report", true, columns);
                    string fileName = "Tasks Report-" + DateTime.Now.Date.ToShortDateString() + ".xlsx";
                    ViewBag.TaskError = "";
                    return File(filecontent, Helpers.ExportToExcel.ExcelContentType, fileName);
                }
                else
                {
                    ViewBag.TaskError = "No Record Found to Export!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.TaskError = ex.InnerException.Message ?? "An exception has occured during execution!";
            }

            PopulateDropDowns();
            PopulateTaskDropdowns();
            return View(taskDTO);
        }

        public void PopulateTicketTypes()
        {
            var ticketType = (dbContext.LkpTicketTypes.Select(x => new SelectListItem
            { Value = x.TicketType, Text = x.TicketType }).Distinct().ToList());
            ViewBag.TicketTypes = ticketType;

            var statusType = (dbContext.LkpStatusTypes.Select(x => new SelectListItem
            { Value = x.Status, Text = x.Status }).Distinct().OrderByDescending(x => x.Value).ToList());
            ViewBag.StatusTypes = statusType;

            var team = (dbContext.Users.Select(x => new SelectListItem
            { Value = x.Team, Text = x.Team }).Distinct().OrderBy(x => x.Value).ToList());
            ViewBag.Teams = team;
        }

        public void PopulateTaskDropdowns()
        {
            var taskStatuses = (dbContext.LkpTaskStatus.Select(x => new SelectListItem
            { Value = x.Name, Text = x.Name }).Distinct());
            ViewBag.TaskStatusList = taskStatuses;

            var assignees = (dbContext.LkpTaskAssigners.Select(x => new SelectListItem
            { Value = x.NTLogin, Text = x.Name }).Distinct());
            ViewBag.Assigners = assignees;

        }

    }
}