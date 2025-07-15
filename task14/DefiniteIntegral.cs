﻿namespace task14
{
    public class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            if (threadsNumber <= 0) 
            { 
                throw new ArgumentException("Number of threads must be positive", nameof(threadsNumber)); 
            }

            if (step <= 0)
            {
                throw new ArgumentException("Step must be positive", nameof(step));
            }

            object lock_ = new();
            double total = 0;
            double segmentLength = (b - a) / threadsNumber;
            Task[] tasks = new Task[threadsNumber];

            for (int i = 0; i < threadsNumber; i++)
            {
                double start = a + i * segmentLength;
                double end;

                if (i == threadsNumber - 1)
                {
                    end = b;
                }
                else
                {
                    end = start + segmentLength;
                }

                tasks[i] = Task.Run(() =>
                {
                    double partialSum = CalculatePartialIntegral(start, end, function, step);

                    lock (lock_)
                    {
                        total += partialSum;
                    }
                });
            }

            Task.WaitAll(tasks);
            return total;
        }

        private static double CalculatePartialIntegral(double a, double b, Func<double, double> function, double step)
        {
            double sum = 0.0;
            double x = a;

            while (x < b)
            {
                double xNext = Math.Min(x + step, b);
                sum += (function(x) + function(xNext)) * (xNext - x) / 2;
                x = xNext;
            }

            return sum;
        }

        public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
        {
            double sum = 0;
            double x = a;

            while (x < b)
            {
                double xNext = Math.Min(x + step, b);
                sum += (function(x) + function(xNext)) * (xNext - x) / 2;
                x = xNext;
            }

            return sum;
        }
    }
}
