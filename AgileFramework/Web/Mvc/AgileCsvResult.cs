using AgileFramework.Office;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace AgileFramework.Web.Mvc
{
    public class AgileCsvResult : ContentResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rows">数据</param>
        /// <param name="fileName">文件名</param>
        public AgileCsvResult(List<dynamic> rows, ControllerBase controller, string fileName = "Export")
            : base()
        {
            controller.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName + ".csv");

            Content = AgileCsv.ToCsv(rows);

            ContentType = "text/csv";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rows">数据</param>
        /// <param name="fileName">文件名</param>
        public AgileCsvResult(DataTable rows, ControllerBase controller, string fileName = "Export")
            : base()
        {
            controller.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName + ".csv");

            this.Content = AgileCsv.ToCsv(rows);

            this.ContentType = "text/csv";
        }
    }
}
