namespace Gomez.Core
{
    public class DebounceWrapper
    {
        private CancellationTokenSource? _cancelTokenSource = null;

        public async Task DebounceAsync(Func<Task> func, int milliseconds = 1000)
        {
            _cancelTokenSource?.Cancel();
            var oldToken = _cancelTokenSource;
            _cancelTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(milliseconds, _cancelTokenSource.Token);
            }
            catch (OperationCanceledException) { }

            if (oldToken?.IsCancellationRequested != true)
            {
                await func();
            }
        }
    }
}
