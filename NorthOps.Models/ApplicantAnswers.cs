//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ApplicantAnswers
    {
        public System.Guid ApplicantAnswerId { get; set; }
        public string UserId { get; set; }
        public Nullable<System.Guid> QuestionId { get; set; }
        public Nullable<System.Guid> ChoiceId { get; set; }
    
        public virtual Choices Choices { get; set; }
        public virtual Questions Questions { get; set; }
        public virtual Users Users { get; set; }
    }
}
