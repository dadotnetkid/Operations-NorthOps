using DevExpress.Web.Mvc;
using NorthOps.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult InventoryGridViewPartial()
        {
            var model = unitOfWork.InventoryRepo.Get(includeProperties: "Items,Items.ItemTypes");
            return PartialView("_InventoryGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> InventoryGridViewPartialAddNew(NorthOps.Models.Inventory item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.InventoryRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.InventoryRepo.Get(includeProperties: "Items,Items.ItemTypes");
            return PartialView("_InventoryGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryGridViewPartialUpdate(NorthOps.Models.Inventory item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.InventoryRepo.Get(includeProperties: "Items,Items.ItemTypes");
            return PartialView("_InventoryGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryGridViewPartialDelete(System.String Id)
        {
            if (Id != null)
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
            var model = unitOfWork.InventoryRepo.Get(includeProperties: "Items,Items.ItemTypes");
            return PartialView("_InventoryGridViewPartial", model);
        }
        public ActionResult CboAgentsPartial()
        {
            var model = unitOfWork.UserRepository.Get() ;
            return PartialView("_cboAgentsPartial", model);
        }
    }
}