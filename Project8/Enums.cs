using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project8
{
    public class Enums
    {
        public enum GameTypeChoice
        {
            SinglePlayer = 1,
            MultiPlayer = 2
        }

        public enum TileType
        {
            Empty,
            X,
            O,
            K,
            U
        }
    }
}