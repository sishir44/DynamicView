﻿namespace DynamicView.Models
{
    public class DynamicReportListModel
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public Dictionary<int, string> ReportList { get; set; }
    }
}
