//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class AddressStateProvince
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AddressStateProvince()
        {
            this.AddressTownCities = new HashSet<AddressTownCity>();
        }
    
        public int StateProvinceId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddressTownCity> AddressTownCities { get; set; }
    }
}
