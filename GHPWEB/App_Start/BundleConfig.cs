using System.Web;
using System.Web.Optimization;

namespace GHPWEB
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region JS
            bundles.Add(new ScriptBundle("~/bundles/base").Include(
          "~/Template/js/jquery.min.js",
          "~/Template/js/bootstrap.js",
          "~/Template/js/app.js",
          "~/Template/js/slimscroll/jquery.slimscroll.min.js",
          //"~/Template/js/app.plugin.js",
          "~/Template/js/jPlayer/jquery.jplayer.min.js",
          "~/Template/js/jPlayer/add-on/jplayer.playlist.min.js",
          "~/Template/js/jPlayer/demo.js"));
            #endregion


            #region CSS
 

           bundles.Add(new StyleBundle("~/Template/css").Include(
          "~/Template/js/jPlayer/jplayer.flat.css",
          "~/Template/css/bootstrap.css",
          "~/Template/css/animate.css",
          "~/Template/css/font-awesome.min.css",
          "~/Template/css/simple-line-icons.css",
          "~/Template/css/font.css",
          "~/Template/css/app.css",
          "~/Template/js/datepicker/datepicker.css",
          "~/Template/js/slider/slider.css",
          "~/Template/js/chosen/chosen.css"
          ));
            #endregion

        }
    }
}
