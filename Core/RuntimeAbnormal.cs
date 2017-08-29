using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*自定义异常捕捉类
* 创建人:郭海鹏
* 创建时间:2017年7月15日09:56:07
*/
  
namespace Core
{

    public class RuntimeAbnormal : Exception
    {

        public RuntimeAbnormal(string _ErrorMessage)
        {
            ErrorMessage = _ErrorMessage;
            OccurrenceTime = DateTime.Now;
            StLog();
        }
        public RuntimeAbnormal(string _ErrorMessage,Exception ErrorInfo)
        {
            ErrorMessage = _ErrorMessage;
            OccurrenceTime = DateTime.Now;
            StLog();
        }
        public string ErrorMessage { get; set; }//错误信息

        public DateTime OccurrenceTime { get; set; }//发生时间

        /// <summary>
        /// 记录日志
        /// </summary>
        public void StLog() {

            string FileUrl = ConfigurationManager.ConnectionStrings["LogText"].ToString();

            using (StreamWriter sw = new StreamWriter(FileUrl, true, Encoding.UTF8))//创建文件
            {
                string LogText = "-----------------------------" + DateTime.Now + "-----------------------------\n";
                LogText += "错误信息：" + ErrorMessage + "\n";

                LogText += "----------------------------END---------------------------------";
                sw.Write(LogText);
            }
        }

    }
}
