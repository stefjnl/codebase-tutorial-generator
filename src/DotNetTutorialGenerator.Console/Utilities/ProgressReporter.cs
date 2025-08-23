using System;
using System.Text;

namespace DotNetTutorialGenerator.Cli.Utilities
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
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
                    System.Console.Write(new string(' ', System.Console.WindowWidth));
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
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
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
                    System.Console.Write(new string(' ', System.Console.WindowWidth));
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
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
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
                    System.Console.Write(progressText.PadRight(System.Console.WindowWidth - 1));
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);

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
