using Dapper;
using IVS.Models.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IVS.Components;
using System;
using System.Transactions;
using System.Management;
using System.Configuration;

namespace IVS.DAL.DAO
{
    public class CategoryDAO
    {
        public IDbConnection _db;
        public CategoryDAO()
        {
            _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }
        //Find data with condition
        public List<CategoryViewModel> Search(CategorySearchModel model)
        {
            string strQuery = "SELECT cate.`id`, cate_parent.`name` parent_name, cate.`code`, cate.`name`, cate.`description`";
            strQuery += " FROM `product_category` cate LEFT JOIN (SELECT `id`, `name` FROM `product_category`) cate_parent";
            strQuery += " ON cate.`parent_id` = cate_parent.id WHERE 1";
            if(!string.IsNullOrEmpty(model.code) || !string.IsNullOrEmpty(model.name))
            {
                strQuery += (!string.IsNullOrEmpty(model.code)) ? " AND cate.`code` LIKE @code" : "";
                strQuery += (!string.IsNullOrEmpty(model.name)) ? " AND cate.`name` LIKE @name" : "";
                strQuery += " OR cate.`parent_id` IN (SELECT `id` FROM product_category WHERE 1";
                strQuery += (!string.IsNullOrEmpty(model.code)) ? " AND `code` LIKE @code" : "";
                strQuery += (!string.IsNullOrEmpty(model.name)) ? " AND `name` LIKE @name" : "";
                strQuery += ")";
            }
            var result = _db.Query<CategoryViewModel>(strQuery, new { code = '%' + model.code + '%', name = '%' + model.name + '%' }).ToList();
            return result;
        }
        //Get list parent for combobox
        public List<CategoryParent> GetListParent(bool hasEmpty)
        {
            List<CategoryParent> result = new List<CategoryParent>();
            if (hasEmpty)
            {
                result.Add(new CategoryParent() { id = 0, name = "", parent_id = 0 });
            }
            string strQuery = "SELECT `id`, `parent_id`, `name` FROM `product_category` ORDER BY `parent_id`";
            result.AddRange(_db.Query<CategoryParent>(strQuery).ToList());
            return result;
        }
        //Insert data 
        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                var strQuery = "INSERT INTO `product_category` (`parent_id`, `code`, `name`, `description`) VALUES(@parent_id, @code, @name, @description); ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }
            return result;
        }
        //Update data
        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();

            try
            {
                if (isError(model, (int)Common.ActionType.Update, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_category` SET `parent_id` = @parent_id, `code` = @code, `name` = @name, `description` = @description, `updated_datetime` = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF") + "' WHERE `id` = @id ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }

            return result;
        }
        //Get data by ID for Update
        public int GetByID(long ID, out CategoryModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT *";
                    strQuery += " FROM product_category";
                    strQuery += " WHERE `id` = @id";
                    Model = _db.Query<CategoryModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }
        //Get data by Id for view info
        public int GetDetail(long ID, out CategoryViewModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryViewModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT cate.`id`, cate_parent.`name` parent_name, cate.`code`, cate.`name`, cate.`description`";
                    strQuery += " FROM product_category cate LEFT JOIN(SELECT `id`, `name` FROM `product_category`) cate_parent";
                    strQuery += " ON cate.`parent_id` = cate_parent.id WHERE cate.`id` = @id";
                    Model = _db.Query<CategoryViewModel>(strQuery, new { id = ID }).SingleOrDefault();
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
                using (var transactionScope = new TransactionScope())
                {
                    string strQuery = "UPDATE `product_category` SET `parent_id` = 0 WHERE `parent_id` = @id; ";
                    strQuery += " UPDATE `product_item` SET `category_id` = 0 WHERE `category_id` = @id; ";
                    strQuery += " DELETE FROM `product_category` WHERE `id` = @id; ";
                    _db.Execute(strQuery, new { id = ID });
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
                lstMsg = "Exception Occurred.";
            }

            return returnCode;
        }

        private bool isError(CategoryModel Model, int ActionType, out List<string> lstMessage)
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
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code LIMIT 1 ";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code AND `id` <> @id LIMIT 1";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code, id = Model.id }).ToList();
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
