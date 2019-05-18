using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniFRCDriver
{
    class InputIdentifier
    {
        static public Key identifier(string input)
        {
            switch (input)
            {
                case "a":
                    return Key.A;
                case "b":
                    return Key.B;
                case "c":
                    return Key.C;
                case "d":
                    return Key.D;
                case "e":
                    return Key.E;
                case "f":
                    return Key.F;
                case "g":
                    return Key.G;
                case "h":
                    return Key.H;
                case "i":
                    return Key.I;
                case "j":
                    return Key.J;
                case "k":
                    return Key.K;
                case "l":
                    return Key.L;
                case "m":
                    return Key.M;
                case "n":
                    return Key.N;
                case "o":
                    return Key.O;
                case "p":
                    return Key.P;
                case "q":
                    return Key.Q;
                case "r":
                    return Key.R;
                case "s":
                    return Key.S;
                case "t":
                    return Key.T;
                case "u":
                    return Key.U;
                case "v":
                    return Key.V;
                case "w":
                    return Key.W;
                case "x":
                    return Key.X;
                case "y":
                    return Key.Y;
                case "z":
                    return Key.Z;
                case "space":
                    return Key.Space;
                case "rCtrl":
                    return Key.RightCtrl;
                case "lCtrl":
                    return Key.LeftCtrl;
                case "lShift":
                    return Key.LeftShift;
                case "rShift":
                    return Key.RightShift;
                default:
                    return Key.Escape;
            }
        }
    }
}
