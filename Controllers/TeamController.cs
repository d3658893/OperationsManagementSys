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

namespace OperationsManagementSystem.Controllers
{

    [Authorize(Roles = "Team,Admin,SuperAdmin,InitiatorTeam,InitiatorTeamAdmin")]
    public class TeamController : Controller
    {
        private OMSEntities dbContext = new OMSEntities();
        private string DOMAIN = "@jazz.com.pk";

        // GET: Team
        [HttpGet]
        public ActionResult Index()
        {
            var totalTickets = dbContext.Tickets.Where(x=>x.AssignedTo == User.Identity.Name).ToList();
            ViewBag.OpenTickets = totalTickets.Where(x => x.Status == "Open").Count();
            ViewBag.HoldTickets = totalTickets.Where(x => x.Status == "Hold").Count();
            ViewBag.CompletedTickets = totalTickets.Where(x => x.Status == "Completed").Count();
            ViewBag.TotalTickets = totalTickets.Count();

            var ticketType = (dbContext.LkpTicketTypes.Select(x => new SelectListItem
            { Value = x.TicketType, Text = x.TicketType }).Distinct().ToList());
            ViewBag.TicketTypes = ticketType;

            var statusType = (dbContext.LkpStatusTypes.Where(x => x.Status == "Open" || x.Status == "Hold" || x.Status == "Completed").Select(x => new SelectListItem
            { Value = x.Status, Text = x.Status }).Distinct().OrderByDescending(x=>x.Value).ToList());
            ViewBag.StatusTypes = statusType;

            return View();
        }

