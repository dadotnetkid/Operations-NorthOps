﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthOps.Ops.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Microsoft.AspNet.Identity.EntityFramework;
    
    public partial class NorthOpsEntities : DbContext
    {
        public NorthOpsEntities()
            : base("name=NorthOpsEntities")
        {
        }
    	public static NorthOpsEntities Create() {
    		return new NorthOpsEntities();
    	}
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<JobApplication> JobApplications { get; set; }
        public virtual DbSet<AddressStateProvince> AddressStateProvinces { get; set; }
        public virtual DbSet<AddressTownCity> AddressTownCities { get; set; }
        public virtual DbSet<ApplicantAnswer> ApplicantAnswers { get; set; }
        public virtual DbSet<Applicant> Applicants { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<PersonalityResult> PersonalityResults { get; set; }
        public virtual DbSet<TypingSpeed> TypingSpeeds { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<FileManagementData> FileManagementDatas { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
    }
}
