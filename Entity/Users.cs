using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Users:B_EntityBase
    { 
        /// <summary>
        /// 登录名
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 21)]
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Password { get; set; }

        /// <summary>
        /// 真是姓名
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 15)]
        public string RealName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 11)]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 80)]
        public string Email { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 5)]
        public string Gender { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 20)]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 20)]
        public string City { get; set; }

        /// <summary>
        /// 县区
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 20)]
        public string County { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string DetailedAddress { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 20)]
        public string NickName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
       [SugarColumn(IsNullable = true)]
        public double UnitPrice { get; set; }

        /// <summary>
        /// 在线状态 【0下线 1在线 2待定】
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int OnlineStatus { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 1000)]
        public string HeadPortrait { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 64)]
        public string UserCode { get; set; }

        /// <summary>
        /// 人气数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int PopularityCount { get; set; }


        /// <summary>
        /// 生日
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 上线地点
        /// </summary>
        [SugarColumn(IsNullable = true,Length =1000)]
        public string OnlineLocation { get; set; }

    }
}
