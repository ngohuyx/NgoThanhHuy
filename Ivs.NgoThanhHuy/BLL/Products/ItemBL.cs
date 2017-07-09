using IVS.DAL.DAO;
using IVS.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.BL
{
    public interface IItemBL
    {
        List<ItemViewModel> Search(ItemSearchModel model, int? page = null);
        List<ListCategory> GetListCategory(bool hasEmpty);
        List<ListMeasure> GetListMeasure(bool hasEmpty);
        int GetByID(long ID, out ItemModel model);
        int Count(ItemSearchModel model);
        int GetDetail(long ID, out ItemViewModel model);
        int Insert(ItemModel model, out List<string> lstMsg);
        int Update(ItemModel model, out List<string> lstMsg);
        int Delete(string id, out string lstMsg);
    }
    public class ItemBL : IItemBL
    {
        public ItemDAO _itemDAO;
        public ItemBL()
        {
            _itemDAO = new ItemDAO();
        }

        public int Delete(string id, out string lstMsg)
        {
            return _itemDAO.Delete(id, out lstMsg);
        }

        public List<ItemViewModel> Search(ItemSearchModel model, int? page = null)
        {
            return _itemDAO.Search(page, model);
        }
        public int GetByID(long ID, out ItemModel model)
        {
            return _itemDAO.GetByID(ID, out model);
        }
        public int GetDetail(long ID, out ItemViewModel model)
        {
            return _itemDAO.GetDetail(ID, out model);
        }

        public int Insert(ItemModel model, out List<string> lstMsg)
        {
            return _itemDAO.Insert(model, out lstMsg);
        }

        public int Update(ItemModel model, out List<string> lstMsg)
        {
            return _itemDAO.Update(model, out lstMsg);
        }
        public List<ListCategory> GetListCategory(bool hasEmpty)
        {
            return _itemDAO.GetListCategory(hasEmpty);
        }

        public List<ListMeasure> GetListMeasure(bool hasEmpty)
        {
            return _itemDAO.GetListMeasure(hasEmpty);
        }

        public int Count(ItemSearchModel model)
        {
            return _itemDAO.Count(model);
        }
    }
}