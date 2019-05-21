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
    public class AttendanceServices : IDailyTimeRecordServices
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public Task<List<Transactions>> AttendanceLog()
        {
            STDDevComm devComm = new STDDevComm(new Machines(ip: "10.10.20.50"));
            var transaction = new List<Transactions>();
            devComm.GetAllTransaction(out transaction);

            return Task.FromResult(transaction);
        }
        /// <summary>
        /// TODO Retrieve Logs from Biometric Device
        /// </summary>
        /// <returns></returns>
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
                        && (i.InOutState == InOutState.CheckIn || i.InOutState == InOutState.CheckOut))
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
        /// <summary>
        /// TODO Retrieve Logs from Biometric Device
        /// </summary>
        /// <returns></returns>

        public async Task SaveAttendanceLog(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var res = (await AttendanceLog()).Where(m => m.LogDateTime >= dateFrom && m.LogDateTime <= dateTo);
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


        /// <summary>
        /// TODO Retrieve Logs from Biometric Device Scheduled Daily
        /// </summary>
        /// <returns></returns>

        public async Task SaveAttendanceLogDaily()
        {
            var res = await AttendanceLog();
            var attendanceRepo = new List<Attendances>();
            foreach (var i in res)
            {
                if (!unitOfWork.AttendancesRepo.Fetch(m =>
                        m.LogDateTime == i.LogDateTime && m.BiometricId == i.Pin &&
                        m.InOutState == (int)i.InOutState).Any()
                    && (i.InOutState == InOutState.CheckIn || i.InOutState == InOutState.CheckOut))
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
        /// <summary>
        /// Description: Get Daily Time Records Daily
        /// </summary>
        public void GetDailyTimeRecordsDaily()
        {
            var users = unitOfWork.UserRepository.Fetch(m => m.UserRoles.Any(x => x.Name == "Employee"),
                includeProperties:
                "Schedules,Schedules.DailyTimeRecords,Overtimes,Overtimes.CreatedByUser,Overtimes.ModifiedByUser,Overtimes.Users");
            int days = Convert.ToInt32(23 +
                       (Convert.ToInt32(DateTime.Now.ToString("dd")) >= 7 && Convert.ToInt32(DateTime.Now.ToString("dd")) <= 23
                    ? DateTime.Now - Convert.ToDateTime($"{DateTime.Now:MM}-23-{DateTime.Now:yy}")
                    : DateTime.Now - Convert.ToDateTime($"{DateTime.Now:MM}-07-{DateTime.Now:yy}")).TotalDays);
            var dateFrom = Convert.ToDateTime(DateTime.Now.AddDays(-days).ToShortDateString() + " 00:00");
            var dateTo = Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString() + " 23:59");


            foreach (var i in users.ToList())
            {
                var attendance = unitOfWork.AttendancesRepo.Get(m =>
                    m.BiometricId == i.BiometricId &&
                    (m.LogDateTime >= dateFrom && m.LogDateTime <= dateTo)).ToList();
                var schedules = unitOfWork.SchedulesRepo.Get(m =>
                        m.UserId == i.Id &&
                        (m.ScheduleDateFrom >= dateFrom && m.ScheduleDateTo <= dateTo))
                    .OrderBy(m => m.ScheduleDateFrom).ToList();

                foreach (var s in schedules)
                {
                    var _dateFrom = Convert.ToDateTime(s.ScheduleDateFrom.ToShortDateString() + " 00:00");
                    var _dateTo = Convert.ToDateTime(s.ScheduleDateTo.ToShortDateString() + " 23:59");

                    var _attendance = attendance.Where(m => (m.LogDateTime >= _dateFrom) && (m.LogDateTime <= _dateTo))
                        .ToList();
                    var dtr = unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.ScheduleId == s.Id);


                    var _from = _attendance.OrderBy(m => m.LogDateTime)
                        .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckIn)?.LogDateTime;
                    var _to = _attendance.OrderByDescending(m => m.LogDateTime)
                        .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckOut)?.LogDateTime;



                    if (dtr.Any(x => x.ModifiedBy == null))
                    {
                        var _dtr = dtr.FirstOrDefault();
                        _dtr.DateFrom = _from < s.ScheduleDateFrom ? s.ScheduleDateFrom : _from;
                        _dtr.DateTo = _to > s.ScheduleDateTo ? s.ScheduleDateTo : _to;

                        unitOfWork.Save();
                    }




                    if (!dtr.Any())
                    {
                        unitOfWork.DailyTimeRecordsRepo.Insert(new DailyTimeRecords()
                        {
                            ScheduleId = s.Id,
                            DateFrom = _from < s.ScheduleDateFrom ? s.ScheduleDateFrom : _from,
                            DateTo = _to > s.ScheduleDateTo ? s.ScheduleDateTo : _to,
                            CreatedBy = HttpContext.Current?.User?.Identity?.GetUserId(),
                            DateCreated = DateTime.Now
                        });
                        unitOfWork.Save();
                    }


                    var __dtr = dtr.FirstOrDefault();
                    __dtr.isAbsent = !_attendance.Any();

                    unitOfWork.Save();

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


        /// <summary>
        /// Description: Get Daily Time Records by user intervention
        /// </summary>
        /// <param name="model"></param>

        public void GetDtrReport(DailyTimeRecordViewModel model)
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


                    var _from = _attendance.OrderBy(m => m.LogDateTime)
                        .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckIn)?.LogDateTime;
                    var _to = _attendance.OrderByDescending(m => m.LogDateTime)
                        .FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckOut)?.LogDateTime;

               



                    if (dtr.Any(x => x.ModifiedBy == null))
                    {
                        var _dtr = dtr.FirstOrDefault();
                        _dtr.DateFrom = _from < s.ScheduleDateFrom ? s.ScheduleDateFrom : _from;
                        _dtr.DateTo = _to > s.ScheduleDateTo ? s.ScheduleDateTo : _to;


                        unitOfWork.Save();
                    }




                    if (!dtr.Any())
                    {
                        unitOfWork.DailyTimeRecordsRepo.Insert(new DailyTimeRecords()
                        {
                            ScheduleId = s.Id,
                            DateFrom = _from < s.ScheduleDateFrom ? s.ScheduleDateFrom : _from,
                            DateTo = _to > s.ScheduleDateTo ? s.ScheduleDateTo : _to,
                            DateCreated = DateTime.Now,
                            CreatedBy = HttpContext.Current.User.Identity.GetUserId(),
         
                        });
                        unitOfWork.Save();
                    }

                    var __dtr = dtr.FirstOrDefault();
                    __dtr.isAbsent = !_attendance.Any();
                   

                    var tFrom = Convert.ToInt32((__dtr.DateFrom - s?.ScheduleDateFrom)?.TotalMinutes);
                    var tTo = Convert.ToInt32((s?.ScheduleDateTo - __dtr.DateTo)?.TotalMinutes);
                    tFrom = tFrom <= 0 ? 0 : tFrom;
                    tTo = tTo <= 0 ? 0 : tTo;
                    int? _tardiness = tFrom + tTo;

                    decimal? _renderedHrs = Convert.ToDecimal(((__dtr.DateTo ?? __dtr.DateFrom ?? DateTime.Now) - (__dtr.DateFrom ?? __dtr.DateTo ?? DateTime.Now)).TotalMinutes / 60);


                    __dtr.Tardiness = _tardiness;
                    __dtr.TotalRenderedHrs = _renderedHrs;
                    unitOfWork.Save();



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

        public List<DailyTimeRecordViewModel> dailyTimeRecords(DailyTimeRecordViewModel model)
        {

            model.DateLogFrom = Convert.ToDateTime(model.DateLogFrom?.ToShortDateString() + " 00:00");
            model.DateLogTo = Convert.ToDateTime(model.DateLogTo?.ToShortDateString() + " 23:59");
            var dailyTimeRecords = new List<DailyTimeRecordViewModel>();

            model.UserId = model.UserId ?? "";
            foreach (var i in unitOfWork.UserRepository.Fetch(m => m.UserRoles.Any(x => x.Name == "Employee") && m.Id.Contains(model.UserId)))
            {

                dailyTimeRecords.Add(new DailyTimeRecordViewModel()
                {
                    RestDays = unitOfWork.RestDaysRepo.Fetch(m => (m.RestDay >= model.DateLogFrom && m.RestDay <= model.DateLogTo) && m.UserId == i.Id).ToList(),
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
            List<DailyTimeRecordViewModel> dtrReportViewModels = new List<DailyTimeRecordViewModel>();
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
                var _timeIn = _attendanceLog.OrderBy(m => m.LogDateTime).FirstOrDefault(m =>
                            m.LogDateTime.ToShortDateString() == i.Key &&
                            (InOutState)m.InOutState == InOutState.CheckIn)
                        ?.LogDateTime.ToShortTimeString();
                var _timeOut = _attendanceLog.OrderBy(m => m.LogDateTime)
                    .FirstOrDefault(m =>
                        m.LogDateTime.ToShortDateString() == i.Key && (InOutState)m.InOutState == InOutState.CheckOut)
                    ?.LogDateTime.ToShortTimeString();


                users.DtrReportViewModels.Add(
                    new DailyTimeRecordViewModel()
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
