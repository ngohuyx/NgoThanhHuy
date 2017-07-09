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
        public IItemBL _itemBL;
        public const string ScreenController = "Item";
        public ItemController()
        {
            _itemBL = new ItemBL();
        }

        public ActionResult ItemSearch()
        {
            ItemSearchModel Model = new ItemSearchModel();
            if (Session[ScreenController] != null)
            {
                Model = (ItemSearchModel)Session[ScreenController];
                int Count = _itemBL.Count(Model);
                Model.searchResultModel = new List<ItemViewModel>();
                Model.searchResultModel = _itemBL.Search(Model);
                Model.searchResultModel = new StaticPagedList<ItemViewModel>(Model.searchResultModel, 1, 200, Count);

            }
            else
            {
                Model.searchResultModel = new List<ItemViewModel>();
                Model.searchResultModel = new StaticPagedList<ItemViewModel>(Model.searchResultModel, 1, 200, 0);
            }
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            ViewBag.No = 0;
            return View(Model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ItemSearch(int? page, ItemSearchModel Model)
        {

            List<ItemViewModel> list = new List<ItemViewModel>();
            list = _itemBL.Search(Model, page);
            if (page.HasValue && Session[ScreenController] != null)
            {
                Model = (ItemSearchModel)Session[ScreenController];
            }
            else
            {
                Session[ScreenController] = Model;
            }

            int Count = _itemBL.Count(Model);
            TempData["SearchCount"] = Count + " row(s)";
            var pageNumber = (page ?? 1);
            ViewBag.No = (pageNumber - 1) * 200;
            Model.searchResultModel = new StaticPagedList<ItemViewModel>(list, pageNumber, 200, Count);
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            ViewBag.CurrentFilter = Model;
            return PartialView("ListItem", Model.searchResultModel);
        }
        #region ADD
        public ActionResult ItemAdd()
        {
            ItemModel Model = new ItemModel();
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            ViewBag.Measure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
            return View(Model);
        }

        [HttpPost]
        public ActionResult ItemAdd(ItemModel Model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
                ViewBag.Mesure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _itemBL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
                ViewBag.Mesure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
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
            return RedirectToAction("ItemSearch");
        }
        #endregion
        #region View & Edit
        public ActionResult ItemView(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("ItemSearch");
            }

            ItemModel Model = new ItemModel();
            int returnCode = _itemBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("ItemSearch");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new ItemModel();
            }
            ViewBag.Measure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        public ActionResult ItemEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("ItemSearch");
            }

            ItemModel Model = new ItemModel();
            int returnCode = _itemBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("ItemSearch");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new ItemModel();
            }
            ViewBag.Measure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        [HttpPost]
        public ActionResult ItemEdit(ItemModel Model)
        {
            if (ModelState.IsValid)
            {
                List<string> lstMsg = new List<string>();
                int returnCode = _itemBL.Update(Model, out lstMsg);

                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    ViewBag.Measure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
                    ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("ItemSearch", new { });
            }
            ViewBag.Measure = new SelectList(_itemBL.GetListMeasure(true), "id", "name");
            ViewBag.Parent = new SelectList(_itemBL.GetListCategory(true), "id", "name");
            return View(Model);
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string lstMsg = string.Empty;

            int returnCode = _itemBL.Delete(id, out lstMsg);
            if ((int)Common.ReturnCode.Succeed == returnCode)
            {
                TempData["Success"] = "Deleted Successfully!";
            }
            else
            {
                TempData["Error"] = lstMsg;
            }
            return RedirectToAction("ItemSearch");
        }
        #endregion
    }
}
