using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.Repository
{
   public partial class UnitOfWork
    {

        private GenericRepository<Holidays> _HolidaysRepo;
        public GenericRepository<Holidays> HolidaysRepo
        {
            get
            {
                if (this._HolidaysRepo == null)
                    this._HolidaysRepo = new GenericRepository<Holidays>(context);
                return _HolidaysRepo;
            }
            set { _HolidaysRepo = value; }
        }
    }
}
