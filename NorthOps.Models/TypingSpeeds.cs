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
    
    public partial class TypingSpeeds
    {
        public System.Guid TypingId { get; set; }
        public Nullable<System.Guid> ExamId { get; set; }
        public string Paragraph { get; set; }
        public Nullable<int> TypingLevel { get; set; }
    
        public virtual Exams Exams { get; set; }
    }
}
