using System.Diagnostics;

namespace ExcelTest
{
    public static class TimeCount
    {
        public static Stopwatch StartTimer()
        {
            var watch = new Stopwatch();
            watch.Start();
            return watch;
        }

        public static long GetMilliseconds(this Stopwatch timer)
        {
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
        public static long GetSeconds(this Stopwatch timer)
        {
            timer.Stop();
            return timer.ElapsedMilliseconds/1000;
        }
    }
}