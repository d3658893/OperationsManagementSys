using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public enum TicketStatus
    {
        UnAssigned,
        Open,
        Hold,
        Completed
    }

    public enum ProcessType
    {
        Process,
        Application,
        Report
    }

}