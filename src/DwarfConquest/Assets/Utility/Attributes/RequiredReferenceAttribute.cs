using System;
using UnityEngine;

namespace Assets.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class RequireReferenceAttribute : PropertyAttribute
    {
        public readonly Color Highlight;

        public RequireReferenceAttribute()
        {
            Highlight = Color.red;
        }

        public RequireReferenceAttribute(float r, float g, float b, float a = 1f)
        {
            Highlight = new Color(r, g, b, a);
        }
    }
}
