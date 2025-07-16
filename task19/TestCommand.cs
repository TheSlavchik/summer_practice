using task18;

namespace task19
{
    public class TestCommand : ILongRunningCommand
    {
        private int _id;
        private int _counter = 0;
        private int _maxExecutions;

        public TestCommand(int id, int maxExecutions)
        {
            _id = id;
            _maxExecutions = maxExecutions;
        }

        public bool IsCompleted => _counter >= _maxExecutions;

        public void Execute()
        {
            if (_counter >= _maxExecutions)
            {
                return;
            }

            Console.WriteLine($"Поток {_id} вызов {++_counter}");
            Thread.Sleep(100);
        }
    }
}
