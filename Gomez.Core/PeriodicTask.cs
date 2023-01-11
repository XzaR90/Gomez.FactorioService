namespace Gomez.Core
{
    public static class PeriodicTask
    {
        public static async Task RunUntilAsync(Action action, Func<bool> condition, TimeSpan interval, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && !condition())
            {
                await Task.Delay(interval, cancellationToken);

                if (!cancellationToken.IsCancellationRequested && condition())
                {
                    action();
                }
            }
        }
    }
}
