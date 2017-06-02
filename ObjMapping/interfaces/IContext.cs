namespace ObjMapping.Interfaces
{
    public interface IContext
    {
        object Value1 { get; }
        object Value2 { get; }

        T GetObject1<T>();
        T GetObject2<T>();
    }
}
