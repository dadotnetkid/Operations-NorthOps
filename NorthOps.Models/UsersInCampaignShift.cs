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
    
    public partial class UsersInCampaignShift
    {
        public string Id { get; set; }
        public string CampaignId { get; set; }
        public string UserId { get; set; }
        public string ShiftId { get; set; }
    
        public virtual Campaigns Campaigns { get; set; }
        public virtual Shifts Shifts { get; set; }
        public virtual Users Users { get; set; }
    }
}
