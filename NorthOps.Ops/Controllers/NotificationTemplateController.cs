﻿using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    [Authorize]
    public class NotificationTemplateController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private object notificationType;


        // GET: NotificationTemplate
        public NotificationTemplateController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult NotificationsTemplatesGridViewPartial()
        {
            var model = unitOfWork.NotificationTemplatesRepo.Get();
            var notification = model.Select(x => x.Type);
            //ViewBag.NotificationType = Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>()
            //    .Select(x => new { Id = x, Name = x.ToString() });//.Where(x => !notification.Contains((int)x.Id));
            return PartialView("_NotificationsTemplatesGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> NotificationsTemplatesGridViewPartialAddNew(NorthOps.Models.NotificationsTemplates item)
        {
            //   var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.NotificationTemplatesRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.NotificationTemplatesRepo.Get();
            return PartialView("_NotificationsTemplatesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> NotificationsTemplatesGridViewPartialUpdate(NorthOps.Models.NotificationsTemplates item)
        {
            // var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.NotificationTemplatesRepo.Update(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.NotificationTemplatesRepo.Get();
            return PartialView("_NotificationsTemplatesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult NotificationsTemplatesGridViewPartialDelete(System.String Template)
        {
            //   var model = new object[0];
            if (Template != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.NotificationTemplatesRepo.Get();
            return PartialView("_NotificationsTemplatesGridViewPartial", model);
        }
    }
}