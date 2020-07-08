using UnityEditor;
using UnityEngine;

namespace NanoEcs
{
    public static class NanoEditorHelper
    {
        static Color greyColor
        {
            get
            {
                return new Color(0, 0, 0, 0.15f);
            }
        }

        static Texture2D _backgroundTexture;
        static Texture2D backgroundTexture
        {
            get
            {
                if (_backgroundTexture == null)
                {
                    _backgroundTexture = TextureTools.MakeTex(3, 3, greyColor);
                }
                return _backgroundTexture;
            }
        }

        public static GUIStyle backStyle(int seed = 0)
        {
            var r = new System.Random(seed);
            var hue = (float)r.NextDouble();
            var color = Color.HSVToRGB(hue, 0.3f, 0.5f); color.a = 0.3f;
            GUIStyle gsAlterQuest = new GUIStyle();
            gsAlterQuest.normal.background = backgroundTexture;
            return gsAlterQuest;
        }

        static Color backColor
        {
            get
            {
                return new Color(.58f, .58f, .58f);
            }
        }

        public static GUIStyle SettingsTitleStyle
        {
            get
            {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleLeft;
                style.contentOffset = new Vector2(5, 0);
                style.fontStyle = FontStyle.Italic;
                return style;
            }
        }

    }

    public static class TextureTools
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}