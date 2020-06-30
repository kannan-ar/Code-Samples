namespace PurchaseHub.Extensions
{
    using System.Threading.Tasks;
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            if(!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }
        }

        async static Task ForgetAwaited(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }
}