using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;
namespace Common
{
    public static class Common
    {

        public static object ConvertType(string Value, Type Type)
        {



            object ReturnData = null;
            switch (Type.FullName)
            {
                case "System.Int32":
                    ReturnData = SqlFunc.ToInt32(Value);
                    break;
                case "System.String":
                    ReturnData = SqlFunc.ToString(Value);
                    break;
                case "System.Decimal":
                    ReturnData = SqlFunc.ToDecimal(Value);
                    break;
                case "System.DateTime":
                    ReturnData = SqlFunc.ToDecimal(Value);
                    break;
                case "System.Double":
                    ReturnData = SqlFunc.ToDouble(Value);
                    break;
                case "System.Boolean":
                    ReturnData = SqlFunc.ToBool(Value);
                    break;
                default:
                    throw new Exception("不存在" + Type.FullName + "类型的转换函数，请联系开发人员");
            }


            return ReturnData;

        }

        public static string OutScript(string Type, string Message, string Url)
        {

            string Script = "";

            switch (Type)
            {
                case "Jump": //直接跳转
                    Script += "<script>";
                    Script += "location='" + Url + "'";
                    Script += "</script>";
                    break;
                case "AlertJump"://弹出后跳转
                    Script += "<script>";
                    Script += "alert('" + Message + "');";
                    Script += "location='" + Url + "'";
                    Script += "</script>";
                    break;
                case "AlertBack":
                    Script += "<script>";
                    Script += "alert('" + Message + "');";
                    Script += "$('#back').click();";
                    Script += "</script>";
                    break;
                default:
                    break;
            }
            return Script;

        }

        public static int[] OutIntArreyForIds(string Ids)
        {

            string DataStr = Ids.Replace("[", "");

            DataStr = DataStr.Replace("]", "");
            string[] IdArrey = DataStr.Split(',');
            int[] IdsArrey = new int[IdArrey.Length];
            //
            Regex rx = new Regex("[0-9]{1,50}");


            for (int i = 0; i < IdArrey.Length; i++)
            {
                int Bl;
                string im = rx.Matches(IdArrey[i])[0].Value;

                if (int.TryParse(im, out Bl))
                {
                    IdsArrey[i] = Bl;
                }
            }
            return IdsArrey;
        }




        #region DataTable 转换为Json 字符串
        /// <summary>
        /// DataTable 对象 转换为Json 字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dt)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue;
            ArrayList arrayList = new ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToStr());
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }

            return javaScriptSerializer.Serialize(arrayList);  //返回一个json字符串
        }
        #endregion

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }
        #endregion

        #region 转换为string字符串类型
        /// <summary>
        ///  转换为string字符串类型
        /// </summary>
        /// <param name="s">获取需要转换的值</param>
        /// <param name="format">需要格式化的位数</param>
        /// <returns>返回一个新的字符串</returns>
        public static string ToStr(this object s, string format = "")
        {
            string result = "";
            try
            {
                if (format == "")
                {
                    result = s.ToString();
                }
                else
                {
                    result = string.Format("{0:" + format + "}", s);
                }
            }
            catch
            {
            }
            return result;
        }
        #endregion


    }
}
