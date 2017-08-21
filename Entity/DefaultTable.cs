using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class DefaultTable : B_EntityBase
    {
        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string Value { get; set; }

        /// <summary>
        /// 对应值
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string CorrespondingValue { get; set; }

        /// <summary>
        /// 对应表名
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string CorrespondingTableName { get; set; }

        /// <summary>
        /// 对应字段名
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string CorrespondingField { get; set; }

    }
}
