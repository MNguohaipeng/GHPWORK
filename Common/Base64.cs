using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Configuration;

namespace Common
{
    public class Base64Helper
    {

        //图片 转为    base64编码的文本
        private static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                FileStream fs = new FileStream(Imagefilename + ".txt", FileMode.Create);


                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);

                return strbaser64;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //base64编码的文本 转为    图片并保存
        public static void Base64StringToImage(string FileText, string Suffix, string FileName,string fileDir)
        {
            try
            {
    

                if (!FileText.Contains("image"))
                    throw new Exception("数据不是有效的图片格式");

                string Title = FileText.Split(',')[0]+",";//开头部分


                FileText = FileText.Replace(Title,"");//去除开头部分

                byte[] arr = Convert.FromBase64String(FileText);

                using (MemoryStream ms2 = new MemoryStream(arr))
                {
                    Bitmap bmp2 = new Bitmap(ms2);

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(fileDir)))
                        Directory.CreateDirectory(fileDir);

                    switch (Suffix)
                    {
                        case "jpeg":
                        case "jpg":
                            bmp2.Save(System.Web.HttpContext.Current.Server.MapPath(fileDir) + FileName, ImageFormat.Jpeg);
                            break;
                        case "bmp":
                            bmp2.Save(System.Web.HttpContext.Current.Server.MapPath(fileDir) + FileName, ImageFormat.Bmp);
                            break;
                        case "gif":
                            bmp2.Save(System.Web.HttpContext.Current.Server.MapPath(fileDir) + FileName, ImageFormat.Gif);
                            break;
                        case "png":
                            bmp2.Save(System.Web.HttpContext.Current.Server.MapPath(fileDir) + FileName, ImageFormat.Png);
                            break;
                        default:
                            throw new Exception("没有" + Suffix + "类型的处理程序");

                    }

                    bmp2.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}