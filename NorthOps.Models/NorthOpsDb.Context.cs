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
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Choices> Choices { get; set; }
        public virtual DbSet<Exams> Exams { get; set; }
        public virtual DbSet<JobApplications> JobApplications { get; set; }
        public virtual DbSet<PersonalityResults> PersonalityResults { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<TypingSpeeds> TypingSpeeds { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
        public virtual DbSet<UserLogins> UserLogins { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Videos> Videos { get; set; }
        public virtual DbSet<EmployeeNotications> EmployeeNotications { get; set; }
        public virtual DbSet<NotificationsTemplates> NotificationsTemplates { get; set; }
    }
}