        [HttpPost]
        public ActionResult Index(SearchTicket ticket)
        {
            ViewBag.SearchMessage = "";
            var currentUser = User.Identity.Name;

            List<Ticket> tickets = new List<Ticket>();
            if (ticket.TicketType != null && ticket.TicketStatus != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus) && x.TicketType.Equals(ticket.TicketType)
                                                     && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                     && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                     && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.TicketStatus != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && x.Status == ticket.TicketStatus && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.TicketStatus != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && x.Status == ticket.TicketStatus && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketStatus != null && ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate) && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.TicketStatus != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && x.Status == ticket.TicketStatus
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType.Equals(ticket.TicketType)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketStatus != null && ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketStatus != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status.Equals(ticket.TicketStatus)
                                                    && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                    && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.StartDate != null && ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate)
                                                       && DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate)
                                                       && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketType != null)
            {
                tickets = dbContext.Tickets.Where(x => x.TicketType == ticket.TicketType
                                                       && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.TicketStatus != null)
            {
                tickets = dbContext.Tickets.Where(x => x.Status == ticket.TicketStatus && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.StartDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) >= DbFunctions.TruncateTime(ticket.StartDate) && x.AssignedTo == currentUser).ToList();
            }
            else if (ticket.EndDate != null)
            {
                tickets = dbContext.Tickets.Where(x => DbFunctions.TruncateTime(x.DateOfInitiation) <= DbFunctions.TruncateTime(ticket.EndDate) && x.AssignedTo == currentUser).ToList();
            }
            else
            {
                ViewBag.SearchMessage = "Please select atleast option to get result.";
            }
           
            if (tickets == null || tickets.Count == 0)
            {
                ViewBag.SearchMessage = "No Record Found";
            }
            else
            {
                tickets = tickets.OrderByDescending(x => x.StatusDate).ToList();
            }    
            if (tickets.Any(x => x.Status == TicketStatus.Open.ToString()))
            {
                return PartialView("_SearchTicketByDate", tickets);
            }
            else if (tickets.Any(x => x.Status == TicketStatus.Hold.ToString()))
            {
                return PartialView("_UpdateHoldStatus", tickets);
            }
            return PartialView("_ShowCompletedTickets", tickets);
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
                return PartialView("_SearchUnassigned", tickets);
            }
            if (tickets.Any(x => x.Status == TicketStatus.Open.ToString()))
            {
                return PartialView("_SearchTicketByDate", tickets);
            }
            else if (tickets.Any(x => x.Status == TicketStatus.Hold.ToString()))
            {
                return PartialView("_UpdateHoldStatus", tickets);
            }
            return PartialView("_ShowCompletedTickets", tickets);
        }

        [HttpGet]
        public ActionResult SearchTask()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchTask(string TicketNumber)
        {
            OMSEntities db = new OMSEntities();
            var tasks = db.TaskAssignments.Where(y => y.TicketNo == TicketNumber && y.AssignedTo == User.Identity.Name).ToList();
            if (tasks.Any(x => x.Status == "Not Accepted"))
            {
                return PartialView("_TeamTasksNotAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Accepted"))
            {
                return PartialView("_TeamTasksAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Completed"))
            {
                return PartialView("_TasksCompleted", tasks);
            }
            if (tasks.Any(x => x.Status == "Extension Requested"))
            {
                return PartialView("_TeamTasksExtensionRequired", tasks);
            }

            return PartialView("_TeamTasksExtensionResponse", tasks);
        }

        public ActionResult UpdateHoldStatus(int ID)
        {
            Ticket model = new Ticket();

            List<SelectListItem> holdStatusTypes = new List<SelectListItem>()
            {
              new SelectListItem {Text="Completed",Value="Completed" },
            };
            ViewBag.HoldStatusTypes = holdStatusTypes;

            if (ID > 0)
            {
                Ticket ticket = dbContext.Tickets.SingleOrDefault(x => x.ID == ID);
                model.ID = ticket.ID;
                model.Name = ticket.Name;
                model.Status = ticket.Status;
                model.TicketNumber = ticket.TicketNumber;
                model.TicketType = ticket.TicketType;
                model.TypeOfReport = ticket.TypeOfReport;
                model.Name = ticket.Name;
                model.Type = ticket.Type;
                model.DateOfInitiation = ticket.DateOfInitiation;
                model.Description = ticket.Description;
                model.Team = ticket.Team;
                model.Attachment = ticket.Attachment;
                model.CreatedBy = ticket.CreatedBy;
                model.CreatedDate = ticket.CreatedDate;
                model.AssignedTo = ticket.AssignedTo;
                model.AssignedDate = ticket.AssignedDate;
                model.DueDate = ticket.DueDate;
                model.AssignedBy = ticket.AssignedBy;
                model.StatusDate = ticket.StatusDate;
                model.CompleterAttachment = ticket.CompleterAttachment;
            }

            return PartialView("_UpdateHoldTicket", model);
        }

        public ActionResult UpdateTicketStatus(int ID)
        {
            Ticket model = new Ticket();

            List<SelectListItem> openStatusTypes = new List<SelectListItem>()
            {
              new SelectListItem {Text="Hold",Value="Hold",Selected=true },
              new SelectListItem {Text="Completed",Value="Completed" },
            };
            ViewBag.OpenStatusTypes = openStatusTypes;

            if (ID > 0)
            {
                Ticket ticket = dbContext.Tickets.SingleOrDefault(x => x.ID == ID);
                model.ID = ticket.ID;
                model.Name = ticket.Name;
                model.Status = ticket.Status;
                model.TicketNumber = ticket.TicketNumber;
                model.TicketType = ticket.TicketType;
                model.TypeOfReport = ticket.TypeOfReport;
                model.Name = ticket.Name;
                model.Type = ticket.Type;
                model.DateOfInitiation = ticket.DateOfInitiation;
                model.Description = ticket.Description;
                model.Team = ticket.Team;
                model.Attachment = ticket.Attachment;
                model.CreatedBy = ticket.CreatedBy;
                model.CreatedDate = ticket.CreatedDate;
                model.AssignedTo = ticket.AssignedTo;
                model.AssignedDate = ticket.AssignedDate;
                model.DueDate = ticket.DueDate;
                model.AssignedBy = ticket.AssignedBy;
                model.StatusDate = ticket.StatusDate;
                model.CompleterAttachment = ticket.CompleterAttachment;
            }

            return PartialView("_UpdateTicket", model);
        }

        public JsonResult UpdateTicket(Ticket ticket, HttpPostedFileBase CompleterAttachment)
        {
            if (ticket.AssignedTo == User.Identity.Name)
            {
                if (ticket.Status == "Completed")
                {
                    #region Save Uploaded File
                    if (CompleterAttachment != null && CompleterAttachment.ContentLength > 0)
                    {
                        string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
                        string fullPath = Path.Combine(Server.MapPath("/UploadedFiles/"), identifier + "_" + CompleterAttachment.FileName);
                        ticket.CompleterAttachment = fullPath;
                        CompleterAttachment.SaveAs(fullPath);
                    }
                    #endregion
                }
                ticket.ModifiedBy = User.Identity.Name.Split('\\')[1];
                ticket.ModifiedDate = DateTime.Now;
                ticket.StatusDate = DateTime.Now;
                dbContext.Set<Ticket>().AddOrUpdate(ticket);
                dbContext.SaveChanges();
                if (ticket.Status == "Hold")
                {
                    SendEmail_HoldStatus(ticket);
                }
                if (ticket.Status == "Completed")
                {
                    SendEmail_CompletedTicket(ticket);
                }
                return Json(new { success = true, responseText = "Ticket Updated Successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, responseText = "Sorry! You don't have permissions to update this ticket." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult TasksAssignedByManager()
        {
            var totalTasks = dbContext.TaskAssignments.Where(x => x.AssignedTo == User.Identity.Name).ToList();
            ViewBag.NotAcceptedTasks = totalTasks.Where(x => x.Status == "Not Accepted").Count();
            ViewBag.AcceptedTasks = totalTasks.Where(x => x.Status == "Accepted").Count();
            ViewBag.CompletedTasks = totalTasks.Where(x => x.Status == "Completed").Count();
            ViewBag.ExtensionRequestedTasks = totalTasks.Where(x => x.Status == "Extension Requested").Count();
            ViewBag.ExtensionAcknowledgedTasks = totalTasks.Where(x => x.Status == "Extension Acknowledged").Count();
            ViewBag.ExtensionRejectedTasks = totalTasks.Where(x => x.Status == "Extension Rejected").Count();

            var taskStatuses = (dbContext.LkpTaskStatus.Select(x => new SelectListItem
            { Value = x.Name, Text = x.Name }).Distinct());
            ViewBag.TaskStatusList = taskStatuses;

            return View();
        }

        [HttpPost]
        public ActionResult TasksAssignedByManager(LkpTaskStatu status)
        {
            ViewBag.SearchMessage = "";
            List<TaskAssignment> tasks = new List<TaskAssignment>();
            tasks = dbContext.TaskAssignments.Where(x => x.Status.ToUpper() == status.Name.ToUpper() && x.AssignedTo == User.Identity.Name).OrderByDescending(x=>x.StatusDate).ToList();
            if (tasks == null || tasks.Count == 0)
                ViewBag.SearchMessage = "No Record Found";
            if (tasks.Any(x => x.Status == "Not Accepted"))
            {
                return PartialView("_TeamTasksNotAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Accepted"))
            {
                return PartialView("_TeamTasksAccepted", tasks);
            }
            if (tasks.Any(x => x.Status == "Completed"))
            {
                return PartialView("_TasksCompleted", tasks);
            }
            if (tasks.Any(x => x.Status == "Extension Requested"))
            {
                return PartialView("_TeamTasksExtensionRequired", tasks);
            }

            return PartialView("_TeamTasksExtensionResponse", tasks);
        }

        public ActionResult UpdateStatusToAccepted(int? ID)
        {
            TaskAssignment model = new TaskAssignment();
            List<SelectListItem> acceptedStatus = new List<SelectListItem>()
            {
              new SelectListItem {Text="Accepted",Value="Accepted",Selected=true },
            };

            ViewBag.AcceptedStatus = acceptedStatus;

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
            }

            return PartialView("_UpdateAcceptedStatus", model);
        }

        public ActionResult UpdateAcceptedStatus(TaskAssignment task)
        {
            task.StatusDate = DateTime.Now;
            dbContext.Set<TaskAssignment>().AddOrUpdate(task);
            dbContext.SaveChanges();
            SendEmail_AcceptedStatus(task);
            return RedirectToAction("TasksAssignedByManager", "Team");
        }

        public ActionResult UpdateStatusToCompleted(int? ID)
        {
            TaskAssignment model = new TaskAssignment();
            List<SelectListItem> completedStatus = new List<SelectListItem>()
            {
              new SelectListItem {Text="Completed",Value="Completed",Selected=true },
              new SelectListItem {Text="Extension Requested",Value="Extension Requested",Selected=true },
            };

            ViewBag.CompletedStatus = completedStatus;

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

            return PartialView("_UpdateCompletedStatus", model);
        }

        public ActionResult UpdateCompletedStatus(TaskAssignment task)
        {
            if (task.Status == "Completed")
            {
                var todayDate = DateTime.Now.Date;
                var dueDate = task.DueDate.Value.Date;
                if (todayDate <= dueDate)
                {
                    task.SL = "Meet";
                }
                else
                {
                    task.SL = "Not Meet";
                }
                SendEmail_CompletedStatus(task);
            }
            if(task.Status == "Extension Requested")
            {
                DateTime extDate = Convert.ToDateTime(task.ExtensionDate).AddDays(1).AddSeconds(-1);
                task.ExtensionDate = extDate;
                task.IsExtensionRequested = true;
                task.ExtensionStatus = "Requested";
                SendEmail_ExtensionRequiredStatus(task);
            }
            task.StatusDate = DateTime.Now;
            dbContext.Set<TaskAssignment>().AddOrUpdate(task);
            dbContext.SaveChanges();
            return RedirectToAction("TasksAssignedByManager", "Team");
        }

        public ActionResult UpdateExtensionToCompleted(int? ID)
        {
            TaskAssignment model = new TaskAssignment();
            List<SelectListItem> completedStatus = new List<SelectListItem>()
            {
              new SelectListItem {Text="Completed",Value="Completed",Selected=true },
            };

            ViewBag.CompletedStatus = completedStatus;

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

            return PartialView("_UpdateExtension", model);
        }

        public ActionResult UpdateExtensionStatus(TaskAssignment task)
        {
            var todayDate = DateTime.Now.Date;
            var dueDate = task.DueDate.Value.Date;
            if (todayDate <= dueDate)
            {
                task.SL = "Meet";
            }
            else
            {
                task.SL = "Not Meet";
            }
            task.StatusDate = DateTime.Now;
            dbContext.Set<TaskAssignment>().AddOrUpdate(task);
            dbContext.SaveChanges();
            SendEmail_CompletedStatus(task);
            return RedirectToAction("TasksAssignedByManager", "Team");
        }

        public void SendEmail_AcceptedStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            string subject = "Task Accepted";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been accepted by {1}. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", task.TicketNo, currentUser);
            string To = task.AssignedBy.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_CompletedStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            string subject = "Task Completed";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been completed. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", task.TicketNo);
            string To = task.AssignedBy.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_ExtensionRequiredStatus(TaskAssignment task)
        {
            var currentUser = User.Identity.Name.Split('\\')[1];
            string subject = "Extension Required";
            string body = string.Format("Dear Concern, \n \n{0} has requested extension in task with ticket no {1}. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTaskDetail?id=" + task.TicketNo + "\n \n Thank you", currentUser, task.TicketNo);
            string To = task.AssignedBy.Split('\\')[1] + DOMAIN;
            string cc = currentUser + DOMAIN;
            dbContext.Proc_SendEmail(To, cc, subject, body);
        }

        public void SendEmail_HoldStatus(Ticket ticket)
        {
            string cc = string.Empty;
            var currentUser = User.Identity.Name.Split('\\')[1];
            var model = dbContext.Users.Where(x => x.Username.Equals(ticket.AssignedTo)).FirstOrDefault();
            var initiator = dbContext.Users.Where(x => x.Username.Equals(ticket.CreatedBy)).FirstOrDefault();
            string subject = "Ticket On Hold";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been put on hold. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber + "\n \n Thank you", ticket.TicketNumber);
            string To = ticket.AssignedBy.Split('\\')[1] + DOMAIN;
            if(model.TeamLead != null)
            {
                cc = model.Email + ";" + model.TeamLead;
            }
            else if(model.TeamLead == null && model.HOD != null)
            {
                cc = model.Email + ";" + model.HOD;
            }
            else
            {
                cc = model.Email;
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

        public void SendEmail_CompletedTicket(Ticket ticket)
        {
            string cc = string.Empty;
            var currentUser = User.Identity.Name.Split('\\')[1];
            var completer = dbContext.Users.Where(x => x.Username.Equals(ticket.AssignedTo)).FirstOrDefault();
            var initiator = dbContext.Users.Where(x => x.Username.Equals(ticket.CreatedBy)).FirstOrDefault();
            string subject = "Ticket Completed";
            string body = string.Format("Dear Concern, \n \nTask with ticket no {0} has been completed. You can view the details through following application. \n \n" +

                                    "http://lhe-ccdev-ms1:85/Home/ShowTicketDetail?id=" + ticket.TicketNumber , ticket.TicketNumber);
            body = body + "\n \n Please click on below link to give feedback on Completed Ticket. \n \n " + "http://lhe-ccdev-ms1:85/Home/UserFeedback?id=" + ticket.TicketNumber + " \n \n Thank You";

            string To = initiator.Email;
            if (completer.TeamLead != null)
            {
                cc = completer.Email + ";" + completer.TeamLead;
            }
            else if (completer.TeamLead == null && completer.HOD != null)
            {
                cc = completer.Email + ";" + completer.HOD;
            }
            else
            {
                cc = completer.Email;
            }
            //if (completer.TeamLead == "moazzam.zaka@jazz.com.pk")
            //{
            //    cc = cc + ";" + "CE-DEVELOPMENTTEAM@jazz.com.pk";
            //}
            //if (completer.TeamLead == "ahmad.ali@jazz.com.pk")
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

    }
}