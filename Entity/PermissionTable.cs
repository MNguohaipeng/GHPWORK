using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class PermissionTable:B_EntityBase
    {

        /// <summary>
        /// 权限名称
        /// </summary>
        [SugarColumn(IsNullable =true,Length =500)]
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string PermissionCode { get; set; }


        /// <summary>
        /// 权限类型   0页面权限  1操作权限
        /// </summary>
        [SugarColumn(IsNullable = true )]
        public int PermissionType { get; set; }


        /// <summary>
        /// 权限组ID
        /// </summary>
        public int GroupID { get; set; }
    }
}
