using Common;
using Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace GHPWEB.Areas.API.Controllers
{
    public class ShortMessageController : ApiController
    {
        /// <summary>
        /// 提交数据请求  方法一
        /// </summary>
        [HttpPost]
        public IHttpActionResult SendOut(dynamic obj)
        {
            try
            {
                string PhoneNumber = "";
 
                if (obj.PhoneNumber == null)
                    throw new RuntimeAbnormal("手机号码不能为空");
 
                PhoneNumber = obj.PhoneNumber;

                if (!PhoneNumber.IsValidPhone())
                    throw new RuntimeAbnormal("手机号码格式不正确");
 
                string POSTURL = ConfigurationManager.ConnectionStrings["ShortMessage"].ToString();

                string ShortMessageCode = "";

                string ShortMessageIndex = "";

                foreach (var item in Code())
                {
                    if (item.Key.Contains("SMCode"))
                        ShortMessageCode = item.Value;

                    if (item.Key.Contains("Index"))
                        ShortMessageIndex = item.Value;
                }

                string PostData = "{\"templateSMS\":{\"appId\":\"67b37b86266b44afa04672cb451621bb\",\"param\":\""+ ShortMessageCode + "\",\"templateId\":\"" + ConfigurationManager.ConnectionStrings["templateId"].ToString() + "\",\"to\":\"" + PhoneNumber + "\"}}";

                string SigParameter = ConfigurationManager.ConnectionStrings["AccountSid"].ToString() + ConfigurationManager.ConnectionStrings["AuthToken"].ToString() + DateTime.Now.ToString("yyyyMMddHHmmss");

                byte[] bytes = Encoding.Default.GetBytes(ConfigurationManager.ConnectionStrings["AccountSid"].ToString() + ":" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                string Authorization = Convert.ToBase64String(bytes);
 
                SigParameter = SigParameter.ToMD5().ToUpper();

                POSTURL = string.Format(POSTURL, SigParameter, Authorization);

                //发送请求的数据
                WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);

                myHttpWebRequest.Method = "POST";

                WebHeaderCollection whc = new WebHeaderCollection();

                whc.Add("Authorization:" + Authorization);

                myHttpWebRequest.Headers = whc;

                UTF8Encoding encoding = new UTF8Encoding();

                byte[] byte1 = encoding.GetBytes(PostData);

                myHttpWebRequest.ContentType = "application/json;charset=utf-8";

                myHttpWebRequest.ContentLength = byte1.Length;
              
                Stream newStream = myHttpWebRequest.GetRequestStream();
 
                newStream.Write(byte1, 0, byte1.Length);

                newStream.Close();
 
                //发送成功后接收返回的XML信息
                HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();

                string lcHtml = string.Empty;

                Encoding enc = Encoding.GetEncoding("UTF-8");

                Stream stream = response.GetResponseStream();

                StreamReader streamReader = new StreamReader(stream, enc);

                lcHtml = streamReader.ReadToEnd();

                return Json(new { status = 1, Index= ShortMessageIndex });

            }
            catch (RuntimeAbnormal ex) {

                return Json(new { status = 0, message = ex.Message });

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Code() {
            try
            {
                Random Ra = new Random(DateTime.Now.ToString("fffffff").ToInt());
               int Code=Ra.Next(100000,999999);
                Dictionary<string, string> dc = new Dictionary<string, string>();

                string Index = Guid.NewGuid().ToString();

                dc.Add("SMCode"+ Index, Code+"");
 
                dc.Add("SMTime"+ Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                dc.Add("Index", Index);

                TokenHelper.WriteInMemory(dc);

                return dc;
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
