namespace ProjectsNow.Data.Library
{
    public class CalculationItem
    {
        public string GroupId { get; set; }
        public string ItemType { get; set; }
        public string ItemTable { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PropertyId { get; set; }
        public int i { get; set; }
        public int j { get; set; }
        public double? Value { get; set; }
        public string Condition { get; set; }
        public int n { get; set; }
        public int m { get; set; }
        public string ConditionValue { get; set; }
        public static string TableName => "CalculationItems";
    }
}
