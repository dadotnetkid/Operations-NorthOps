using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models;
using NorthOps.Models.Repository;
using ZKTeco.SDK.Model;
using ZKTeco.SDK.MachineManager;
namespace NorthOps.Services.DTRService
{
    public class AttendanceServices : IDtrServices
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public Task<List<Transactions>> AttendanceLog()
        {
            STDDevComm devComm = new STDDevComm(new Machines(ip: "192.168.5.150"));
            var transaction = new List<Transactions>();
            devComm.GetAllTransaction(out transaction);
            return Task.FromResult(transaction);
        }
        public async Task SaveAttendanceLog()
        {
            try
            {
                var res = await AttendanceLog();
                var attendanceRepo = new List<Attendances>();
                foreach (var i in res)
                {
                    if (!unitOfWork.AttendancesRepo.Fetch(m =>
                        m.LogDateTime == i.LogDateTime && m.BiometricId == i.Pin &&
                        m.InOutState == (int)i.InOutState).Any()
                        && unitOfWork.BiometricsRepo.Fetch(m => m.BiometricId == i.Pin).Any() && (i.InOutState == InOutState.CheckIn || i.InOutState == InOutState.CheckOut))
                    {
                        attendanceRepo.Add(new Attendances()
                        {
                            LogDateTime = i.LogDateTime,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            BiometricId = i.Pin,
                            InOutState = (int)i.InOutState,
                        });
                    }
                }

                unitOfWork.AttendancesRepo.InsertRange(attendanceRepo);
                await unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

    }
}
