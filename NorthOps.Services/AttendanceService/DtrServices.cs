using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;
using NorthOps.Services.DTRService;
using ZKTeco.SDK.MachineManager;
using ZKTeco.SDK.Model;

namespace NorthOps.Services.AttendanceService
{
    public class AttendanceServices : IDtrServices
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public Task<List<Transactions>> AttendanceLog()
        {
            STDDevComm devComm = new STDDevComm(new Machines(ip: "10.10.30.50"));
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
                        && unitOfWork.UserRepository.Fetch(m => m.BiometricId == i.Pin).Any() && (i.InOutState == InOutState.CheckIn || i.InOutState == InOutState.CheckOut))
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

        public void GetDtrReport(DtrReportViewModel model)
        {

            var users = unitOfWork.UserRepository.Fetch(m => m.UserRoles.Any(x => x.Name == "Employee"), includeProperties: "Schedules,Schedules.DailyTimeRecords,Overtimes,Overtimes.CreatedByUser,Overtimes.ModifiedByUser,Overtimes.Users");
            if (!string.IsNullOrEmpty(model.UserId))
            {
                users = users.Where(m => m.Id == model.UserId);
            }


            foreach (var i in users.ToList())
            {
                var attendance = unitOfWork.AttendancesRepo.Get(m =>
                    m.BiometricId == i.BiometricId &&
                    (m.LogDateTime >= model.DateLogFrom && m.LogDateTime <= model.DateLogTo)).ToList();
                var schedules = unitOfWork.SchedulesRepo.Get(m =>
                   m.UserId == i.Id && (m.ScheduleDateFrom >= model.DateLogFrom && m.ScheduleDateTo <= model.DateLogTo)).OrderBy(m => m.ScheduleDateFrom).ToList();

                foreach (var s in schedules)
                {
                    var _dateFrom = Convert.ToDateTime(s.ScheduleDateFrom.ToShortDateString());
                    var _dateTo = Convert.ToDateTime(s.ScheduleDateTo.ToShortDateString() + " 11:59 PM");

                    var _attendance = attendance.Where(m => (m.LogDateTime >= _dateFrom) && (m.LogDateTime <= _dateTo)).ToList();
                    var dtr = unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.ScheduleId == s.Id);
                    if (dtr.Any(x => x.ModifiedBy == null))
                    {
                        var _dtr = dtr.FirstOrDefault();
                        _dtr.DateFrom = _attendance.OrderBy(m => m.Id)
                            .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckIn)?.LogDateTime;
                        _dtr.DateTo = _attendance.OrderByDescending(m => m.Id)
                            .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckOut)?.LogDateTime;
                        unitOfWork.Save();
                    }


                    var _from = _attendance.OrderBy(m => m.Id)
                            .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckIn)?.LogDateTime;
                    var _to = _attendance.OrderByDescending(m => m.Id)
                        .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckOut)?.LogDateTime;

                    if (!dtr.Any())
                    {
                        unitOfWork.DailyTimeRecordsRepo.Insert(new DailyTimeRecords()
                        {
                            ScheduleId = s.Id,
                            DateFrom = _from > s.ScheduleDateFrom ? s.ScheduleDateFrom : _from,
                            DateTo = _to > s.ScheduleDateTo ? s.ScheduleDateTo : _to,
                            CreatedBy = HttpContext.Current.User.Identity.GetUserId()
                        });
                        unitOfWork.Save();
                    }





                    foreach (var _a in _attendance)
                    {

                        attendance.Remove(_a);
                        if ((InOutState)_a.InOutState == InOutState.CheckOut)
                        {
                            break;
                        }
                    }
                }

            }




        }

        public List<DailyTimeRecordViewModel> dailyTimeRecords(DtrReportViewModel model)
        {


            var dailyTimeRecords = new List<DailyTimeRecordViewModel>();

            model.UserId = model.UserId ?? "";
            foreach (var i in unitOfWork.UserRepository.Fetch(m => m.UserRoles.Any(x => x.Name == "Employee") && m.Id.Contains(model.UserId)))
            {

                dailyTimeRecords.Add(new DailyTimeRecordViewModel()
                {
                    Users = i,
                    Overtimes = i.Overtimes.Where(m => m.DateFrom >= model.DateLogFrom && m.DateTo <= model.DateLogTo).ToList(),
                    DailyTimeRecords = unitOfWork.DailyTimeRecordsRepo.Get(m => m.Schedules.UserId == i.Id &&
                                                                                (m.Schedules.ScheduleDateFrom >=
                                                                                 model.DateLogFrom &&
                                                                                 m.Schedules.ScheduleDateTo <=
                                                                                 model.DateLogTo))
                        .OrderBy(m => m.DateFrom).ToList()
                });
            }
            return dailyTimeRecords;
        }
        public async Task<Users> GetDtrReport(string userId)
        {
            List<DtrReportViewModel> dtrReportViewModels = new List<DtrReportViewModel>();
            var users = unitOfWork.UserRepository.Find(m => m.Id == userId);


            var attendance = unitOfWork.AttendancesRepo.Fetch();
            attendance = attendance.Where(m => m.BiometricId == users.BiometricId);

            Debug.WriteLine("Total Model Count:" + (attendance.Count() - 1));

            var _attendanceLog = attendance.Where(m => m.BiometricId == users.BiometricId).ToList();
            var id = 0;
            foreach (var i in attendance.ToList()
                .Select(x => new { LogDate = x.LogDateTime.ToShortDateString() }).ToList()
                .GroupBy(m => m.LogDate))
            {
                id++;
                var _timeIn = _attendanceLog.OrderBy(m => m.Id).FirstOrDefault(m =>
                            m.LogDateTime.ToShortDateString() == i.Key &&
                            (InOutState)m.InOutState == InOutState.CheckIn)
                        ?.LogDateTime.ToShortTimeString();
                var _timeOut = _attendanceLog.OrderBy(m => m.Id)
                    .FirstOrDefault(m =>
                        m.LogDateTime.ToShortDateString() == i.Key && (InOutState)m.InOutState == InOutState.CheckOut)
                    ?.LogDateTime.ToShortTimeString();


                users.DtrReportViewModels.Add(
                    new DtrReportViewModel()
                    {
                        Id = id,
                        DateLog = i.Key,
                        TimeIn = _timeIn,
                        TimeOut = _timeOut

                    });
            }


            return users;




        }
    }
}


/* while (_attendanceLog <= model.DateLogTo)
             {



                 var logDate = _dateLogFrom.ToShortDateString();

                 i.DtrReportViewModels.Add(new DtrReportViewModel()
                 {
                     LogDate = _dateLogFrom,
                     TimeIn = _attendanceLog.Where(m => m.logDate.Value.ToShortDateString() == logDate && (InOutState)m.InOutState == InOutState.CheckIn).OrderBy(m => m.LogTime).FirstOrDefault()?.LogTime.Value.ToShortTimeString(),
                     TimeOut = _attendanceLog.Where(m => m.logDate.Value.ToShortDateString() == logDate && (InOutState)m.InOutState == InOutState.CheckOut).OrderByDescending(m => m.LogTime).FirstOrDefault()?.LogTime.Value.ToShortTimeString(),
                     DateLogFrom = model.DateLogFrom,
                     DateLogTo = model.DateLogTo

                 });

                 _dateLogFrom = _dateLogFrom.AddDays(1);
             }*/
