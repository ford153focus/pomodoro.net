using System;
using System.Threading.Tasks;

namespace pomodoro.net
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("1. Work timer");
            // Console.WriteLine("2. Add 5 minutes");
            // Console.WriteLine("3. End of working day");

            Task.Run((Action)Pause.Watchdog);

            for (int i = 0; i < 25; i++)
            {
                Timer.Work();
                Timer.Break();
                Console.WriteLine("Press any key to continue...");
                Pause.TogglePause();
                Console.ReadKey();
                Pause.TogglePause();
            }
        }


    }

    class Timer
    {
        private static int workPeriod = 1500;
        private static int breakPeriod = 300;
        public static void Work()
        {
            var secondsLeft = workPeriod;
            while (secondsLeft > 0)
            {
                Console.WriteLine(secondsLeft);
                secondsLeft--;
                System.Threading.Thread.Sleep(1000);
            }
            System.Diagnostics.Process.Start("qdbus", "org.freedesktop.ScreenSaver /ScreenSaver Lock");
        }
        public static void Break()
        {
            var secondsLeft = breakPeriod;
            while (secondsLeft > 0)
            {
                Console.WriteLine(secondsLeft);
                secondsLeft--;
                System.Threading.Thread.Sleep(1000);
            }
            System.Diagnostics.Process.Start("loginctl", "unlock-session");
        }
    }

    class Pause
    {
        public static bool isPaused = false;
        public static DateTime? pauseDate = null;
        public static void TogglePause()
        {
            if (isPaused)
            {
                isPaused = false;
                pauseDate = null;
            }
            else
            {
                isPaused = true;
                pauseDate = DateTime.Now;
            }
        }
        public static void Watchdog()
        {
            while (true)
            {
                if (isPaused)
                {
                    if (pauseDate.Value.AddSeconds(60) < DateTime.Now)
                    {
                        System.Diagnostics.Process.Start("kdialog", "--title 'Pomodoro.NET' --passivepopup 'Timer is paused too long. Are you still alive there?' 15");
                        System.Diagnostics.Process.Start("x-www-browser", "'data:text/html,<h1>Pomodoro.NET waiting YOU to start next time segment!</h1>'");
                    }
                }
                System.Threading.Thread.Sleep(60000);
            }
        }
    }
}
