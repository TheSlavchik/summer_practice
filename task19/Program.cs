using task18;

namespace task19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new RoundRobinScheduler();
            var thread = new ConcurrentSchedulerThread(scheduler);

            for (int i = 0; i < 5; i++)
            {
                var command = new TestCommand(i, 3);
                thread.Post(command);
            }

            Thread.Sleep(2000);
            thread.Post(new HardStop(thread));
            thread.Dispose();

            Console.WriteLine("Все команды выполнены");
        }
    }
}
