using System.Text;

namespace Covenant.Common.Models.Request
{
    public class RequestsHistoryReportModel
    {
        public const string Headers = "COMPANY INDUSTRY,REQUEST INDUSTRY,JOB POSITION,JOB TITLE,TIMES";
        public string CompanyIndustry { get; set; }
        public string RequestIndustry { get; set; }
        public string JobPosition { get; set; }
        public string JobTitle { get; set; }
        public int Times { get; set; }
        public override string ToString() =>
            $"{CompanyIndustry?.Replace(",", string.Empty)},{RequestIndustry?.Replace(",", string.Empty)},{JobPosition?.Replace(",", string.Empty)},{JobTitle?.Replace(",", string.Empty)},{Times}";

        public static string ToSvc(IEnumerable<RequestsHistoryReportModel> list)
        {
            var b = new StringBuilder();
            b.AppendLine(Headers);
            foreach (RequestsHistoryReportModel m in list) b.AppendLine(m.ToString());
            return b.ToString();
        }
    }
}