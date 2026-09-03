using UnityEditor;
using UnityEngine;

public class PixelArtImportSettings : AssetPostprocessor
{
    private const string ArtRoot = "Assets/Art/";
    private const int PixelsPerUnit = 48;

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtRoot))
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        bool isOverlay = assetPath.Contains("/Tiles/overlay_");
        importer.filterMode = isOverlay ? FilterMode.Bilinear : FilterMode.Point;

        bool isTile = assetPath.Contains("/Tiles/tile_");
        importer.wrapMode = isTile ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        settings.spriteAlignment = assetPath.EndsWith("cannon_barrel.png")
            ? (int)SpriteAlignment.BottomCenter
            : (int)SpriteAlignment.Center;

        bool isBackgroundTile = assetPath.Contains("/Tiles/");
        settings.spriteMeshType = isBackgroundTile
            ? SpriteMeshType.FullRect
            : SpriteMeshType.Tight;

        importer.SetTextureSettings(settings);
    }
}
