using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GHPWEB.Areas.API.Controllers
{
    public class UsersController : ApiController
    {

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SignIn(dynamic obj)
        {
            try
            {

                if (obj == null)
                {
                    return Json(new { status = 3, message = "参数不能为空" });
                }

                string ResourceType = obj.ResourceType + "";

                string LoginName = obj.LoginName + "";

                string Password = obj.Password + "";

                switch (ResourceType)
                {
                    case "0":

                        if (string.IsNullOrEmpty(LoginName))
                            throw new Exception("手机号码不能为空");

                        if (!LoginName.IsValidMobile())
                            throw new Exception("手机号码格式错误");

                        break;
                    case "1":

                        if (string.IsNullOrEmpty(LoginName))
                            throw new Exception("Email不能为空");

                        if (!LoginName.IsValidEmail())
                            throw new Exception("Email格式错误");

                        break;
                    default:

                        if (string.IsNullOrEmpty(LoginName))
                            throw new Exception("登录名不能为空");

                        break;
                }

                using (var db = Common.LinkDBHelper.CreateDB())
                {

                    string password = Password.ToMD5();
                    var user = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == LoginName && T.Password == password).First();

                    if (user != null)
                    {
                        string Token = TokenHelper.GenerateToken(user.UserCode);
                        return Json(new { status = 1, data = user, Token = Token });
                    }
                    else
                    {
                        return Json(new { status = 0, message = "用户名或密码错误" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SignUp(dynamic obj)
        {
            if (obj == null)
            {
                return Json(new { status = 3, message = "参数不能为空" });
            }


            string PhoneNumber = Convert.ToString(obj.PhoneNumber);
            string Password = Convert.ToString(obj.Password);
            using (var db = Common.LinkDBHelper.CreateDB())
            {

                string password = Password.ToMD5();

                Entity.Users users = new Entity.Users();
                users.CreateTime = DateTime.Now;
                users.LoginName = PhoneNumber;
                users.Phone = PhoneNumber;
                users.Password = password;
                users.IsDeleted = false;
                users.RealName = "待补充";
                users.Email = "GD@GD.com";
                var usercount = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == PhoneNumber || T.Phone == PhoneNumber).Count();



                if (usercount > 0)
                {
                    return Json(new { status = 0, message = "此手机号以绑定账号，请更换手机或找回密码" });
                }
                else
                {
                    int insertrows = db.Insertable<Entity.Users>(users).ExecuteCommand();
                    if (insertrows > 0)
                    {
                        return Json(new { status = 1, message = "注册成功" });
                    }
                    else
                    {
                        return Json(new { status = 2, message = "系统出错:受影响的行数小于等于0" });
                    }
                }

            }

        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateUserData(dynamic obj)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {

                    string UserCode = obj.UserCode;

                    string password = obj.Password;
                    password = password.ToMD5();

                    string Birthday = obj.Birthday;



                    var users = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.UserCode == UserCode && T.Password == password).ToList();

                    if (users.Count <= 0)
                        throw new Exception("没有查询到对应数据，无法进行用户信息的修改");

                    Entity.Users user = users[0];

                    user.Gender = obj.Gender;

                    if (!string.IsNullOrEmpty(Birthday))
                    {

                        DateTime BirthdayTime;

                        if (!DateTime.TryParse(Birthday, out BirthdayTime))
                        {

                            throw new Exception("生日格式错误");

                        }
                        else
                        {

                            user.Birthday = BirthdayTime;

                        }

                    }

                    user.NickName = obj.NickName;

                    user.HeadPortrait = obj.HeadPortrait;

                    var fanhui = db.Updateable<Entity.Users>(user).ExecuteCommand();

                    return Json(new { status = 1, message = "修改成功" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = 0, message = ex.Message });
                }

        }

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
                    string AgeText = obj.Age;

                    string PopularityCountText = obj.PopularityCount;

                    string OnlineLocation = obj.OnlineLocation;

                    string Gender = obj.Gender;

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

                    string Sql = "select LoginName,RealName,Phone,Email,Gender,Province,City,County,DetailedAddress,NickName,HeadPortrait,PopularityCount,Birthday,OnlineStatus,Gender from Users where 1=1 ";

                    if (!string.IsNullOrEmpty(Gender))
                    {
                        Sql += " AND Gender='" + Gender+"'";
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
