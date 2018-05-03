﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.Repository;

namespace NorthOps.Models
{
    public partial class Questions
    {
        public IEnumerable<Videos> VideoList => new UnitOfWork().VideoRepo.Get();
    }
}
