using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    /// <summary>
    /// 页面元素表
    /// </summary>
   public class EditPage:B_EntityBase
    {
        /// <summary>
        /// 元素名称  【检索用】
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 80)]
        public string YSName { get; set; }
 
        /// <summary>
        /// 元素名称  展示用
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 300)]
        public string YSNameZs { get; set; }

        /// <summary>
        /// 元素值
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 1500)]
        public string YSValue { get; set; }

        /// <summary>
        /// 元素类型编号
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 30)]
        public string YSType { get; set; }


    }
}
