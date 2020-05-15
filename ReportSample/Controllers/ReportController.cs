using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace ReportSample.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintFile()
        {
            LocalReport localReport = new LocalReport();

            localReport.ReportPath = @"Report/Report1.rdlc";

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";

            //The DeviceInfo settings should be changed based on the reportType

            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx


            string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>9.2in</PageWidth>
                <PageHeight>12in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.45in</MarginLeft>
                <MarginRight>0.45in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;

            string[] streams;

            byte[] renderedBytes;
            //Render the report

            renderedBytes = localReport.Render(
        reportType,
        deviceInfo,
        out mimeType,
        out encoding,
        out fileNameExtension,
        out streams,
        out warnings);

            var doc = new Document();
            var reader = new PdfReader(renderedBytes);

            using (FileStream fs = new FileStream(Server.MapPath("~/Report/Summary.pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);
                string Printer = "";
                if (Printer == null || Printer == "")
                {
                    stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                    stamper.Close();
                }
                else
                {
                    stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = " + Printer + ";print(pp);\r";
                    stamper.Close();
                }
            }
            reader.Close();

            FileStream fss = new FileStream(Server.MapPath("~/Report/Summary.pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(Server.MapPath("~/Report/Summary.pdf"));
            return File(bytes, "application/pdf");
        }
    }
}
