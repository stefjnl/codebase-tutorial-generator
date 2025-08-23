using System.Text;

namespace DotNetTutorialGenerator.Console.Utilities
{
    public class ProgressReporter
    {
        private readonly ConsoleLogger _logger;
        private readonly bool _useSpinner;
        private readonly object _lock = new object();
        private bool _isRunning = false;
        private int _currentProgress = 0;
        private string _currentMessage = string.Empty;
        private CancellationTokenSource? _cancellationTokenSource;

        public ProgressReporter(ConsoleLogger logger, bool useSpinner = true)
        {
            _logger = logger;
            _useSpinner = useSpinner;
        }

        public void StartProgress(string initialMessage)
        {
            lock (_lock)
            {
                _currentMessage = initialMessage;
                _currentProgress = 0;
                _isRunning = true;

                if (_useSpinner)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    Task.Run(() => AnimateSpinner(_cancellationTokenSource.Token));
                }
                else
                {
                    _logger.LogInformation(initialMessage);
                }
            }
        }

        public void UpdateProgress(int progress, string message)
        {
            lock (_lock)
            {
                _currentProgress = progress;
                _currentMessage = message;
            }
        }

        public void CompleteProgress(string finalMessage)
        {
            lock (_lock)
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();

                if (_useSpinner)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                }

                _logger.LogInformation(finalMessage);
            }
        }

        public void ReportError(string errorMessage)
        {
            lock (_lock)
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();

                if (_useSpinner)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                }

                _logger.LogError(errorMessage);
            }
        }

        private async Task AnimateSpinner(CancellationToken cancellationToken)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            var spinnerIndex = 0;

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                lock (_lock)
                {
                    if (!_isRunning) break;

                    var progressText = $"[{spinner[spinnerIndex]}] {_currentMessage}";
                    if (_currentProgress > 0)
                    {
                        progressText += $" ({_currentProgress}%)";
                    }

                    // Clear the line and write the new progress
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(progressText.PadRight(Console.WindowWidth - 1));
                    Console.SetCursorPosition(0, Console.CursorTop);

                    spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                }

                try
                {
                    await Task.Delay(100, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
