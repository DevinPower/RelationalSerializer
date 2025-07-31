namespace webapi.Model
{
    public class ValuePairs
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public ValuePairs(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class CustomEnum
    {
        public string Name { get; set; }
        public List<ValuePairs> Values { get; set; }

        public CustomEnum(string name)
        {
            this.Name = name;
            Values = new List<ValuePairs>();
        }

        public void AddValue(string name, int value)
        {
            Values.Add(new ValuePairs(name, value));
        }
    }
}
