using Log.common.enums;

namespace Log.common.interfaces
{
    public interface ILog
    {
        long Write(TYPE type, string message);
    }
}
