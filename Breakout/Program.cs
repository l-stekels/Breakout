using System;

namespace Breakout
{
    class Program
    {
        static void Main(string[] args)
        {
            using Window window = Window.WindowFactory("Breakout", 1024, 800);
            window.Run();
        }
    }
}
