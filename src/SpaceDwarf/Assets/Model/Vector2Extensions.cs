using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Model
{
    public static class Vector2Extensions
    {
        public static Vector2 Translate(this Vector2 u, Vector2 translation)
        {
            return new Vector2(u.x + translation.x, u.y + translation.y);
        }
    }
}
