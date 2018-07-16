using NorthOps.Models.Repository;
using NorthOps.Services.SchedulerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthOps.Scheduler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Debug.WriteLine(Convert.ToDateTime("01-02-2018").ToLongDateString());
        }
        void r()
        {
            var unitOfWork = new UnitOfWork();
            foreach (var i in unitOfWork.UserRepository.Get(includeProperties: "RestDays"))
            {
                for (var d = 1; d <= 7; d++)
                {

                    if (i.RestDays.Count() < 2)
                    {
                        if (unitOfWork.RestDaysRepo.Get().Where(n => n.RestDate == Convert.ToDateTime($"01-{d}-2018")).Count() <= 3)
                        {
                            unitOfWork.RestDaysRepo.Insert(new Models.RestDays()
                            {
                                UserId = i.Id,
                                RestDate = Convert.ToDateTime($"01-{d}-2018")

                            });
                            unitOfWork.Save();
                        }
                        else
                        if (!i.RestDays.Where(m => m.RestDate == Convert.ToDateTime($"01-{d}-2018")).Any())
                        {
                            unitOfWork.SchedulesRepo.Insert(new Models.Schedules()
                            {
                                Id=Guid.NewGuid().ToString(),
                                ScheduleDate = Convert.ToDateTime($"01-{d}-2018"),
                                UserId = i.Id
                            });
                            unitOfWork.Save();
                        }
                    }
                    else
                    {
                        if (!i.RestDays.Where(m => m.RestDate == Convert.ToDateTime($"01-{d}-2018")).Any())
                        {
                            unitOfWork.SchedulesRepo.Insert(new Models.Schedules()
                            {
                                Id = Guid.NewGuid().ToString(),
                                ScheduleDate = Convert.ToDateTime($"01-{d}-2018"),
                                UserId = i.Id
                            });
                            unitOfWork.Save();
                        }

                    }



                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            r();
        }
    }
}
