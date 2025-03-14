
namespace DynamicView.Models
{
    public class DynamicDataModel
    {
        public List<string> FieldNames { get; set; }
        public List<string> FieldNamesAlias { get; set; }
        public List<string> isFixedCol { get; set; }
        public List<string> isFixedM { get; set; }
        public List<string> isFilterCol { get; set; }
        public bool FixedCard { get; set; }
        public List<string> isSubTotalCol { get; set; }
        public Dictionary<string, short?> NoOfDecimal { get; set; }
        public Dictionary<string, bool?> ShowCard { get; set; }
        public Dictionary<string, bool?> isPercent { get; set; }
        public List<Dictionary<string, object>> TableData { get; set; }
        public List<Dictionary<string, object>> TotalSum { get; set; }
        public List<Dictionary<string, object>> SubTotalResults { get; set; }
        public List<Dictionary<string, object>> TotalMMM { get; set; }
        public List<Dictionary<string, object>> TotalTM { get; set; }
        
        //public List<string> SubTotalResults { get; set; }
        public List<List<Dictionary<string, object>>> GetDynamicReport { get; set; }

        public bool isMobile { get; set; }
        public List<string> firstRowColumn { get; set; }
        
        public int TotalCount { get; set; }
        public int ColorCodeCnt1 { get; set; }
        public int ColorCodeCnt2 { get; set; }
        public int ColorCodeCnt3 { get; set; }

        public int TableDataCount { get; set; }

        // public List<string> TtlCount { get; set; }
        public string FilterColumn { get; set; }
        public string FilterVluae { get; set; }
        public List<string> Store { get; set; }
        public List<string> Department { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }

        public string ReportName { get; set; }
        public string Color { get; set; }
        public string AlternateRowColor { get; set; }



        // color code property
        public List<string> ColorAliasName { get; set; }
        public float? ColorValue1 { get; set; }
        public string ColorCode1 { get; set; }
        public float? ColorValue2 { get; set; }
        public string ColorCode2 { get; set; }
        public float? ColorValue3 { get; set; }
        public string ColorCode3 { get; set; }


    }
}
