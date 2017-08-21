﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{


    /*创建时间：
     *     2017年7月11日09:21:12
     * 创建人 : 
     *     郭海鹏
     * 说明 :
     *     生成页面 和 控制器的核心代码类
     */

    public class TableString : B_EntityBase
    {
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(Length = 50)]
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [SugarColumn(Length = 50)]
        public string FieldName { get; set; }

        [SugarColumn(Length = 50)]
        public string FieldShowMing { get; set; }

        [SugarColumn(Length = 50)]
        public string InputType { get; set; }

        [SugarColumn(IsNullable = true, Length = 500)]
        public string AddWhere { get; set; }  //添加时的筛选条件 用逗号分割  不能为空等等~~~

        [SugarColumn(IsNullable = true, Length = 500)]
        public string IsOtherTable { get; set; } //是否关联其他表数据

        [SugarColumn(IsNullable= true, Length = 500)]
        public string OtherTableName { get; set; }//其他表名称  

        [SugarColumn(IsNullable = true, Length = 500)]
        public string OtherTableFieldZS { get; set; }//关联其他表的字段展示用  逗号分割 

        [SugarColumn(IsNullable = true, Length = 50)]
        public string OtherTableFieldBC { get; set; }//关联其他表的字段保存用

        [SugarColumn(IsNullable = true, Length = 500)]
        public string OtherTableWhere { get; set; }//关联其他表的查询条件

        /// <summary>
        /// 是否有默认值
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool IsDefaultData { get; set; }
  
        /// <summary>
        /// 与其他字段联动
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool IsOtherFieldLinkage { get; set; }

        /// <summary>
        /// 其他字段名称
        /// </summary>
        [SugarColumn(IsNullable = true,Length =100)]
        public string OtherFieldName { get; set; }


        /// <summary>
        /// 联动方式
        /// </summary>
        [SugarColumn(IsNullable = true, Length=100)]
        public string LinkageMode { get; set; }


        /// <summary>
        /// 配置默认值
        /// </summary>
        [SugarColumn(IsNullable = true,Length =5000)]
        public string PageDefaultData { get; set; }



    }
}
