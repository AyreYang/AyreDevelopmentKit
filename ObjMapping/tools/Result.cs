namespace ObjMapping.Tools
{
    public struct Result
    {
        public bool Mapping { get; set; }
        public object Value { get; set; }

        public Result(bool mapping, object value = null)
        {
            Mapping = mapping;
            Value = value;
        }
    }
}
