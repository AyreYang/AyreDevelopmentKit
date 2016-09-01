
namespace Task.common.utilities
{
    public class TransData
    {
        public object Data { get; private set; }

        public TransData(object data)
        {
            Data = data;
        }

        public bool IsTypeOf<T>()
        {
            if (Data == null) return false;
            return (Data is T);
        }

        public T GetData<T>()
        {
            T data = default(T);
            try
            {
                data = (T)Data;
            }
            catch { }
            //if (Data != null && Data.GetType() is T)
            //{
            //    data = (T)Data;
            //}
            return data;
        }
    }
}
