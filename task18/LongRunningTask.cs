namespace task18
{
    public class LongRunningTask : ILongRunningCommand
    {
        private int _stepsRemaining;

        public LongRunningTask(int steps)
        {
            _stepsRemaining = steps;
        }

        public bool IsCompleted => _stepsRemaining <= 0;

        public void Execute()
        {
            if (_stepsRemaining <= 0)
            {
                return;
            }

            Thread.Sleep(50);
            _stepsRemaining--;
        }
    }
}
