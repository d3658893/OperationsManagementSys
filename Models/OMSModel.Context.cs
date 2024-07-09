﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class OMSEntities : DbContext
    {
        public OMSEntities()
            : base("name=OMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LkpApplicationName> LkpApplicationNames { get; set; }
        public virtual DbSet<LkpProcessName> LkpProcessNames { get; set; }
        public virtual DbSet<LkpReportType> LkpReportTypes { get; set; }
        public virtual DbSet<LkpStatusType> LkpStatusTypes { get; set; }
        public virtual DbSet<LkpTaskStatu> LkpTaskStatus { get; set; }
        public virtual DbSet<LkpTeam> LkpTeams { get; set; }
        public virtual DbSet<LkpTicketType> LkpTicketTypes { get; set; }
        public virtual DbSet<LkpType> LkpTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }
        public virtual DbSet<TeamMember> TeamMembers { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<LkpTaskAssigner> LkpTaskAssigners { get; set; }
        public virtual DbSet<Tickets_Log> Tickets_Log { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
    
        public virtual int Proc_SendEmail(string to, string cC, string subject, string bodyText)
        {
            var toParameter = to != null ?
                new ObjectParameter("To", to) :
                new ObjectParameter("To", typeof(string));
    
            var cCParameter = cC != null ?
                new ObjectParameter("CC", cC) :
                new ObjectParameter("CC", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("Subject", subject) :
                new ObjectParameter("Subject", typeof(string));
    
            var bodyTextParameter = bodyText != null ?
                new ObjectParameter("BodyText", bodyText) :
                new ObjectParameter("BodyText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Proc_SendEmail", toParameter, cCParameter, subjectParameter, bodyTextParameter);
        }
    }
}