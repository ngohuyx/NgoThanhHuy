using IVS.BL;
using IVS.Components;
using IVS.Models.Model;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IVS.Web.Controllers
{
    public class CategoryController : Controller
    {
        public ICategoryBL _categoryBL;
        public const string ScreenController = "Category";
        public CategoryController()
        {
            _categoryBL = new CategoryBL();
        }

        public ActionResult Index()
        {
            CategorySearchModel Model = new CategorySearchModel();
            if (Session[ScreenController] != null)
            {
                Model = (CategorySearchModel)Session[ScreenController];
                Model.searchResultModel = new List<CategoryViewModel>();
                Model.searchResultModel = _categoryBL.Search(Model).ToPagedList(1, 200);
            }
            else
            {
                Model.searchResultModel = new List<CategoryViewModel>().ToPagedList(1, 200);
            }
            return View(Model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(int? page, CategorySearchModel Model)
        {
            Model.searchResultModel = new List<CategoryViewModel>();
            Model.searchResultModel = _categoryBL.Search(Model);
            Session[ScreenController] = Model;
            var pageNumber = (page ?? 1);
            var list = Model.searchResultModel.ToPagedList(pageNumber, 200);
            return PartialView("ListCategory", list);
        }
        #region ADD
        public ActionResult Add()
        {
            CategoryModel Model = new CategoryModel();
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
            return View(Model);
        }

        [HttpPost]
        public ActionResult Add(CategoryModel Model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _categoryBL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
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

            CategoryModel Model = new CategoryModel();
            int returnCode = _categoryBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryModel();
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
            return View(Model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            CategoryModel Model = new CategoryModel();
            int returnCode = _categoryBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryModel();
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(CategoryModel Model)
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
                    ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("View", new { @id = Model.id });
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(true), "id", "name");
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
