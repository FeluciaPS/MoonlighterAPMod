using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MoonlighterAPMod.Archipelago
{
    internal static class SpriteLoader
    {
        private static readonly Dictionary<string, Sprite> cache = [];
        private static readonly string BasePath = Path.GetDirectoryName(typeof(SpriteLoader).Assembly.Location);
        private static readonly string SpriteFolder = Path.Combine(BasePath, "Sprites");

        internal static Sprite GetSprite(string name)
        {
            if (cache.TryGetValue(name, out var cached))
                return cached;

            string path = Path.Combine(SpriteFolder, name + ".png");

            if (!File.Exists(path))
            {
                Debug.LogError($"[MoonlighterAPMod] Sprite not found: {path}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            var texture = new Texture2D(6, 9);

            if (!ImageConversion.LoadImage(texture, fileData))
            {
                Debug.LogError($"[MoonlighterAPMod] Failed to decode sprite: {path}");
                return null;
            }

            texture.filterMode = FilterMode.Point;
            texture.name = name;

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                1f
            );

            sprite.name = name;

            cache[name] = sprite;
            return sprite;
        }
    }
}
