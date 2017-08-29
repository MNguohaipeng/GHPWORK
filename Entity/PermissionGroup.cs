using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    /// <summary>
    /// 权限组
    /// </summary>
    public class PermissionGroup:B_EntityBase
    {

        /// <summary>
        /// 组名称
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 100)]
        public string GroupName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 5000)]
        public string Note { get; set; }

    }
}
