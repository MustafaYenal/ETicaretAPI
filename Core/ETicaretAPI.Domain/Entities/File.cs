using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class File:BaseEntity
    {
        [NotMapped]
        public override DateTime UpdatedDate { get => base.UpdatedDate; set => base.UpdatedDate = value; }

        public string? FileName { get; set; }
        public string? Path { get; set; }
        public string? Storage { get; set; }
    }
}

//bu attributun sayesinde File tablosuna UpdatedDate eklenmeyecektir.
//Table Per Hierarchy Yaklaşımı  TPH
