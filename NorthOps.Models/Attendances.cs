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
    
    public partial class Attendances
    {
        public int Id { get; set; }
        public int BiometricId { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int InOutState { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> IsApproved { get; set; }
    
        public virtual Biometrics Biometrics { get; set; }
    }
}
