using Common;
using Core;
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

                using (var db = LinkDBHelper.CreateDB())
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
            

            using (var db = LinkDBHelper.CreateDB())
                try
                {
                    if (obj == null)
                    {
                        return Json(new { status = 3, message = "参数不能为空" });
                    }

                    string LoginName = "";

                    string ResourceType = "";

                    if (ResourceType != null)
                        ResourceType = obj.ResourceType;

                    if (obj.LoginName == null)
                        throw new RuntimeAbnormal("用户登录名不能为空");

                    if (obj.Password == null)
                        throw new RuntimeAbnormal("密码不能为空");

                    LoginName = obj.LoginName + "";

                    string Password = obj.Password + "";


                    string password = Password.ToMD5();

                    Entity.Users users = new Entity.Users();

                    users.CreateTime = DateTime.Now;

                    users.LoginName = LoginName;

                    users.Password = password;

                    users.IsDeleted = false;

                    users.RealName = "待补充";

                    users.UserCode = Common.Common.OutUserCode();

                    int usercount = 0;

                    switch (ResourceType)
                    {
                        case "0":

                            if (!LoginName.IsValidPhone())
                                throw new RuntimeAbnormal("手机号格式不正确");

                            users.Phone = LoginName;

                            usercount = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == LoginName || T.Phone == LoginName).Count();

                            if (usercount > 0)
                                throw new RuntimeAbnormal("此手机号已存在，请更换手机或找回密码。");


                            break;
                        case "1":

                            if (!LoginName.IsValidEmail())
                                throw new RuntimeAbnormal("Email格式不正确");

                            users.Email = LoginName;

                            usercount = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == LoginName || T.Email == LoginName).Count();

                            if (usercount > 0)
                                throw new RuntimeAbnormal("此邮箱已存在，请更换邮箱或找回密码");

                            break;
                        default:

                            usercount = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == LoginName || T.LoginName == LoginName).Count();

                            if (usercount > 0)
                                throw new RuntimeAbnormal("此登陆名已存在，请更换或找回密码");

                            break;
                    }

                    int insertrows = db.Insertable<Entity.Users>(users).ExecuteCommand();
                    if (insertrows > 0)
                    {
                        string Token = TokenHelper.GenerateToken(users.UserCode);
                        users.Password = "";
                        return Json(new { status = 1, message = "注册成功",data= users, Token= Token });
                    }
                    else
                    {
                        return Json(new { status = 2, message = "系统出错:受影响的行数小于等于0" });
                    }


                }
                catch (RuntimeAbnormal ex) {

                    return Json(new { status = 0, message = ex.ErrorMessage });
                }
                catch (Exception)
                {
                    throw;

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

                    if (obj != null)
                    {
                        if (obj.UserCode == null)
                            throw new Exception("用户编号不能为空");

                        if (obj.Password == null)
                            throw new Exception("密码不能为空");


                    }
                    string UserCode = obj.UserCode;

                    string password = obj.Password;

                    password = password.ToMD5();





                    var users = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.UserCode == UserCode && T.Password == password).ToList();

                    if (users.Count <= 0)
                        throw new Exception("没有查询到对应数据，无法进行用户信息的修改");

                    Entity.Users user = users[0];

                    if (obj.Gender != null)
                        user.Gender = obj.Gender;

                    if (obj.Birthday != null)
                    {

                        DateTime BirthdayTime;

                        if (!DateTime.TryParse(obj.Birthday, out BirthdayTime))
                        {

                            throw new Exception("生日格式错误");

                        }
                        else
                        {

                            user.Birthday = BirthdayTime;

                        }

                    }

                    if (obj.NickName != null)
                        user.NickName = obj.NickName;

                    if (obj.HeadPortrait != null)
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
        /// 获取用户信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetUserData(dynamic obj)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {
                    if (obj.UserCode == null)
                        throw new Exception("用户编号不能为空");

                    if (obj.Password == null)
                        throw new Exception("密码不能为空");

                    string UserCode = obj.UserCode;

                    string Password = obj.Password;

                    Password = Password.ToMD5();
                    var user = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.Password == Password && T.UserCode == UserCode).ToList();

                    if (user.Count <= 0)
                        throw new Exception("用户名或密码错误");
                    user[0].Password = "";
                    return Json(new { status = 1, data = user[0] });
                }
                catch (Exception ex)
                {
                    return Json(new { status = 0, message = ex.Message });
                }



        }
    }
}
