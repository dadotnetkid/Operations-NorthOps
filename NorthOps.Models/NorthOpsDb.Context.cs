﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthOps.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class northopsEntities : DbContext
    {
        public northopsEntities()
            : base("name=northopsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AddressStateProvinces> AddressStateProvinces { get; set; }
        public virtual DbSet<AddressTownCities> AddressTownCities { get; set; }
        public virtual DbSet<ApplicantAnswers> ApplicantAnswers { get; set; }
        public virtual DbSet<Applicants> Applicants { get; set; }
        public virtual DbSet<Biometrics> Biometrics { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Campaigns> Campaigns { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Choices> Choices { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Divisions> Divisions { get; set; }
        public virtual DbSet<EducationAttainments> EducationAttainments { get; set; }
        public virtual DbSet<EmployeeNotications> EmployeeNotications { get; set; }
        public virtual DbSet<EmploymentHistories> EmploymentHistories { get; set; }
        public virtual DbSet<Equivalents> Equivalents { get; set; }
        public virtual DbSet<Exams> Exams { get; set; }
        public virtual DbSet<JobApplications> JobApplications { get; set; }
        public virtual DbSet<NotificationsTemplates> NotificationsTemplates { get; set; }
        public virtual DbSet<PersonalityResults> PersonalityResults { get; set; }
        public virtual DbSet<Positions> Positions { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<Shifts> Shifts { get; set; }
        public virtual DbSet<TypingSpeeds> TypingSpeeds { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
        public virtual DbSet<UserLogins> UserLogins { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Videos> Videos { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<ItemTypes> ItemTypes { get; set; }
        public virtual DbSet<Recordings> Recordings { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<UsersInCampaignShift> UsersInCampaignShift { get; set; }
        public virtual DbSet<Overtimes> Overtimes { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<ErrorTypes> ErrorTypes { get; set; }
        public virtual DbSet<DailyTimeRecords> DailyTimeRecords { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<Attendances> Attendances { get; set; }
        public virtual DbSet<MachineConfigs> MachineConfigs { get; set; }
        public virtual DbSet<Sanctions> Sanctions { get; set; }
        public virtual DbSet<ViolationTypes> ViolationTypes { get; set; }
        public virtual DbSet<RestDays> RestDays { get; set; }
        public virtual DbSet<Violations> Violations { get; set; }
        public virtual DbSet<Breaks> Breaks { get; set; }
        public virtual DbSet<BreakTypes> BreakTypes { get; set; }
        public virtual DbSet<LeaveTypes> LeaveTypes { get; set; }
        public virtual DbSet<OvertimeAttendances> OvertimeAttendances { get; set; }
        public virtual DbSet<Leaves> Leaves { get; set; }
    }
}
