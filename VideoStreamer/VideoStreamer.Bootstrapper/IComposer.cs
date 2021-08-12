namespace VideoStreamer.Bootstrapper
{
    public interface IComposer
    {
        IComposer RegisterType<T, K>() where K : T;
    }
}
