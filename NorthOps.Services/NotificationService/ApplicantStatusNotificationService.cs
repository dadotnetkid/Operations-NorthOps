using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.Repository;

namespace NorthOps.Services.NotificationService
{
    public class ApplicantStatusNotificationService : INotificationService
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public async Task Notify(string userId, string message)
        {
            unitOfWork.EmployeeNoticationsRepo.Insert(
                new Models.EmployeeNotications() { Id=Guid.NewGuid().ToString(), UserId = userId, Message = message });
            await unitOfWork.SaveAsync();
        }
    }
}
