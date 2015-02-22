using System;

namespace TIE_Fighter_Forever
{
    static class Program
    {
        /// <summary>
        /// Itt van az alkalmazásunk belépési pontja
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

