using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;
using DevExpress.Web.ASPxScheduler;

namespace NorthOps.Ops.Controllers
{
    public class CampaignsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Campaigns
        public ActionResult Index()
        {
            return View();
        }
        #region Partials
        public ActionResult TokenBoxCampaignPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string ShiftId)
        {
            var model = unitOfWork.ShiftsRepo.Find(m => m.Id == ShiftId);
            ViewBag.ShiftId = ShiftId;
            return PartialView("TokenBoxCampaignPartial", model);
        }

        public ActionResult TokenBoxShiftPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string CampaignId)
        {
            var model = unitOfWork.CampaignsRepo.Find(m => m.Id == CampaignId);
            ViewBag.CampaignId = CampaignId;
            return PartialView("TokenBoxShiftPartial", model);
        }

        public ActionResult TokenBoxUsersPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string ShiftId)
        {
            var model = unitOfWork.ShiftsRepo.Find(m => m.Id == ShiftId);
            return PartialView("TokenBoxUsersPartial", model);
        }

        public ActionResult cboEmployeesPartial(string UserId = "")
        {
            ViewBag.UserId = UserId;
            var model = unitOfWork.UserRepository.Get();
            return PartialView("_cboEmployeesPartial", model);
        }

        public ActionResult cboCampaignsPartial(string CampaignId = "")
        {
            ViewBag.CampaignId = CampaignId;
            var model = unitOfWork.CampaignsRepo.Get();
            return PartialView("_cboCampaignsPartial", model);
        }

        public ActionResult cboShiftsPartial(string CampaignId = "", string ShiftId = "")
        {
            ViewBag.ShiftId = ShiftId;
            var model = unitOfWork.ShiftsRepo.Get(m => m.Campaigns.Any(x => x.Id == CampaignId));
            return PartialView("_cboShiftsPartial", model);
        }
        #endregion
        #region Campaign
        [ValidateInput(false)]
        public ActionResult CampaignsGridViewPartial()
        {
            var model = unitOfWork.CampaignsRepo.Get();
            return PartialView("_CampaignsGridViewPartial", model);
        }

        public ActionResult AddEditCampaignPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string CampaignId)
        {
            var model = unitOfWork.CampaignsRepo.Find(m => m.Id == CampaignId);
            return PartialView("AddEditCampaignPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CampaignsGridViewPartialAddNew([ModelBinder(typeof(DevExpress.Web.Mvc.DevExpressEditorsBinder))]NorthOps.Models.Campaigns item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var shifts = Request["ShiftId"];

                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.CampaignsRepo.Insert(item);
                    #region Adding Shifts in Campaign
                    var campaign = unitOfWork.CampaignsRepo.Find(m => m.Id == item.Id, includeProperties: "Shifts");
                    campaign?.Shifts?.Clear();
                    if (!string.IsNullOrEmpty(shifts))
                        foreach (var i in shifts.Split(','))
                        {
                            campaign.Shifts.Add(unitOfWork.ShiftsRepo.Find(m => m.Id == i));
                        }
                    #endregion
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.CampaignsRepo.Get();
            return PartialView("_CampaignsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CampaignsGridViewPartialUpdate([ModelBinder(typeof(DevExpress.Web.Mvc.DevExpressEditorsBinder))]NorthOps.Models.Campaigns item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                    unitOfWork.CampaignsRepo.Update(item);
                    //await unitOfWork.SaveAsync();
                    var shifts = Request["ShiftId"];
                    #region Adding Shifts in Campaign
                    var campaign = unitOfWork.CampaignsRepo.Find(m => m.Id == item.Id, includeProperties: "Shifts");
                    campaign.Shifts.Clear();
                    foreach (var i in shifts.Split(','))
                    {
                        campaign.Shifts.Add(unitOfWork.ShiftsRepo.Find(m => m.Id == i));
                    }
                    #endregion
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.CampaignsRepo.Get();
            return PartialView("_CampaignsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CampaignsGridViewPartialDelete([ModelBinder(typeof(DevExpress.Web.Mvc.DevExpressEditorsBinder))]System.String Id)
        {

            if (Id != null)
            {
                try
                {
                    unitOfWork.CampaignsRepo.Delete(unitOfWork.CampaignsRepo.Find(m => m.Id == Id));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.CampaignsRepo.Get();
            return PartialView("_CampaignsGridViewPartial", model);
        }


        #endregion

        #region Shifts
        public ActionResult Shifts()
        {
            return View();
        }
        public ActionResult AddEditShiftPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string ShiftId)
        {
            var model = unitOfWork.ShiftsRepo.Find(m => m.Id == ShiftId);
            return PartialView("AddEditShiftPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult ShiftsGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string CampaignId, bool IsHeaderVisible = false)
        {
            ViewBag.CampaignId = CampaignId;
            ViewBag.IsHeaderVisible = IsHeaderVisible;
            var model = CampaignId == null ? unitOfWork.ShiftsRepo.Get() :
                unitOfWork.ShiftsRepo.Get(filter: m => m.Campaigns.Any(n => n.Id == CampaignId));

            return PartialView("_ShiftsGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ShiftsGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Shifts item, bool IsHeaderVisible = false)
        {
            ViewBag.IsHeaderVisible = IsHeaderVisible;
            if (ModelState.IsValid)
            {
                try
                {
                    var campaigns = Request["CampaignId"];
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.ShiftsRepo.Insert(item);
                    #region Adding Campaign in Shifts
                    var shifts = unitOfWork.ShiftsRepo.Find(m => m.Id == item.Id, includeProperties: "Campaigns,Users");

                    shifts.Campaigns.Clear();
                    foreach (var i in campaigns.Split(','))
                        shifts.Campaigns.Add(unitOfWork.CampaignsRepo.Find(m => m.Id == i));
                    #endregion
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = unitOfWork.ShiftsRepo.Get();
            return PartialView("_ShiftsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ShiftsGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Shifts item, bool IsHeaderVisible = false)
        {
            ViewBag.IsHeaderVisible = IsHeaderVisible;
            if (ModelState.IsValid)
            {
                try
                {
                    var campaigns = Request["CampaignId"];
                    var users = Request["UserId"];
                    unitOfWork.ShiftsRepo.Update(item);

                    #region Adding Campaigns in Shift
                    var shifts = unitOfWork.ShiftsRepo.Find(m => m.Id == item.Id, includeProperties: "Campaigns,Users");
                    shifts.Campaigns.Clear();
                    foreach (var i in campaigns.Split(','))
                        shifts.Campaigns.Add(unitOfWork.CampaignsRepo.Find(m => m.Id == i));
                    #endregion

                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = unitOfWork.ShiftsRepo.Get();
            return PartialView("_ShiftsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ShiftsGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]System.String Id, bool IsHeaderVisible = false)
        {
            ViewBag.IsHeaderVisible = IsHeaderVisible;
            if (Id != null)
            {
                try
                {
                    unitOfWork.ShiftsRepo.Delete(unitOfWork.ShiftsRepo.Find(m => m.Id == Id));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.ShiftsRepo.Get();
            return PartialView("_ShiftsGridViewPartial", model);
        }
        #endregion







        #region Users in Campaign Shifts

        public ActionResult UsersCampaignShift()
        {
            return View();
        }

        public ActionResult AddEditUsersInCampaignShiftPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string UserInCampaignShiftId)
        {
            var model = unitOfWork.UsersInCampaignShiftRepo.Find(m => m.Id == UserInCampaignShiftId);
            return PartialView("AddEditUsersInCampaignShiftPartial", model);
        }
        [ValidateInput(false)]
        public ActionResult UsersInCampaignShiftGridViewPartial()
        {
            var model = unitOfWork.UsersInCampaignShiftRepo.Get(includeProperties: "Users,Campaigns,Shifts");
            return PartialView("_UsersInCampaignShiftGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UsersInCampaignShiftGridViewPartialAddNew(NorthOps.Models.UsersInCampaignShift item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.UsersInCampaignShiftRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = unitOfWork.UsersInCampaignShiftRepo.Get(includeProperties: "Users,Campaigns,Shifts");
            return PartialView("_UsersInCampaignShiftGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UsersInCampaignShiftGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.UsersInCampaignShift item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.UsersInCampaignShiftRepo.Update(item);
                    await unitOfWork.SaveAsync();

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = unitOfWork.UsersInCampaignShiftRepo.Get(includeProperties: "Users,Campaigns,Shifts");
            return PartialView("_UsersInCampaignShiftGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UsersInCampaignShiftGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]System.String Id)
        {

            if (Id != null)
            {
                try
                {
                    unitOfWork.UsersInCampaignShiftRepo.Delete(unitOfWork.UsersInCampaignShiftRepo.Find(m => m.Id == Id));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.UsersInCampaignShiftRepo.Get(includeProperties: "Users,Campaigns,Shifts");
            return PartialView("_UsersInCampaignShiftGridViewPartial", model);
        }

        public ActionResult BulkAddEditUsersInCampaignPartial()
        {
            return PartialView("BulkAddEditUsersInCampaignPartial");
        }
        #endregion



    }


}