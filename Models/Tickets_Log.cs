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
    
    public partial class Tickets_Log
    {
        public int LogID { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public string ActionPerformed { get; set; }
        public string TicketNumber { get; set; }
        public string AssignedTo { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string Comments { get; set; }
        public string CompletedBy { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public string InitFeedback { get; set; }
        public string InitComments { get; set; }
        public Nullable<System.DateTime> FeedbackDate { get; set; }
    }
}
