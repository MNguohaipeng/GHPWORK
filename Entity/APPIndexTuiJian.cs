using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Entity
{
  public class APPIndexTuiJian:B_EntityBase
    {

        [SugarColumn(IsNullable =false,Length =70)]
        public string UserCode { get; set; }

        [SugarColumn(IsNullable = false)]
        public int Index { get; set; }



    }
}
