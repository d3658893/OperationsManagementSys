//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OperationsManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskAssignment
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Attachment { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<int> Counter { get; set; }
        public Nullable<System.DateTime> ExtensionDate { get; set; }
        public string EmployeeComments { get; set; }
        public string ManagerComments { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string TeamMember { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string SL { get; set; }
        public string TicketNo { get; set; }
        public Nullable<bool> IsExtensionRequested { get; set; }
        public string ExtensionStatus { get; set; }
    }
}