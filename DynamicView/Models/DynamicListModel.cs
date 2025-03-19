namespace DynamicView.Models
{
    public class DynamicListModel
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public Dictionary<int, string> ReportList { get; set; }
    }
}
