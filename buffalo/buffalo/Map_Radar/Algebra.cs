using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace buffalo.Map_Radar
{
    static class Algebra
    {
        public static Vector2 Intercept(Vector2 p1, Vector2 direction1, Vector2 p2, Vector2 direction2)
        {
            Vector2 delta = p1 - p2;
            float nom = (direction1.X * direction2.Y) - (direction1.Y * direction2.X);  //anstieg vergleichen

            if (nom == 0)
                return Vector2.Zero;

            float fac1 = (delta.Y * direction2.X) - (delta.X * direction2.Y);
            fac1 /= nom;

            float fac2 = (delta.Y * direction1.X) - (delta.X * direction1.Y);
            fac2 /= nom;

            if ((fac1 < 0 || fac1 > 1) || (fac2 < 0 || fac2 > 1)) //out of range
                return Vector2.Zero;

            return (p1 + fac1 * direction1);
        }
    }
}
