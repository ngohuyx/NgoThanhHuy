using IVS.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.Models.Model
{
    public class ItemModel : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DisplayName("Category")]
        public int? category_id { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "[Code] must be filled in!")]
        [MaxLength(30, ErrorMessage = "[Code] must be a string with a maximum length of '30'")]
        public string code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "[Name] must be filled in!")]
        [MaxLength(64, ErrorMessage = "[Name] must be a string with a maximum length of '64'")]
        public string name { get; set; }

        [DisplayName("Specification")]
        [MaxLength(256, ErrorMessage = "[Specification] must be a string with a maximum length of '256'")]
        public string specification { get; set; }

        [DisplayName("Description")]
        [MaxLength(256, ErrorMessage = "[Description] must be a string with a maximum length of '256'")]
        public string description { get; set; }

        [DisplayName("Dangerous")]
        public bool dangerous { get; set; }

        [DisplayName("Discontinued")]
        public DateTime? discontinued_datetime { get; set; }

        [DisplayName("Inventory Measure")]
        public int? inventory_measure_id { get; set; }

        [DisplayName("Inventory Expired")]
        public int? inventory_expired { get; set; }

        [DisplayName("Inventory Standard Cost")]
        public double? inventory_standard_cost { get; set; }

        [DisplayName("Inventory List Price")]
        public double? inventory_list_price { get; set; }

        [DisplayName("Manufacture Day")]
        public double? manufacture_day { get; set; }

        [DisplayName("Manufacture Make")]
        public bool manufacture_make { get; set; }

        [DisplayName("Manufacture Tool")]
        public bool manufacture_tool { get; set; }

        [DisplayName("Manufacture Finished Goods")]
        public bool manufacture_finished_goods { get; set; }

        [DisplayName("Manufacture Size")]
        [MaxLength(16, ErrorMessage = "[Manufacture Size] must be a string with a maximum length of '16'")]
        public string manufacture_size { get; set; }

        [DisplayName("Manufacture Size Measure")]
        public int? manufacture_size_measure_id { get; set; }

        [DisplayName("Manufacture Weight")]
        [MaxLength(16, ErrorMessage = "[Manufacture Weight] must be a string with a maximum length of '16'")]
        public string manufacture_weight { get; set; }

        [DisplayName("Manufacture Weight Measure")]
        public int? manufacture_weight_measure_id { get; set; }

        [DisplayName("Manufacture Style")]
        [MaxLength(16, ErrorMessage = "[Manufacture Style] must be a string with a maximum length of '16'")]
        public string manufacture_style { get; set; }

        [DisplayName("Manufacture Class")]
        [MaxLength(16, ErrorMessage = "[Manufacture Class] must be a string with a maximum length of '16'")]
        public string manufacture_class { get; set; }

        [DisplayName("Manufacture Color")]
        [MaxLength(16, ErrorMessage = "[Manufacture Color] must be a string with a maximum length of '16'")]
        public string manufacture_color { get; set; }        
    }

    public class ItemViewModel
    {
        public int? id { get; set; }

        [DisplayName("Category")]
        public string category_name { get; set; }

        [DisplayName("Code")]
        public string code { get; set; }

        [DisplayName("Name")]
        public string name { get; set; }

        [DisplayName("Specification")]
        public string specification { get; set; }

        [DisplayName("Description")]
        public string description { get; set; }        
    }

    public class ItemSearchModel
    {
        [DisplayName("Code")]
        public string code { get; set; }
        [DisplayName("Name")]
        public string name { get; set; }
        public string category { get; set; }
        [DisplayName("Category")]
        public int? category_id { get; set; }
        public IEnumerable<ItemViewModel> searchResultModel;
    }

    public class ListCategory
    {
        public int id { get; set; }
        public int parent_id { get; set; }
        public string name { get; set; }
    }

    public class ListMeasure
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
