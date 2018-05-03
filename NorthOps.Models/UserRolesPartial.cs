using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace NorthOps.Models
{
    [MetadataTypeAttribute(typeof(UserRoleMetaData))]
    public partial class UserRoles : IRole<string>
    {
    }
    public class UserRoleMetaData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
