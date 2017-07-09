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

namespace IVS.DAL.DAO
{
    public class ItemDAO
    {
        public IDbConnection _db;
        public ItemDAO()
        {
            _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }

        public List<ItemViewModel> Search(int? page, ItemSearchModel model)
        {
            int? start;
            if (page != null)
            {
                start = (page - 1) * 200;
            }
            else start = 0;
            string strQuery = "SELECT item.`id`, item.`code`, item.`name`, cate.`name` category_name, item.`specification`, item.`description`  ";
            strQuery += " FROM `product_item` item LEFT JOIN (SELECT c.`id`, cpp.`name` AS category_parent_name, c.`name` FROM product_category c Left JOIN (SELECT cp.`name`, cp.`id` FROM product_category cp ) cpp on cpp.`id` = c.`parent_id`) cate ON item.`category_id` = cate.`id` WHERE 1 ";
            strQuery += ((model.category_id) != null) ? " AND (item.`category_id` = @category OR item.`category_id` IN (SELECT cate.`id` FROM product_category cate WHERE cate.`parent_id` = @category ))" : "";
            strQuery += (!string.IsNullOrEmpty(model.code)) ? " AND item.`code` LIKE @code" : "";
            strQuery += (!string.IsNullOrEmpty(model.name)) ? " AND item.`name` LIKE @name" : "";
            strQuery += " LIMIT @start,200 ";
            var result = _db.Query<ItemViewModel>(strQuery, new { code = '%' + model.code + '%', name = '%' + model.name + '%', category = model.category_id, start = start }).ToList();
            return result;
        }
        //Insert data 
        public int Insert(ItemModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                var strQuery = "INSERT INTO `product_item` (`category_id`, `code`, `name`, `specification`, `description`, `dangerous`";
                strQuery += " ,`discontinued_datetime`, `inventory_measure_id`, `inventory_expired`, `inventory_standard_cost`, `inventory_list_price`";
                strQuery += " , `manufacture_day`, `manufacture_make`, `manufacture_tool`, `manufacture_finished_goods`, `manufacture_size`, `manufacture_size_measure_id`";
                strQuery += " , `manufacture_weight`, `manufacture_weight_measure_id`, `manufacture_style`, `manufacture_class`, `manufacture_color`)";

                strQuery += " VALUES (@category_id, @code, @name, @specification, @description, @dangerous, @discontinued_datetime, @inventory_measure_id, @inventory_expired";
                strQuery += " , @inventory_standard_cost, @inventory_list_price, @manufacture_day, @manufacture_make, @manufacture_tool, @manufacture_finished_goods";
                strQuery += " , @manufacture_size, @manufacture_size_measure_id, @manufacture_weight, @manufacture_weight_measure_id, @manufacture_style, @manufacture_class, @manufacture_color)";

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
        public int Update(ItemModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();

            try
            {
                if (isError(model, (int)Common.ActionType.Update, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_item` SET `category_id` = @category_id, `code` = @code, `name` = @name, `specification` = @specification,";
                strQuery += " `description` = @description, `dangerous` = @dangerous, `discontinued_datetime` = @discontinued_datetime, `inventory_measure_id` = @inventory_measure_id,";
                strQuery += " `inventory_expired` = @inventory_expired, `inventory_standard_cost` = @inventory_standard_cost, `inventory_list_price` = @inventory_list_price,";
                strQuery += " `manufacture_day` = @manufacture_day, `manufacture_make` = @manufacture_make, `manufacture_tool` = @manufacture_tool, `manufacture_finished_goods` = manufacture_finished_goods,";
                strQuery += " `manufacture_size` = @manufacture_size, `manufacture_size_measure_id` = @manufacture_size_measure_id, `manufacture_weight` = @manufacture_weight,";
                strQuery += " `manufacture_weight_measure_id` = @manufacture_weight_measure_id, `manufacture_style` = @manufacture_style, `manufacture_class` = @manufacture_class, `manufacture_color` = @manufacture_color WHERE `id` = @id";

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
        public int GetByID(long ID, out ItemModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new ItemModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT * FROM product_item WHERE `id` = @id";
                    Model = _db.Query<ItemModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }
        //Get data by Id for view info
        public int GetDetail(long ID, out ItemViewModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new ItemViewModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT `id`, `code`, `name`, `description`";
                    strQuery += " FROM product_item WHERE `id` = @id";
                    Model = _db.Query<ItemViewModel>(strQuery, new { id = ID }).SingleOrDefault();
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
                strQuery += " DELETE FROM `product_item` WHERE `id` = @id; ";
                _db.Execute(strQuery, new { id = ID });
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
                lstMsg = "Exception Occurred.";
            }

            return returnCode;
        }

        //Get list parent for combobox
        public List<ListCategory> GetListCategory(bool hasEmpty)
        {
            List<ListCategory> result = new List<ListCategory>();
            //if (hasEmpty)
            //{
            //    result.Add(new ListCategory() { id = 0, name = "", parent_id = 0 });
            //}
            string strQuery = "SELECT `id`, `parent_id`, `name` FROM `product_category` ORDER BY CASE WHEN parent_id = 0 THEN id ELSE parent_id END, id";
            result.AddRange(_db.Query<ListCategory>(strQuery).ToList());
            return result;
        }
        //Get list parent for combobox
        public List<ListMeasure> GetListMeasure(bool hasEmpty)
        {
            List<ListMeasure> result = new List<ListMeasure>();
            string strQuery = "SELECT `id`, `name` FROM `product_measure` ORDER BY id";
            result.AddRange(_db.Query<ListMeasure>(strQuery).ToList());
            return result;
        }
        //Check error of model
        private bool isError(ItemModel Model, int ActionType, out List<string> lstMessage)
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
                string strQuery = "SELECT `id` FROM `product_item` WHERE `code` = @code LIMIT 1 ";
                var hasItem = _db.Query<int>(strQuery, new { code = Model.code }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_item` WHERE `code` = @code AND `id` <> @id LIMIT 1";
                var hasItem = _db.Query<int>(strQuery, new { code = Model.code, id = Model.id }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }

        public int Count(ItemSearchModel model)
        {
            int result;
            string strQuery = "SELECT COUNT(item.id) ";
            strQuery += " FROM `product_item` item LEFT JOIN (SELECT c.`id`, cpp.`name` AS category_parent_name, c.`name` FROM product_category c Left JOIN (SELECT cp.`name`, cp.`id` FROM product_category cp ) cpp on cpp.`id` = c.`parent_id`) cate ON item.`category_id` = cate.`id` WHERE 1 ";
            strQuery += ((model.category_id) != null) ? " AND (item.`category_id` = @category OR item.`category_id` IN (SELECT cate.`id` FROM product_category cate WHERE cate.`parent_id` = @category ))" : "";
            strQuery += (!string.IsNullOrEmpty(model.code)) ? " AND item.`code` LIKE @code" : "";
            strQuery += (!string.IsNullOrEmpty(model.name)) ? " AND item.`name` LIKE @name" : "";
            result = (_db.ExecuteScalar<int>(strQuery, new { code = '%' + model.code + '%', name = '%' + model.name + '%', category = model.category_id }));
            return result;
        }
    }
}