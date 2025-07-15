using System.Diagnostics;
using task14;
using ScottPlot;

namespace task15
{
    internal class IntegralCalculationEfficiency
    {
        static void Main(string[] args)
        {
            int runsCount = 10;
            int maxThreads = 16;
            double optimalStep = OptimalStep(-100, 100, Math.Sin, 1e-4, 6);
            List<Coordinates> result = new();

            for (int i = 0; i < runsCount; i++)
            {
                result = CheckPerformance(-100, 100, Math.Sin, optimalStep, maxThreads);

                string dir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName, $"results{i}.png");

                Plot plot = new();
                plot.Add.Scatter(result);

                plot.SavePng(dir, 800, 600);
            }
        }

        private static double OptimalStep(double a, double b, Func<double, double> function, double targetAccuracy, int maxSteps)
        {
            double currentStep = 0.1;
            double referenceResult = DefiniteIntegral.Solve(a, b, function, Math.Pow(10, -maxSteps), 8);
            double previousResult = 0;
            double optimalStep = currentStep;

            for (int i = 1; i <= maxSteps; i++)
            {
                currentStep = Math.Pow(10, -i);
                double currentResult = DefiniteIntegral.Solve(a, b, function, currentStep, 8);
                double currentAccuracy = Math.Abs(currentResult - referenceResult);

                Console.WriteLine($"Шаг: {currentStep}, результат: {currentResult}, точность: {currentAccuracy}");

                if (currentAccuracy <= targetAccuracy)
                {
                    optimalStep = currentStep;
                    return optimalStep;
                }

                previousResult = currentResult;
            }

            return Math.Pow(10, -maxSteps);
        }

        private static List<Coordinates> CheckPerformance(double a, double b, Func<double, double> function, double step, int maxThreads)
        {
            Stopwatch stopwatch = new();
            double result;
            List<Coordinates> times = new();

            stopwatch.Restart();
            result = DefiniteIntegral.SolveSingleThread(a, b, function, step);
            stopwatch.Stop();
            times.Add(new Coordinates(1, stopwatch.Elapsed.TotalMilliseconds));

            for (int threads = 2; threads <= maxThreads; threads++)
            {
                stopwatch.Restart();
                result = DefiniteIntegral.Solve(a, b, function, step, threads);
                stopwatch.Stop();
                times.Add(new Coordinates(threads,stopwatch.Elapsed.TotalMilliseconds));
            }

            return times;
        }
    }
}
