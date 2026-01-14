using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.PDFHanlder
{
    public class ExportPDF
    {
        //private string RenderViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using var sw = new StringWriter();
        //    var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

        //    var viewContext = new ViewContext(
        //        ControllerContext,
        //        viewResult.View,
        //        ViewData,
        //        TempData,
        //        sw,
        //        new HtmlHelperOptions()
        //    );

        //    viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        //    return sw.ToString();
        //}
    }
}
