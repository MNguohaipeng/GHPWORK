using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GHPWEB.Areas.API.Controllers
{
    public class APPIndexController : ApiController
    {



        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetIndexData(dynamic obj)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {
                    string AgeText = "";
                    string PopularityCountText = "";

                    string OnlineLocation = "";

                    string Gender = "";

                    if (obj != null)
                    {

                        if (obj.Age != null)
                            AgeText = obj.Age;

                        if (obj.PopularityCountText != null)
                            PopularityCountText = obj.PopularityCount;

                        if (obj.OnlineLocation != null)
                            OnlineLocation = obj.OnlineLocation;

                        if (obj.Gender != null)
                            Gender = obj.Gender;
                    }



                    int Age;

                    int PopularityCount;

                    if (!string.IsNullOrEmpty(AgeText))
                    {
                        if (!int.TryParse(AgeText, out Age))
                        {
                            throw new Exception("年龄格式错误");
                        }
                    }

                    if (!string.IsNullOrEmpty(PopularityCountText))
                    {
                        if (!int.TryParse(PopularityCountText, out PopularityCount))
                        {
                            throw new Exception("人气格式错误");
                        }
                    }



                    string Sql = "select LoginName,RealName,Phone,Email,Gender,Province,City,County,DetailedAddress,NickName,HeadPortrait,PopularityCount,Birthday,OnlineStatus,Gender from Users where UserCode in (select UserCode from APPIndexTuiJian order by `index`)  ";

                    if (!string.IsNullOrEmpty(Gender))
                    {
                        Sql += " AND Gender='" + Gender + "'";
                    }

                    if (!string.IsNullOrEmpty(AgeText))
                    {
                        Sql += " AND Age <" + AgeText;
                    }

                    if (!string.IsNullOrEmpty(PopularityCountText))
                    {
                        Sql += " AND PopularityCount=" + PopularityCountText;
                    }

                    if (!string.IsNullOrEmpty(OnlineLocation))
                    {
                        Sql += " AND OnlineLocation=" + OnlineLocation;
                    }

                    var list = db.Ado.GetDataTable(Sql);

                    return Json(new { status = 1, data = list, message = "距离没做筛选，待定算法" });

                }
                catch (Exception ex)
                {
                    return Json(new { status = 0, message = ex.Message });
                }
        }



    }
}
