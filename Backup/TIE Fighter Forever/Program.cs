using System;

namespace TIE_Fighter_Forever
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TIEGame game = new TIEGame())
            {
                game.Run();
            }
        }
    }
}

