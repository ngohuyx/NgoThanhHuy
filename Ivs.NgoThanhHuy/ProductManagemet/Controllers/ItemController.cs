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
    public class ItemController : Controller
    {
        public IItemBL _categoryBL;
        public const string ScreenController = "Item";
        public ItemController()
        {
            _categoryBL = new ItemBL();
        }

        public ActionResult Index()
        {
            ItemSearchModel Model = new ItemSearchModel();
            if (Session[ScreenController] != null)
            {
                Model = (ItemSearchModel)Session[ScreenController];
                Model.searchResultModel = new List<ItemViewModel>();
                Model.searchResultModel = _categoryBL.Search(Model).ToPagedList(1, 100);
            }
            else
            {
                Model.searchResultModel = new List<ItemViewModel>().ToPagedList(1, 100);
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(int? page, ItemSearchModel Model)
        {
            Model.searchResultModel = new List<ItemViewModel>();
            Model.searchResultModel = _categoryBL.Search(Model);
            Session[ScreenController] = Model;
            var pageNumber = (page ?? 1);
            var list = Model.searchResultModel.ToPagedList(pageNumber, 100);
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            return PartialView("ListItem", list);
        }
        #region ADD
        public ActionResult Add()
        {
            ItemModel Model = new ItemModel();
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            ViewBag.Measure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
            return View(Model);
        }

        [HttpPost]
        public ActionResult Add(ItemModel Model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
                ViewBag.Mesure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _categoryBL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
                ViewBag.Mesure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
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

            ItemModel Model = new ItemModel();
            int returnCode = _categoryBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new ItemModel();
            }
            ViewBag.Measure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            ItemModel Model = new ItemModel();
            int returnCode = _categoryBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new ItemModel();
            }
            ViewBag.Measure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(ItemModel Model)
        {
            if (ModelState.IsValid)
            {
                List<string> lstMsg = new List<string>();
                int returnCode = _categoryBL.Update(Model, out lstMsg);

                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    ViewBag.Measure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
                    ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("Index", new { });
            }
            ViewBag.Measure = new SelectList(_categoryBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_categoryBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string lstMsg = string.Empty;

            int returnCode = _categoryBL.Delete(id, out lstMsg);
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
