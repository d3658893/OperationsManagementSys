using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class TaskReportDTO
    {
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
    }
}