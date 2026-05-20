using Archipelago.MultiClient.Net.Models;
using HarmonyLib;
using nickeltin.SDF.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Mod.Classes
{
    public class ArchipelagoShopitem : Item
    {
        public ScoutedItemInfo ItemInfo { get; private set; }

        private static Texture2D _cachedTexture;
        private static Sprite _cachedSprite;

        private static Texture2D _cachedSDFTexture;
        private static Sprite _cachedSDFSprite;

        public ArchipelagoShopitem() { }

        public ArchipelagoShopitem(ScoutedItemInfo itemInfo, bool isSticker = true) : base()
        {
            Cost = 10;
            ItemInfo = itemInfo;
            Name = itemInfo.ItemDisplayName;

            if (isSticker)
            {
                UpgradeableComponents = new List<UpgradeableComponent>
                {
                    new UpgradeableComponent(0, 0, 0)
                };
            }

            // Add sprite data
            SpriteData.Add(new ItemSpriteData(ItemSpriteUsage.Default, "Archipelago"));
        }

        public override Sprite GetCurrentSprite()
        {
            return GetSprite();
        }

        public override SDFSpriteMetadataAsset GetCurrentSDFSprite()
        {
            if (_cachedSDFSprite == null)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mod.Sprites.SDF.archipelago.png"))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    _cachedSDFTexture = new Texture2D(128, 128, TextureFormat.Alpha8, false);
                    ImageConversion.LoadImage(_cachedSDFTexture, bytes);

                    _cachedSDFTexture.Apply();
                    _cachedSDFSprite = Sprite.Create(
                        _cachedSDFTexture,
                        new Rect(0, 0, _cachedSDFTexture.width, _cachedSDFTexture.height),
                        new Vector2(0.5f, 0.5f),
                        80f);
                }
            }

            SDFSpriteMetadata metadata = new SDFSpriteMetadata();
            metadata.SourceSprite = GetSprite();
            metadata.SDFSprite = _cachedSDFSprite;
            metadata.BorderOffset = new Vector4(10, 10, 10, 10);

            SDFSpriteMetadataAsset asset = ScriptableObject.CreateInstance<SDFSpriteMetadataAsset>();
            Traverse.Create(asset).Field("_metadata").SetValue(metadata);
            return asset;
        }

        public static Sprite GetSprite()
        {
            if (_cachedSprite != null)
            {
                return _cachedSprite;
            }

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mod.Sprites.archipelago.png"))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                _cachedTexture = new Texture2D(512, 512);
                ImageConversion.LoadImage(_cachedTexture, bytes);
                _cachedTexture.Compress(true);
                _cachedTexture.Apply();

                _cachedSprite = Sprite.Create(
                    _cachedTexture,
                    new Rect(0, 0, _cachedTexture.width, _cachedTexture.height),
                    new Vector2(0.5f, 0.5f),
                    80f);
            }

            return _cachedSprite;
        }

        public override string GetDescription()
        {
            return $"This is a {ItemInfo.Flags} item for {ItemInfo.Player.Name}";
        }
    }
}
