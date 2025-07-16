using task17;
using System.Collections.Concurrent;

namespace task18
{
    public class ConcurrentSchedulerThread : IDisposable
    {
        public bool IsRunning => _isRunning;

        private Thread _thread;
        private BlockingCollection<ICommand> _commandQueue = new();
        private IScheduler _scheduler;
        private bool _isRunning = true;
        public event Action<ICommand, Exception>? ExceptionHandler;

        public ConcurrentSchedulerThread(IScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            _scheduler = scheduler;

            _thread = new Thread(Run)
            {
                IsBackground = true
            };

            _thread.Start();
        }

        private void Run()
        {
            while (_isRunning)
            {
                if (_scheduler.HasCommand())
                {
                    var command = _scheduler.Select();
                    ExecuteCommand(command);
                    continue;
                }

                if (_commandQueue.TryTake(out ICommand? newCommand, 100))
                {
                    if (newCommand is ILongRunningCommand)
                    {
                        _scheduler.Add(newCommand);
                    }
                    else
                    {
                        ExecuteCommand(newCommand);
                    }
                }
            }
            
            //_commandQueue.Dispose();
        }

        private void ExecuteCommand(ICommand? command)
        {
            if (command == null)
            {
                return;
            }

            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(command, ex);
            }
        }

        public void Post(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (!_isRunning)
            {
                throw new InvalidOperationException("Thread is not running");
            }

            _commandQueue.Add(command);
        }

        public void Stop() => _isRunning = false;

        public void Dispose()
        {
            Stop();
            _commandQueue.CompleteAdding();

            if (_thread.IsAlive)
            {
                _thread.Join(TimeSpan.FromSeconds(5));
            }

            GC.SuppressFinalize(this);
        }
    }
}
