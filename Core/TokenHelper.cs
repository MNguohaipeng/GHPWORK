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
        public static string GenerateToken(string UserCode)
        {
            try
            {
                string Time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string GUID = GenerateGUID();

                Dictionary<string, string> Dic = new Dictionary<string, string>();

                Dic.Add("Token", GUID);
                Dic.Add("UserCode",UserCode);
                //存入缓存   时限一个小时
                CacheHelper.SetCache("UsersToKen" + GUID, Dic,3600);

                return GUID;

            }
            catch (Exception)
            {

                throw;
            }

        }
 
    }
}
