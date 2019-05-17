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
        public Key identifier(string input)
        {
            switch (input)
            {
                case "a":
                    return Key.A;
                    break;
                case "b":
                    return Key.B;
                    break;
                case "c":
                    return Key.C;
                    break;
                case "d":
                    return Key.D;
                    break;
                case "e":
                    return Key.E;
                    break;
                case "f":
                    return Key.F;
                    break;
                case "g":
                    return Key.G;
                    break;
                case "h":
                    return Key.H;
                    break;
                case "i":
                    return Key.I;
                    break;
                case "j":
                    return Key.J;
                    break;
                case "k":
                    return Key.K;
                    break;
                case "l":
                    return Key.L;
                    break;
                case "m":
                    return Key.M;
                    break;
                case "n":
                    return Key.N;
                    break;
                case "o":
                    return Key.O;
                    break;
                case "p":
                    return Key.P;
                    break;
                case "q":
                    return Key.Q;
                    break;
                case "r":
                    return Key.R;
                    break;
                case "s":
                    return Key.S;
                    break;
                case "t":
                    return Key.T;
                    break;
                case "u":
                    return Key.U;
                    break;
                case "v":
                    return Key.V;
                    break;
                case "w":
                    return Key.W;
                    break;
                case "x":
                    return Key.X;
                    break;
                case "y":
                    return Key.Y;
                    break;
                case "z":
                    return Key.Z;
                    break;
                case "space":
                    return Key.Space;
                    break;
                case "rCtrl":
                    return Key.RightCtrl;
                    break;
                case "lCtrl":
                    return Key.LeftCtrl;
                    break;
                case "lShift":
                    return Key.LeftShift;
                    break;
                case "rShift":
                    return Key.RightShift;
                    break;
                default:
                    return Key.Escape;
                    break;
            }
        }
    }
}
