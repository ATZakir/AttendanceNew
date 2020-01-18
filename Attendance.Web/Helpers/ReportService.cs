using System;
using System.Web.Mvc;

namespace Attendance.Web.Helpers
{
    public class ReportService : Controller
    {
        //public readonly IReportConfigurationService reportConfigurationService;
        public ReportService()
        {
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

    }
}