
using System.Data;

namespace DynamicView.Models
{
    public class DynamicDataModel
    {
        // DataTable Property
        public DataTable FirstTable { get; set; }
        public DataTable SecondTable { get; set; }
        public DataTable ThirdTable { get; set; }
        public DataTable FourthTable { get; set; }
        public DataTable FifthTable { get; set; }
        public DataTable SixthTable { get; set; }
        public DataTable SeventhTable { get; set; }
        public DataTable EighthTable { get; set; }
        public DataTable NinthTable { get; set; }
        public DataTable ShowCardTbl { get; set; }

        public bool FixedCard { get; set; }
        public bool isMobile { get; set; }
        public string ReportName { get; set; }
        public List<string> FieldNames { get; set; }
        public List<string> FieldNamesAlias { get; set; }
        public List<string> isFixedCol { get; set; }
        public List<string> isFixedM { get; set; }
        public List<string> isFilterCol { get; set; }
        public List<string> firstRowColumn { get; set; }
        public Dictionary<string, short?> NoOfDecimal { get; set; }
        public Dictionary<string, bool?> ShowCard { get; set; }
        public Dictionary<string, bool?> isPercent { get; set; }
        public List<Dictionary<string, object>> TableData { get; set; }
        public List<Dictionary<string, object>> GrandTotalSum { get; set; }
        public List<Dictionary<string, object>> SubTotalResults { get; set; }
        public List<Dictionary<string, object>> TotalMMM { get; set; }
        public List<Dictionary<string, object>> TotalTM { get; set; }

        // color code property
        public string Color { get; set; }
        public string AlternateRowColor { get; set; }
        public float? ColorValue1 { get; set; }
        public string ColorCode1 { get; set; }
        public float? ColorValue2 { get; set; }
        public string ColorCode2 { get; set; }
        public float? ColorValue3 { get; set; }
        public string ColorCode3 { get; set; }
        public List<string> ColorAliasName { get; set; }


    }
}
