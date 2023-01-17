using Gomez.Core;
using Gomez.Factorio.Options;

namespace Gomez.Factorio.Utils
{
    internal class DailyScheduler : IDisposable
    {
        private readonly ApplicationOption _option;
        private readonly CancellationToken _ct;

        private CancellationTokenSource? _cts = new();
        private DateOnly? _lastRestart;
        private bool _disposedValue;

        public DailyScheduler(ApplicationOption option, CancellationToken ct)
        {
            _option = option;
            _ct = ct;
            SetLastRestartIfDue();
        }

        public event EventHandler? Invoked;

        public Task StartAsync()
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts!.Token, _ct);
            return Task.Factory.StartNew(
                () => PeriodicTask.RunUntilAsync(
                    () =>
                        {
                            Invoked?.Invoke(this, new EventArgs());
                        },
                    () => CanBeInvoked(),
                    TimeSpan.FromSeconds(1),
                    linkedCts.Token),
                linkedCts.Token);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && _cts is not null)
                {
                    _cts.Dispose();
                    _cts = null;
                }

                _disposedValue = true;
            }
        }

        private bool CanBeInvoked()
        {
            var currentDateTime = DateTime.Now;
            var currentDate = DateOnly.FromDateTime(currentDateTime);
            if (currentDate == _lastRestart)
            {
                return false;
            }

            var currentTime = TimeOnly.FromDateTime(currentDateTime);
            if (currentTime >= _option.RestartAfter)
            {
                _lastRestart = currentDate;
                return true;
            }

            return false;
        }

        private void SetLastRestartIfDue()
        {
            var currentDateTime = DateTime.Now;
            var currentTime = TimeOnly.FromDateTime(currentDateTime);
            if (_lastRestart == null && currentTime > _option.RestartAfter)
            {
                var currentDate = DateOnly.FromDateTime(currentDateTime);
                _lastRestart = currentDate;
            }
        }
    }
}
