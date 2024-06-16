namespace webapi.Model
{
    public class ValuePairs
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public ValuePairs(string Name, int Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }

    public class CustomEnum
    {
        public string Name { get; set; }
        public List<ValuePairs> Values { get; set; }

        public CustomEnum(string Name)
        {
            this.Name = Name;
            Values = new List<ValuePairs>();
        }

        public void AddValue(string Name, int Value)
        {
            Values.Add(new ValuePairs(Name, Value));
        }
    }
}
