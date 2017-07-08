using Dapper;
using IVS.Components;
using IVS.Models.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IVS.DAL.DAO
{
    public class MeasureDAO
    {
        public IDbConnection _db;
        public MeasureDAO()
        {
            _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }

        public List<MeasureViewModel> Search(MeasureSearchModel model)
        {
            string strQuery = "SELECT `id`, `code`, `name`, `description`";
            strQuery += " FROM product_measure WHERE 1";
            if (!string.IsNullOrEmpty(model.code) || !string.IsNullOrEmpty(model.name))
            {
                strQuery += (!string.IsNullOrEmpty(model.code)) ? " AND `code` LIKE @code" : "";
                strQuery += (!string.IsNullOrEmpty(model.name)) ? " AND `name` LIKE @name" : "";
            }
            var result = _db.Query<MeasureViewModel>(strQuery, new { code = '%' + model.code + '%', name = '%' + model.name + '%' }).ToList();
            return result;
        }
        //Insert data 
        public int Insert(MeasureModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                var strQuery = "INSERT INTO `product_measure` (`code`, `name`, `description`) VALUES(@code, @name, @description);";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred." + ex);
                result = (int)Common.ReturnCode.UnSuccess;
            }
            return result;
        }
        //Update data
        public int Update(MeasureModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();

            try
            {
                if (isError(model, (int)Common.ActionType.Update, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_measure` SET `code` = @code, `name` = @name, `description` = @description, `updated_datetime` = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF") + "' WHERE `id` = @id ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred." + ex);
                result = (int)Common.ReturnCode.UnSuccess;
            }

            return result;
        }
        //Get data by ID for Update
        public int GetByID(long ID, out MeasureModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new MeasureModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT * FROM product_measure WHERE `id` = @id";
                    Model = _db.Query<MeasureModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }
        //Get data by Id for view info
        public int GetDetail(long ID, out MeasureViewModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new MeasureViewModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT `id`, `code`, `name`, `description`";
                    strQuery += " FROM product_measure WHERE `id` = @id";
                    Model = _db.Query<MeasureViewModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int Delete(string ID, out string lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(ID))
                {
                    lstMsg = "Data has already been deleted by other user!";
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_item` SET `inventory_measure_id` = 0, `manufacture_size_measure_id` = 0, `manufacture_weight_measure_id` = 0 WHERE `category_id` = @id; ";
                strQuery += " DELETE FROM `product_measure` WHERE `id` = @id; ";
                _db.Execute(strQuery, new { id = ID });
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
                lstMsg = "Exception Occurred." + ex;
            }

            return returnCode;
        }

        //Check error of model
        private bool isError(MeasureModel Model, int ActionType, out List<string> lstMessage)
        {
            bool isError = false;
            lstMessage = new List<string>();
            if (Model.code.Contains(" "))
            {
                isError = true;
                lstMessage.Add("[Code] must not contains space character!");
            }
            if ((int)Common.ActionType.Add == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_measure` WHERE `code` = @code LIMIT 1 ";
                var hasItem = _db.Query<int>(strQuery, new { code = Model.code }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_measure` WHERE `code` = @code AND `id` <> @id LIMIT 1";
                var hasItem = _db.Query<int>(strQuery, new { code = Model.code, id = Model.id }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }
    }
}
