using Common;
using Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
                        string Token = TokenHelper.GenerateToken(user.UserCode, LoginName);
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

                    users.Birthday = DateTime.Now;

                    users.Age = 1;

                    users.Gender = "男";

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
                        string Token = TokenHelper.GenerateToken(users.UserCode, LoginName);
                        users.Password = "";
                        return Json(new { status = 1, message = "注册成功", data = users, Token = Token });
                    }
                    else
                    {
                        return Json(new { status = 2, message = "系统出错:受影响的行数小于等于0" });
                    }


                }
                catch (RuntimeAbnormal ex)
                {

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
        public IHttpActionResult UpdateUserData()
        {

 
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context

            HttpRequestBase request = context.Request;//定义传统request对象


            using (var db = LinkDBHelper.CreateDB())
                try
                {
                    if (request == null)
                        throw new RuntimeAbnormal("参数不能为空");
 

                    if (request.Form["ToKen"] == null)
                        throw new RuntimeAbnormal("ToKen不能为空");

                    string ToKen = request.Form["ToKen"];

                    //获取缓存数据
                    var CacheData = TokenHelper.GetToKenGlData(ToKen);

                //    return Json(new { status = 1, message = "测试信息", data = CacheData });
                    if (CacheData == null)
                        throw new RuntimeAbnormal("ToKen已失效");

                    string UserCode = "";

                    string CacheToKen = "";

                    foreach (var item in CacheData)
                    {
                        if (item.Key == "UserCode")
                            UserCode = item.Value;

                        if (item.Key == "Token")
                            CacheToKen = item.Value;
                    }

                    if (CacheToKen != ToKen)
                        throw new RuntimeAbnormal("ToKen已失效,传入的Token为:"+ ToKen);

                    var users = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.UserCode == UserCode).ToList();

                    if (users.Count <= 0)
                        throw new RuntimeAbnormal("没有查询到对应数据，无法进行用户信息的修改");

                    Entity.Users user = users[0];
                    
                    if (request.Form["Gender"] != null)
                        user.Gender = request.Form["Gender"];
                   
                    if (request.Form["Birthday"] != null)
                    {

                        DateTime BirthdayTime=DateTime.Now;
                       
                        string BirthdayText = request.Form["Birthday"];

                        if (!DateTime.TryParse(BirthdayText, out BirthdayTime))
                        {

                            throw new RuntimeAbnormal("生日格式错误");

                        }
                        else
                        {

                            var years = ((DateTime.Now - BirthdayTime).Days) / 365;

                            if (years <= 0)
                                years++;

                            user.Age = years;
                            user.Birthday = BirthdayTime;

                        }

                    }
                    
                    if (request.Form["NickName"] != null)
                        user.NickName = request.Form["NickName"];

                    
                    if (request.Files["HeadPortrait"] != null) {
                    
                        var HeadPortraitFile = request.Files["HeadPortrait"];

                        if (HeadPortraitFile == null)
                            throw new RuntimeAbnormal("文件不能为空");

                        string Url = ConfigurationManager.ConnectionStrings["HeadPortrait"].ToString();

                        var TDa = HeadPortraitFile.FileName.Split('.');

                        string FileHz= TDa[TDa.Count()-1];

                        Common.Common.UpLoadFile(HeadPortraitFile, Url, UserCode + "." + FileHz);

                        user.HeadPortrait = Url+""+UserCode + "." + FileHz;
                    }
                      

                    var fanhui = db.Updateable<Entity.Users>(user).ExecuteCommand();
                    user.Password = "";
                    return Json(new { status = 1, message = "修改成功",data= user });
                }
                catch (RuntimeAbnormal ex)
                {
                    return Json(new { status = 0, message = ex.ErrorMessage });
                }
                catch (Exception ex)
                {
                    throw;
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
                    if (obj.ToKen == null)
                        throw new RuntimeAbnormal("ToKen不能为空");

                    string UserCode = "";

                    string ToKen = obj.ToKen;

                    Dictionary<string, string> HcData = TokenHelper.GetToKenGlData(ToKen);

                    foreach (var items in HcData)
                    {

                        if (items.Key == "UserCode")
                            UserCode = items.Value;

                    }

                    var user = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.UserCode == UserCode).ToList();

                    if (user.Count <= 0)
                        throw new RuntimeAbnormal("用户名或密码错误");
                    user[0].Password = "";
                    return Json(new { status = 1, data = user[0] });
                }
                catch (RuntimeAbnormal ex)
                {
                    return Json(new { status = 0, message = ex.ErrorMessage });
                }
                catch (Exception ex)
                {
                    throw;
                }

        }


        [HttpPost]
        public IHttpActionResult UpdatePassword(dynamic obj)
        {
            try
            {


                if (obj == null)
                    throw new RuntimeAbnormal("参数不能为空");

                if (obj.ToKen == null)
                    throw new RuntimeAbnormal("ToKen不能为空");

                if (obj.Password == null)
                    throw new RuntimeAbnormal("新密码不能为空");

                string ToKen = obj.ToKen;

                //获取缓存数据
                var CacheData = TokenHelper.GetToKenGlData(ToKen);

                if (CacheData == null)
                    throw new RuntimeAbnormal("ToKen已失效");

                string UserCode = "";

                string Password = obj.Password;

                foreach (var item in CacheData)
                {
                    if (item.Key == "UserCode")
                        UserCode = item.Value;
                }

                UpdatePassword(UserCode, Password);
                return Json(new { status = 1, message = "修改成功" });
            }
            catch (RuntimeAbnormal ex)
            {
                return Json(new { status = 0, message = ex.ErrorMessage });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="Password">新密码</param>
        [NonAction]
        public bool UpdatePassword(string UserCode, string Password)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {


                    var Users = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.UserCode == UserCode).ToList();

                    if (Users.Count <= 0)
                        throw new RuntimeAbnormal("没有查询到对应用户");
                    var User = Users[0];

                    User.Password = Password.ToMD5();

                    int count = db.Updateable<Entity.Users>(User).ExecuteCommand();

                    if (count <= 0)
                        throw new RuntimeAbnormal("系统出错，受影响的行数小于等于0");

                    return true;

                }
                catch (RuntimeAbnormal ex)
                {
                    throw;
                }
                catch (Exception)
                {

                    throw;
                }

        }


        /// <summary>
        /// 验证账号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult VerifyAccount(dynamic obj)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {
                 
                    string LoginName = "";

                    string ResourceType = "";

                    if (obj == null)
                        throw new RuntimeAbnormal("参数不能为空");

                    if (obj.LoginName == null)
                        throw new RuntimeAbnormal("登录名不能为空");

                    if (obj.ResourceType != null)
                        ResourceType = obj.ResourceType;

                    LoginName = obj.LoginName;

                    int Count = 0;

                    switch (ResourceType)
                    {
                        case "0":
                            if (!LoginName.IsValidPhone())
                                throw new RuntimeAbnormal("手机号格式不正确");

                            Count = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.Phone == LoginName).Count();

                            break;
                        case "1":
                            if (!LoginName.IsValidEmail())
                                throw new RuntimeAbnormal("邮箱格式不正确");

                            Count = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.Email == LoginName).Count();
                            break;
                        default:
                            Count = db.Queryable<Entity.Users>().Where(T => T.IsDeleted == false && T.LoginName == LoginName).Count();
                            break;
                    }

                    if (Count > 0)
                        throw new RuntimeAbnormal("此账号已存在");

                    return Json(new { status = 1, message = "验证成功" });

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
        /// 注销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Cancellation(dynamic obj) {
            using (var db=LinkDBHelper.CreateDB())
                try
                {

                    if (obj == null)
                        throw new RuntimeAbnormal("参数不能为空");

                    if(string.IsNullOrEmpty(obj.LoginName))
                        throw new RuntimeAbnormal("登录名不能为空");

                    string LoginName = obj.LoginName;

                    //清除缓存
                    CacheHelper.RemoveAllCache(LoginName);


                    return Json(new { status = 1, message = "验证成功" });
                }
                catch (RuntimeAbnormal ex) {
                    return Json(new { status = 0, message = ex.ErrorMessage });
                }
                catch (Exception)
                {

                    throw;
                }
        }

        [HttpPost]
        public IHttpActionResult Test() {
            try
            {

                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context

                HttpRequestBase request = context.Request;//定义传统request对象


                if (request == null)
                    throw new RuntimeAbnormal("参数不能为空");


                if (request.Form["ToKen"] == null)
                    throw new RuntimeAbnormal("ToKen不能为空");

                var File = request.Files["File"];

                if (File == null)
                    throw new RuntimeAbnormal("文件不能为空");



                string ToKen = request.Form["ToKen"];

                //获取缓存数据
                var CacheData = TokenHelper.GetToKenGlData(ToKen);

                //    return Json(new { status = 1, message = "测试信息", data = CacheData });
                if (CacheData == null)
                    throw new RuntimeAbnormal("ToKen已失效,传入的Token为:" + ToKen);

                string UserCode = "";

                string CacheToKen = "";

                foreach (var item in CacheData)
                {
                    if (item.Key == "UserCode")
                        UserCode = item.Value;

                    if (item.Key == "Token")
                        CacheToKen = item.Value;
                }

                if (CacheToKen != ToKen)
                    throw new RuntimeAbnormal("ToKen已失效,传入的Token为:" + ToKen);

                var TDa = File.FileName.Split('.');

                string FileHz = TDa[TDa.Count() - 1];

                string Url = ConfigurationManager.ConnectionStrings["HeadPortrait"].ToString();

                Common.Common.UpLoadFile(File, Url, UserCode + "." + FileHz);



                return Json(new { Url = Url + "" + UserCode + "." + FileHz, ToKen = ToKen });



            }
            catch (RuntimeAbnormal ex) {
                return Json(new { message=ex.ErrorMessage});
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult UploadImage(dynamic obj)
        {

            try
            {

                if (obj == null)
                    throw new RuntimeAbnormal("参数不能为空");

                if (obj.ToKen + ""==""|| obj.ToKen + ""==null)
                    throw new RuntimeAbnormal("ToKen不能为空");

                if (obj.Type + ""==""|| obj.Type + ""==null)
                    throw new RuntimeAbnormal("图片用途类型不能为空");

                if (obj.File0 + ""==""|| obj.File0 + ""==null)
                    throw new RuntimeAbnormal("图片文件不能为空");

                if (obj.Suffix + ""==""|| obj.Suffix + ""==null)
                    throw new RuntimeAbnormal("图片后缀不能为空");

                string Type = obj.Type;//0 头像 1 用户介绍

                string[] FileArrey = new string[5];

                string Suffix = obj.Suffix;

                string ToKen = obj.ToKen;

                string fileDir=ConfigurationManager.ConnectionStrings["HeadPortrait"].ToString();

                Dictionary<string,string> Cachen=TokenHelper.GetToKenGlData(ToKen);

                string UserCode = "";

                foreach (var item in Cachen)
                {
                    if (item.Key == "UserCode")
                        UserCode = item.Value;
                }

                

                switch (Type)
                {
                    case "0":

                        FileArrey[0] = obj.File0;

                        //转换并保存图片
                        Base64Helper.Base64StringToImage(FileArrey[0], Suffix, UserCode+ "HeadPortrait."+Suffix,fileDir);

                        break;
                    case "1":
                        break;
                    default:
                        throw new RuntimeAbnormal("没有对应的处理程序");

                }
              return Json(new { status =1, message = "上传成功",url= fileDir+UserCode + "HeadPortrait." + Suffix }); ;
            }
            catch (RuntimeAbnormal ex)
            {

                return Json(new { status = 0, message = ex.ErrorMessage });

            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });

            }

        }




    }
}
