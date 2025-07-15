using task17;

namespace task18
{
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _commands = new();
        private readonly object _lock = new();

        public bool HasCommand()
        {
            lock (_lock)
            {
                return _commands.Count > 0;
            }
        }

        public ICommand Select()
        {
            lock (_lock)
            {
                if (_commands.Count == 0)
                {
                    throw new InvalidOperationException("No commands");
                }

                var command = _commands.Dequeue();

                if (command is ILongRunningCommand longCmd && !longCmd.IsCompleted)
                {
                    _commands.Enqueue(command);
                }

                return command;
            }
        }

        public void Add(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            lock (_lock)
            {
                _commands.Enqueue(command);
            }
        }
    }
}
