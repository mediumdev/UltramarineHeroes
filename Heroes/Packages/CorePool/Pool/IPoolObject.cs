namespace Pool
{
    public interface IPoolObject
    {
        IPoolObject Origin { get; }
        IPoolObject LoadObject(IPoolObject origin);
        int PreloadCount { get; }
        void OnPop();
        void OnPush();
    }
}
