using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class TicketsReportDTO
    {
        public string TicketType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TicketStatus { get; set; }
        public string AssignedTo { get; set; }
        public string Team { get; set; }
    }
}