using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class SpriteProcessor : AssetPostprocessor
    {
        //todo: configurable
        private const string SpritePathSearchToken = "sprites";
        private const int SpritePixelsPerUnit = 32;
        void OnPostprocessTexture(Texture2D texture)
        {
            var path = assetPath.ToLowerInvariant();
            var isInSpriteDir = path.Contains(SpritePathSearchToken);

            if (!isInSpriteDir)
                return;
            
            //todo: check for predefined sprite slicing file

            var importer = assetImporter as TextureImporter;
            if (importer == null)
            {
                throw new InvalidCastException("Unable to cast asset importer to TextureImporter");
            }

            Debug.Log("Imported Sprite");
        }
    }
}
