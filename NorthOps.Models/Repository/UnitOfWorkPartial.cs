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

        private GenericRepository<EmploymentHistories> _EmploymentHistoriesRepo;
        public GenericRepository<EmploymentHistories> EmploymentHistoriesRepo
        {
            get
            {
                if (this._EmploymentHistoriesRepo == null)
                    this._EmploymentHistoriesRepo = new GenericRepository<EmploymentHistories>(context);
                return _EmploymentHistoriesRepo;
            }
            set { _EmploymentHistoriesRepo = value; }
        }

        private GenericRepository<Inventory> _InventoryRepo;
        public GenericRepository<Inventory> InventoryRepo
        {
            get
            {
                if (this._InventoryRepo == null)
                    this._InventoryRepo = new GenericRepository<Inventory>(context);
                return _InventoryRepo;
            }
            set { _InventoryRepo = value; }
        }

        private GenericRepository<Branch> _BranchRepo;
        public GenericRepository<Branch> BranchRepo
        {
            get
            {
                if (this._BranchRepo == null)
                    this._BranchRepo = new GenericRepository<Branch>(context);
                return _BranchRepo;
            }
            set { _BranchRepo = value; }
        }

        private GenericRepository<Divisions> _DivisionsRepo;
        public GenericRepository<Divisions> DivisionsRepo
        {
            get
            {
                if (this._DivisionsRepo == null)
                    this._DivisionsRepo = new GenericRepository<Divisions>(context);
                return _DivisionsRepo;
            }
            set { _DivisionsRepo = value; }
        }

        private GenericRepository<Departments> _DepartmentsRepo;
        public GenericRepository<Departments> DepartmentsRepo
        {
            get
            {
                if (this._DepartmentsRepo == null)
                    this._DepartmentsRepo = new GenericRepository<Departments>(context);
                return _DepartmentsRepo;
            }
            set { _DepartmentsRepo = value; }
        }

        private GenericRepository<Documents> _DocumentsRepo;
        public GenericRepository<Documents> DocumentsRepo
        {
            get
            {
                if (this._DocumentsRepo == null)
                    this._DocumentsRepo = new GenericRepository<Documents>(context);
                return _DocumentsRepo;
            }
            set { _DocumentsRepo = value; }
        }

        private GenericRepository<Overtimes> _OvertimesRepo;
        public GenericRepository<Overtimes> OvertimesRepo
        {
            get
            {
                if (this._OvertimesRepo == null)
                    this._OvertimesRepo = new GenericRepository<Overtimes>(context);
                return _OvertimesRepo;
            }
            set { _OvertimesRepo = value; }
        }

        private GenericRepository<ErrorTypes> _ErrorTypesRepo;
        public GenericRepository<ErrorTypes> ErrorTypesRepo
        {
            get
            {
                if (this._ErrorTypesRepo == null)
                    this._ErrorTypesRepo = new GenericRepository<ErrorTypes>(context);
                return _ErrorTypesRepo;
            }
            set { _ErrorTypesRepo = value; }
        }


        private GenericRepository<Breaks> _BreaksRepo;
        public GenericRepository<Breaks> BreaksRepo
        {
            get
            {
                if (this._BreaksRepo == null)
                    this._BreaksRepo = new GenericRepository<Breaks>(context);
                return _BreaksRepo;
            }
            set { _BreaksRepo = value; }
        }


        private GenericRepository<BreakTypes> _BreakTypesRepo;
        public GenericRepository<BreakTypes> BreakTypesRepo
        {
            get
            {
                if (this._BreakTypesRepo == null)
                    this._BreakTypesRepo = new GenericRepository<BreakTypes>(context);
                return _BreakTypesRepo;
            }
            set { _BreakTypesRepo = value; }
        }


        private GenericRepository<Violations> _ViolationsRepo;
        public GenericRepository<Violations> ViolationsRepo
        {
            get
            {
                if (this._ViolationsRepo == null)
                    this._ViolationsRepo = new GenericRepository<Violations>(context);
                return _ViolationsRepo;
            }
            set { _ViolationsRepo = value; }
        }

        private GenericRepository<ViolationTypes> _ViolationTypesRepo;
        public GenericRepository<ViolationTypes> ViolationTypesRepo
        {
            get
            {
                if (this._ViolationTypesRepo == null)
                    this._ViolationTypesRepo = new GenericRepository<ViolationTypes>(context);
                return _ViolationTypesRepo;
            }
            set { _ViolationTypesRepo = value; }
        }


        
    }
}
