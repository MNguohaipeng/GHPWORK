using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GHPWEB.Areas.API.Controllers
{
    public class APPIndexController : ApiController
    {
        [HttpPost]
        public string TuiJian() {

            try
            {

            }
            catch (Exception)
            {

                throw;
            }


            return "测试";
        }

    }
}
