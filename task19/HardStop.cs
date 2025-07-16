using task18;
using task17;

namespace task19
{
    public class HardStop : ICommand
    {
        private ConcurrentSchedulerThread _target;

        public HardStop(ConcurrentSchedulerThread target)
        {
            _target = target;
        }

        public void Execute()
        {
            _target.Stop();
        }
    }
}
