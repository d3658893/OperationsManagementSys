using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class VMTaskReport
    {

        public string TicketNo { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string EmployeeComments { get; set; }
        public Nullable<System.DateTime> ExtensionDate { get; set; }
        public string ManagerComments { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string SL { get; set; }     
        public Nullable<bool> IsExtensionRequested { get; set; }
        public string ExtensionStatus { get; set; }
    }
}