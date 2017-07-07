using IVS.DAL.DAO;
using IVS.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.BL
{
    public interface IMeasureBL
    {
        List<MeasureViewModel> Search(MeasureSearchModel model);        
        int GetByID(long ID, out MeasureModel model);
        int GetDetail(long ID, out MeasureViewModel model);
        int Insert(MeasureModel model, out List<string> lstMsg);
        int Update(MeasureModel model, out List<string> lstMsg);
        int Delete(string id, out string lstMsg);
    }
    public class MeasureBL : IMeasureBL
    {
        public MeasureDAO _measureDAO;
        public MeasureBL()
        {
            _measureDAO = new MeasureDAO();
        }
        public int Delete(string id, out string lstMsg)
        {
            return _measureDAO.Delete(id, out lstMsg);
        }

        public int GetByID(long ID, out MeasureModel model)
        {
            return _measureDAO.GetByID(ID, out model);
        }

        public int GetDetail(long ID, out MeasureViewModel model)
        {
            return _measureDAO.GetDetail(ID, out model);
        }

        public int Insert(MeasureModel model, out List<string> lstMsg)
        {
            return _measureDAO.Insert(model, out lstMsg);
        }

        public List<MeasureViewModel> Search(MeasureSearchModel model)
        {
            return _measureDAO.Search(model);
        }

        public int Update(MeasureModel model, out List<string> lstMsg)
        {
            return _measureDAO.Update(model, out lstMsg);
        }
    }
}
