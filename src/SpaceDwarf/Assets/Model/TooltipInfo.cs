using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace Assets.Model
{
    [CreateAssetMenu(menuName = "Tooltip/Tooltip Info")]
    public class TooltipInfo : ScriptableObject
    {
        public string Label;
        public Sprite Thumbnail;
        public string ThumbnailFlavor;
        public string Paragraph;
    }
}
