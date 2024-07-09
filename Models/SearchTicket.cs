using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class SearchTicket
    {
        public string TicketType { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string TicketStatus { get; set; }
        public string AssignedTo { get; set; }
    }
}