using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Ops.Repository;
using NorthOps.Ops.Models;
using System.Threading.Tasks;

namespace NorthOps.Ops.Controllers
{
    public class CampaignController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CampaignGridViewPartial()
        {
            var model = unitOfWork.CampaignRepo.Get();
            return PartialView("_CampaignGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CampaignGridViewPartialAddNew(NorthOps.Ops.Models.Campaign item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = Request.Params["UserId"];
                    item.CampaignId = Guid.NewGuid().ToString();
                    foreach (var i in UserId.Split(','))
                    {
                        item.Users.Add(unitOfWork.UserRepository.Find(m => m.Id == i));
                    }
                    await unitOfWork.CampaignRepo.InsertAsync(item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.CampaignRepo.Get();
            return PartialView("_CampaignGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CampaignGridViewPartialUpdate(NorthOps.Ops.Models.Campaign item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = Request.Params["UserId"];
                    var campaign = await unitOfWork.CampaignRepo.FindAsync(m => m.CampaignId == item.CampaignId);
                    campaign.CampaignName = item.CampaignName;
                    campaign.CampaignDescription = item.CampaignDescription;
                    campaign.DateTimePeriodFrom = item.DateTimePeriodFrom;
                    campaign.DateTimePeriodTo = item.DateTimePeriodTo;
                    campaign.Users.Clear();
                    foreach (var i in UserId.Split(','))
                        campaign.Users.Add(await unitOfWork.UserRepository.FindAsync(m => m.Id == i));

                    var res = await unitOfWork.SaveAsync();


                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.CampaignRepo.Get();
            return PartialView("_CampaignGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CampaignGridViewPartialDelete(NorthOps.Ops.Models.Campaign item)
        {

            if (item.CampaignId != null)
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
            var model = unitOfWork.CampaignRepo.Get();
            return PartialView("_CampaignGridViewPartial", model);
        }
        public ActionResult CampaignAddEditGridViewPartial(string _CampaignId)
        {
            ViewBag.CampaignId = _CampaignId;
            var model = unitOfWork.CampaignRepo.Find(m => m.CampaignId == _CampaignId);
            return PartialView("_CampaignAddEditGridViewPartial", model);
        }

        public ActionResult UserTokenPartial(string _CampaignId)
        {

            ViewBag.CampaignId = _CampaignId;

            if (unitOfWork.CampaignRepo.Find(m => m.CampaignId == _CampaignId) != null)
                ViewBag.Users = unitOfWork.CampaignRepo.Find(m => m.CampaignId == _CampaignId).Users;



            var model = unitOfWork.UserRepository.Fetch().Select(x => new { Id = x.Id, Name = x.FirstName + " " + x.LastName }).ToList();
            return PartialView("_UserTokenPartial", model);
        }
    }
}