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
    
    public partial class Documents
    {
        public string Id { get; set; }
        public string CampaignId { get; set; }
        public string Title { get; set; }
        public string Project { get; set; }
        public string AddedBy { get; set; }
        public string Path { get; set; }
        public Nullable<int> DocumentType { get; set; }
        public string DocumentContent { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
    
        public virtual Campaigns Campaigns { get; set; }
        public virtual Users Users { get; set; }
    }
}