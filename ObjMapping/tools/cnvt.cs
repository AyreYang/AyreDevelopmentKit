namespace ObjMapping.tools
{
    public struct cnvt
    {
        public bool Mapping { get; set; }
        public object Value { get; set; }

        public cnvt(bool mapping, object value)
        {
            Mapping = mapping;
            Value = value;
        }
    }
}
