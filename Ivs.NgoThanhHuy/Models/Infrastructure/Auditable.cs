using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.Models.Infrastructure
{
    public abstract class Auditable
    {
        public DateTime? created_datetime { set; get; }

        public int? created_by { set; get; }

        public DateTime? updated_datetime { set; get; }

        public int? updated_by { set; get; }
    } 
}
