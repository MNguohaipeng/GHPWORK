using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Core
{
/*
 *2017年8月18日16:55:38  郭海鹏
 * 生成页面类
 */
    public class GeneratePage
    {

        public static string ProjectFileUrl = ConfigurationManager.ConnectionStrings["ProjectFileUrl"].ToString();


        //生成  视图和控制器
        public static void Gpage()
        {
            using (var db = LinkDBHelper.CreateDB())

                try
                {
                    var data = db.Queryable<Entity.PageUrl>().Where(T => T.IsDeleted == false).ToList();//获取所有生成的页面信息

                    for (int i = 0; i < data.Count; i++)
                    {

                        if (Directory.Exists(ProjectFileUrl + data[i].HtmlUrl) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(ProjectFileUrl + data[i].HtmlUrl);
                        }
                        if (Directory.Exists(ProjectFileUrl + data[i].ControllerUrl) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(ProjectFileUrl + data[i].ControllerUrl);
                        }


                        string EntityName = data[i].EntityName;

                        var xxx = db.Queryable<Entity.TableString>().Where(T => T.IsDeleted == false && T.TableName == EntityName).ToSql();

                        List<Entity.TableString> TableString = db.Queryable<Entity.TableString>().Where(T => T.IsDeleted == false && T.TableName == EntityName).ToList();

                        //创建list
                        ListHtml(TableString, data[i].EntityName, ProjectFileUrl + data[i].HtmlUrl);
                        //创建Add
                        AddHtml(TableString, data[i].EntityName, ProjectFileUrl + data[i].HtmlUrl, data[i].PageName);
                        //创建Controller
                        Controller(TableString, data[i].EntityName, ProjectFileUrl + data[i].ControllerUrl);


                    }

                }
                catch (Exception ex)
                {
                    throw;
                }

        }



        //加载list
        private static void ListHtml(List<Entity.TableString> TableString, string entityName, string HtmlUrl)
        {
            using (var db = LinkDBHelper.CreateDB())
                try
                {
                    #region 创建Listhtml
                    using (StreamWriter listhtml = new StreamWriter(HtmlUrl + @"\List.cshtml", false, Encoding.UTF8))//创建文件
                    {
                        string cshtml = File.ReadAllText(ProjectFileUrl + @"\Models\cshtmlList模板.txt");

                        string scriptCode = File.ReadAllText(ProjectFileUrl + @"\Models\ListScript模板.txt");
                        string thead = "";

                        thead += "{";
                        thead += "name:'',";
                        thead += "value:'Id',";
                        thead += "IsChack:true,";
                        thead += "},";

                        foreach (var item in TableString)
                        {
                            thead += "{";
                            thead += "name:'" + item.FieldShowMing + "',";
                            thead += "value:'" + item.FieldName + "',";
                            thead += "IsOrder:false" + ",";
                            thead += "},";
                        }
                        thead = thead.TrimEnd(',');


                        scriptCode = string.Format(scriptCode, thead, entityName);
                        scriptCode = scriptCode.Replace("^|", "{");
                        scriptCode = scriptCode.Replace("|^", "}");
                        string ListhtmlString = string.Format(cshtml, entityName, scriptCode);
                        ListhtmlString = ListhtmlString.Replace("【", "{");
                        ListhtmlString = ListhtmlString.Replace("】", "}");
                        listhtml.Write(ListhtmlString);
                    }
                    #endregion
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

        private static void AddHtml(List<Entity.TableString> TableString, string entityName, string HtmlUrl, string pageName)
        {
            using (var db = LinkDBHelper.CreateDB())

            using (StreamWriter sw = new StreamWriter(HtmlUrl + "/Add.cshtml", false, Encoding.UTF8))//创建文件
            {

                if (TableString != null)
                {

                    string input = "";

                    string Script = "";

                    string Select = "";

                    string MrSelect = "";

                    string AddDefaultSelect = "";//默认值下拉编辑框

                    string AddDefaultSelectScript = "";//默认值下拉编辑框

                    string DefaultSelectData = "";//默认下拉数据处理

                    foreach (var item in TableString)
                    {
                        var one = db.Queryable<Entity.EditPage>().Where(T => T.IsDeleted == false && T.YSName == item.InputType).First();
                        if (one == null)
                        {
                            //throw new Exception("没有对应的数据类型");
                        }
                        else
                        {

                            switch (one.YSType)
                            {
                                case "6"://Select
                                    if (item.IsOtherTable == false)
                                        throw new Exception("生成下拉框必须要关联到表的数据");

                                    input += string.Format(one.YSValue, item.FieldName, item.OtherTableFieldBC, item.OtherTableFieldZS, (item.FieldName + "Data"), item.FieldShowMing, item.FieldName);

                                    input = input.Replace("【", "{");
                                    input = input.Replace("】", "}");
                                    string Ls = File.ReadAllText(ProjectFileUrl + @"\Models\angularGetSelectData方法模板.txt");
                                    Select += string.Format(Ls, "GetSelect" + item.FieldName, item.OtherTableName, item.OtherTableWhere, (item.OtherTableFieldBC + "," + item.OtherTableFieldZS), (item.FieldName + "Data"));

                                    MrSelect += "       $scope." + item.FieldName + " = $scope.Update." + item.FieldName + ";\n";


                                    break;
                                case "7"://AddDefaultSelect
                                         //是否有默认值


                                    if (item.IsDefaultData) {

                                        //获取对应的字段HTML
                                        var ModeHtml = db.Queryable<Entity.EditPage>().Where(T => T.IsDeleted == false && T.YSName == item.InputType);
                                        if (ModeHtml.Count()<=0)
                                            throw new Exception(@"没有查询到对用的输入类型：位置GeneratePage\AddHtml函数");

                                        AddDefaultSelect = string.Format(ModeHtml.First().YSValue,item.FieldName,item.FieldShowMing);

                                        input += AddDefaultSelect;

                                        string LsYsname=ModeHtml.First().YSName + "Script";

                                        //获取对应的字段Script
                                        var ModeScript = db.Queryable<Entity.EditPage>().Where(T => T.IsDeleted == false && T.YSName == LsYsname);

                                        if (ModeScript.Count() <= 0)
                                            throw new Exception(@"没有查询到对用的输入类型Script：位置GeneratePage\AddHtml函数");

                                        string xxxxxx = ModeScript.First().YSValue;

                                        AddDefaultSelectScript =ModeScript.First().YSValue;
                                    }

                                    break;

                                case "9":
                                    //获取对应的字段HTML
                                    var DefaultSelectHtml = db.Queryable<Entity.EditPage>().Where(T => T.IsDeleted == false && T.YSName == item.InputType);
                                    if (DefaultSelectHtml.Count() <= 0)
                                        throw new Exception(@"没有查询DefaultSelectHtml：位置GeneratePage\AddHtml函数");

                                    AddDefaultSelect = string.Format(DefaultSelectHtml.First().YSValue,item.FieldName,item.FieldShowMing,"");

                                    input += AddDefaultSelect;

                                    DefaultSelectData = "$scope.Default___Json" + item.Id + "=JSON.parse($scope.Update."+ item.FieldName + ")";

                                    break;

                                default:
                                    input += string.Format(one.YSValue,item.FieldShowMing, item.FieldName);
                                    break;

                            }
                        }
                    }

                    #region 创建script

                    string xxxxx = TableString[0].TableName;

                    Script = "$('#" + TableString[0].TableName + "EditForm').validate(【";

                    Script += "rules: 【";

                    //Script
                    foreach (Entity.TableString item in TableString)
                    {

                        //判断是否有表单验证 
                        if (!string.IsNullOrEmpty(item.AddWhere))
                        {

                            Script += item.FieldName + ":【 \n";

                            string[] AddWhere = item.AddWhere.Split(',');

                            for (int s = 0; s < AddWhere.Length; s++)
                            {

                                Script += AddWhere[s] + ",\n";

                            }

                            Script = Script.TrimEnd(',');

                            Script += "】,\n";

                        }

                    }

                    Script = Script.TrimEnd(',');
 
                    Script += "】   】);";
 


                    #endregion

                    //读取模板

                    string cshtml = File.ReadAllText(ProjectFileUrl + @"\Models\cshtml模版.txt");

                    string PageName = pageName;

                    Script += File.ReadAllText(ProjectFileUrl + @"\Models\angular模板[特殊表单元素].txt");

                    

                    Script = string.Format(Script, Select+"  \n "+ AddDefaultSelectScript, MrSelect+"\n"+ DefaultSelectData);

                    string AddHtml = string.Format(cshtml, PageName, entityName + "EditForm", input, "", Script);

                    Script += AddDefaultSelectScript;

                    AddHtml = AddHtml.Replace("【", "{");

                    AddHtml = AddHtml.Replace("】", "}");

                    sw.Write(AddHtml);

                }
            }
        }

        private static void Controller(List<Entity.TableString> TableString, string entityName, string ControllerUrl)
        {
            using (StreamWriter zs = new StreamWriter(ControllerUrl + "/" + entityName + "Controller.cs", false, Encoding.UTF8))//创建文件
            {

                //读取模板
                //string controllertxt = string.Format(File.ReadAllText(ProjectFileUrl + @"\Models\Controller模板.txt", Encoding.UTF8), entityName);
                using (StreamReader sr = new StreamReader(ProjectFileUrl + @"\Models\Controller模板.txt"))
                {
                    string msg = sr.ReadToEnd();
                    msg = msg.Replace("{0}", entityName);
                    zs.Write(msg);
                }

            }
        }
    }
}
