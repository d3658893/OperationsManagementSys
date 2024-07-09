using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class VMTicketReport
    {
        public string TicketNumber { get; set; }
        public string TicketType { get; set; }
        public Nullable<System.DateTime> DateOfInitiation { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Team { get; set; }
        public string AssignedTo { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string InitFeedback { get; set; }
        public string AssignedBy { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string InitComments { get; set; }
        public Nullable<System.DateTime> FeedbackDate { get; set; }
        public string IsReassigned { get; set; }
        public int ReassignCount { get; set; }
        public string ReassignedBy { get; set; }
        public Nullable<System.DateTime> ReassignedDate { get; set; }
    }
}