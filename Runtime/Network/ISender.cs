
namespace klib
{
    public interface ISender
    {
        void Send<T>(T data) where T : class;
        void Close();
    }
}
