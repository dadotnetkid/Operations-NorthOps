using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class Campaigns
    {
        public string UsersInCampaigns => string.Join(",", this.Users.Select(x => x.FullName));
    }
}
