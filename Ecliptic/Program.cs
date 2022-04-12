using System;
using Ecliptic.Core;

namespace Ecliptic
{
    class Program
    {
        static void Main(string[] args)
        {
            using(Game game = new Game())
            {
                game.Run(World.GameConstants.PHYSIC_FRAME_RATE);
            }
        }
    }
}
