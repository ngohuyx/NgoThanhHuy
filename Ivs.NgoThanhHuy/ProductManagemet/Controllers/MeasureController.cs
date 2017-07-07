using IVS.BL;
using IVS.Components;
using IVS.Models.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IVS.Web.Controllers
{
    public class MeasureController : Controller
    {
        public IMeasureBL _measureBL;
        public const string ScreenController = "Measure";
        public MeasureController()
        {
            _measureBL = new MeasureBL();
        }
        public ActionResult Index()
        {
            MeasureSearchModel Model = new MeasureSearchModel();
            if (Session[ScreenController] != null)
            {
                Model = (MeasureSearchModel)Session[ScreenController];
                Model.searchResultModel = new List<MeasureViewModel>();
                Model.searchResultModel = _measureBL.Search(Model).ToPagedList(1, 50);
            }
            else
            {
                Model.searchResultModel = new List<MeasureViewModel>().ToPagedList(1, 50);
            }
            return View(Model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(int? page, MeasureSearchModel Model)
        {
            Model.searchResultModel = new List<MeasureViewModel>();
            Model.searchResultModel = _measureBL.Search(Model);
            Session[ScreenController] = Model;
            var pageNumber = (page ?? 1);
            var list = Model.searchResultModel.ToPagedList(pageNumber, 50);
            return PartialView("ListMeasure", list);
        }
        #region ADD
        public ActionResult Add()
        {
            MeasureModel Model = new MeasureModel();
            return View(Model);
        }

        [HttpPost]
        public ActionResult Add(MeasureModel Model)
        {
            if (!ModelState.IsValid)
            {
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _measureBL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                if (lstMsg != null)
                {
                    for (int i = 0; i < lstMsg.Count(); i++)
                    {
                        ModelState.AddModelError(string.Empty, lstMsg[i]);
                    }
                }
                return View(Model);
            }
            TempData["Success"] = "Inserted Successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region View & Edit
        public ActionResult View(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            MeasureViewModel Model = new MeasureViewModel();
            int returnCode = _measureBL.GetDetail(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new MeasureViewModel();
            }

            return View(Model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            MeasureModel Model = new MeasureModel();
            int returnCode = _measureBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new MeasureModel();
            }
            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(MeasureModel Model)
        {
            if (ModelState.IsValid)
            {
                List<string> lstMsg = new List<string>();
                int returnCode = _measureBL.Update(Model, out lstMsg);

                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("View", new { @id = Model.id });
            }
            return View(Model);
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string lstMsg = string.Empty;

            int returnCode = _measureBL.Delete(id, out lstMsg);
            if ((int)Common.ReturnCode.Succeed == returnCode)
            {
                TempData["Success"] = "Deleted Successfully!";
            }
            else
            {
                TempData["Error"] = lstMsg;
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
