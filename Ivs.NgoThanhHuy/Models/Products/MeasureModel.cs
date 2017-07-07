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
    public class MeasureModel : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "[Code] must be filled in!")]
        [MaxLength(16, ErrorMessage = "[Code] must be a string with a maximum length of '16'")]
        public string code { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "[Name] must be filled in!")]
        [MaxLength(64, ErrorMessage = "[Name] must be a string with a maximum length of '64'")]
        public string name { get; set; }
        [DisplayName("Description")]
        [MaxLength(256, ErrorMessage = "[Description] must be a string with a maximum length of '256'")]
        public string description { get; set; }
    }
    public class MeasureViewModel
    {
        public int id { get; set; }
        [DisplayName("Code")]
        public string code { get; set; }
        [DisplayName("Name")]
        public string name { get; set; }
        [DisplayName("Description")]
        public string description { get; set; }
    }
    public class MeasureSearchModel
    {
        [DisplayName("Code")]
        public string code { get; set; }
        [DisplayName("Name")]
        public string name { get; set; }
        public IEnumerable<MeasureViewModel> searchResultModel;
    }
}
