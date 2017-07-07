using IVS.DAL.DAO;
using IVS.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.BL
{
    public interface ICategoryBL
    {
        List<CategoryViewModel> Search(CategorySearchModel model);
        List<CategoryParent> GetListParent(bool hasEmpty);
        int GetByID(long ID, out CategoryModel model);
        int GetDetail(long ID, out CategoryViewModel model);
        int Insert(CategoryModel model, out List<string> lstMsg);
        int Update(CategoryModel model, out List<string> lstMsg);
        int Delete(string id, out string lstMsg);
    }
    public class CategoryBL : ICategoryBL
    {
        public CategoryDAO _categoryDAO;
        public CategoryBL()
        {
            _categoryDAO = new CategoryDAO();
        }

        public int Delete(string id, out string lstMsg)
        {
            return _categoryDAO.Delete(id, out lstMsg);
        }

        public List<CategoryViewModel> Search(CategorySearchModel model)
        {
            return _categoryDAO.Search(model);
        }
        public int GetByID(long ID, out CategoryModel model)
        {
            return _categoryDAO.GetByID(ID, out model);
        }
        public int GetDetail(long ID, out CategoryViewModel model)
        {
            return _categoryDAO.GetDetail(ID, out model);
        }

        public List<CategoryParent> GetListParent(bool hasEmpty)
        {
            return _categoryDAO.GetListParent(hasEmpty);
        }

        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Insert(model, out lstMsg);
        }

        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Update(model, out lstMsg);
        }
    }
}
