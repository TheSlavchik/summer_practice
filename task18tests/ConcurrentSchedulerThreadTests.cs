using task17;
using task18;

namespace task18tests
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }

    public class ConcurrentSchedulerThreadTests
    {
        [Fact]
        public void Thread_ExecutesRegularCommandsImmediately()
        {
            var scheduler = new RoundRobinScheduler();
            var thread = new ConcurrentSchedulerThread(scheduler);
            bool executed = false;

            thread.Post(new ActionCommand(() => executed = true));
            Thread.Sleep(100);

            Assert.True(executed);
            thread.Dispose();
        }

        [Fact]
        public void Thread_HandlesLongRunningCommandsWithScheduler()
        {
            var scheduler = new RoundRobinScheduler();
            using var thread = new ConcurrentSchedulerThread(scheduler);
            var longTask = new LongRunningTask(3);
            bool shortTaskExecuted = false;
            var shortTaskExecutedSignal = new ManualResetEvent(false);

            thread.Post(longTask);

            Thread.Sleep(100);

            thread.Post(new ActionCommand(() => {
                shortTaskExecuted = true;
                shortTaskExecutedSignal.Set();
            }));

            Assert.True(shortTaskExecutedSignal.WaitOne(TimeSpan.FromSeconds(1)));
            Assert.True(shortTaskExecuted);

            var completionSignal = new ManualResetEvent(false);

            var checkThread = new Thread(() => {
                while (!longTask.IsCompleted)
                {
                    Thread.Sleep(50);
                }
                completionSignal.Set();
            });
            checkThread.Start();

            Assert.True(completionSignal.WaitOne(TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public void RoundRobinScheduler_ExecutesCommandsFairly()
        {
            var scheduler = new RoundRobinScheduler();
            var task1 = new LongRunningTask(2);
            var task2 = new LongRunningTask(2);
            var task3 = new LongRunningTask(2);

            scheduler.Add(task1);
            scheduler.Add(task2);
            scheduler.Add(task3);

            var command1 = scheduler.Select();
            Assert.Same(task1, command1);

            var command2 = scheduler.Select();
            Assert.Same(task2, command2);

            var command3 = scheduler.Select();
            Assert.Same(task3, command3);

            command1.Execute();
            var command4 = scheduler.Select();
            Assert.Same(task1, command4);
        }

        [Fact]
        public void Thread_StopsWhenRequested()
        {
            var scheduler = new RoundRobinScheduler();
            var thread = new ConcurrentSchedulerThread(scheduler);

            thread.Stop();
            Thread.Sleep(100);

            Assert.False(thread.IsRunning);
        }

        [Fact]
        public void Thread_HandlesExceptions()
        {
            var scheduler = new RoundRobinScheduler();
            var thread = new ConcurrentSchedulerThread(scheduler);
            Exception? caughtException = null;

            thread.ExceptionHandler += (command, ex) => caughtException = ex;
            thread.Post(new ActionCommand(() => throw new InvalidOperationException("Test exception")));

            Thread.Sleep(100);
            Assert.NotNull(caughtException);
            Assert.IsType<InvalidOperationException>(caughtException);

            thread.Dispose();
        }
    }
}
