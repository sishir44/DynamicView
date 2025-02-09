
namespace DynamicView.Models
{
    public class DynamicDataModel
    {
        public List<string> FieldNames { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public List<Dictionary<string, object>> TtlCount { get; set; }
       // public List<string> TtlCount { get; set; }
        public string FilterColumn { get; set; }
        public string FilterVluae { get; set; }
        public List<string> Store { get; set; }
        public List<string> Department { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }

        
    }
}
