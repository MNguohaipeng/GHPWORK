using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
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
        /// 写入内存
        /// </summary>
        /// <param name="Value">数据格式：{"Key":"Vlaue"}</param>
        /// <returns></returns>
        public static bool WriteInMemory(Dictionary<string, string> Value)
        {


            //构造MemoryStream实例，并输出初始分配容量及使用大小  
            MemoryStream mem = new MemoryStream();

            //数据格式：{"Key":"Vlaue"}
            //  Common.ToDataTable();、

            //将待写入的数据从字符串转换为字节数组  
            UnicodeEncoding encoder = new UnicodeEncoding();

            foreach (var item in Value)
            {

                byte[] bytes = encoder.GetBytes("{\"" + item.Key + "\":\"" + item.Value + "\"}");//创建字节

                mem.Write(bytes, 0, bytes.Length);//写入数据

            }

            return true;


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
                Dic.Add(UserCode, GUID);
                Dic.Add(UserCode + "Time", Time);
                if (!WriteInMemory(Dic))
                    throw new Exception("Token存入内存出错");

                return GUID;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
