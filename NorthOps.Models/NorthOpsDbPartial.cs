﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class northopsEntities
    {
        public static northopsEntities Create()
        {
            return new northopsEntities();
        }
    }
}
