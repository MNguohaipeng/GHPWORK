using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class TokenHelper
    {

        /// <summary>
        /// 生成GUID字符串
        /// </summary>
        /// <returns></returns>
        public static string GenerateGUID()
        {

            string GUID = Guid.NewGuid().ToString();

            return GUID;

        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <returns></returns>
        public static string GenerateToken(string UserCode, string LoginName)
        {
            try
            {
                string Time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string GUID = GenerateGUID();

                Dictionary<string, string> Dic = new Dictionary<string, string>();

                Dic.Add("Token", GUID);
                Dic.Add("UserCode", UserCode);

                //清除原缓存
                CacheHelper.RemoveAllCache(LoginName);

                Dictionary<string, string> Ulogin = new Dictionary<string, string>();
                Ulogin.Add("LoginName", LoginName);

                //存入缓存   时限一个小时
                CacheHelper.SetCache("LoginName" + GUID, Ulogin, 3600);

                CacheHelper.SetCache(LoginName,Dic,3600);
                return GUID;

            }
            catch (Exception)
            {

                throw;
            }

        }


        public static Dictionary<string, string> GetToKenGlData(string ToKen)
        {

            try
            {

                var xxxx = CacheHelper.GetCache("LoginName" + ToKen);

                //获取缓存数据
                var CacheLoginNameData = CacheHelper.GetCache("LoginName" + ToKen) ?? throw new RuntimeAbnormal("ToKen已失效"); ;
 
                Dictionary<string, string> LoginData = new Dictionary<string, string>();

                LoginData = (Dictionary<string, string>)CacheLoginNameData;

                string LoginName = "";

                foreach (var item in LoginData)
                {

                    if (item.Key == "LoginName")
                        LoginName = item.Value;

                }

                var CacheData = CacheHelper.GetCache(LoginName) ?? throw new RuntimeAbnormal("ToKen已失效");
 
                Dictionary<string, string> HcData = (Dictionary<string, string>)CacheData;

                return HcData;
            }
            catch (RuntimeAbnormal ex)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
