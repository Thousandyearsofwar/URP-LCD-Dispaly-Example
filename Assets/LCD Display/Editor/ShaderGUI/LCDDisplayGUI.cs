﻿using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    public static class LCDDisplayGUI
    {
        public enum PixelLayoutMode
        {
            Square = 0,
            OffsetSquare = 1,
            Arrow = 2,
            Triangular = 3
        }

        public enum ColorWashoutMode
        {
            None = 0,
            Simple = 1,
            IPS = 2,
            VA = 3,
            TN = 4
        }

        public static class Styles
        {
            public static GUIContent pixelMaskText =
                new GUIContent("Pixel Mask Map", "Sets and configures the pixel mask map for the LCD Display.");

            public static GUIContent pixelLumaText =
                new GUIContent("Pixel Luma", "Sets the pixel luminace scale of the Pixel Mask map.");

            public static GUIContent pixelLayoutText =
                new GUIContent("Pixel Layout", "Select the pixel layout.");

            public static GUIContent pixelLayoutOffsetSquareText =
                new GUIContent("Offset", "Sets the vertical offset of even column pixels.");

            public static GUIContent pixelLayoutArrowText =
                new GUIContent("Shear", "Sets the shear offset of pixels.");

            public static GUIContent pixelLayoutTriangularText =
                new GUIContent("Triangle Aspect", "Sets the aspect ratio of triangle pixel. The aspect ratio is the height of the isosceles triangle divided by the half of the bottom side length.");
            
            public static GUIContent colorWashoutText =
                new GUIContent("Color Washout", "Select the display color washout mode.");

            public static readonly string[] pixelLayoutName = { "Square", "Offset Square", "Arrow", "Triangular" };

            public static readonly string[] colorWashoutModeName = { "None", "Simple", "IPS", "VA", "TN" };
        }

        public struct LCDDisplayProperties
        {
            public MaterialProperty pixelMask;
            public MaterialProperty pixelLuma;
            public MaterialProperty pixelLayout;
            public MaterialProperty pixelLayoutOffset;
            public MaterialProperty colorWashout;

            public LCDDisplayProperties(MaterialProperty[] properties)
            {
                pixelMask = BaseShaderGUI.FindProperty("_PixelMask", properties, false);
                pixelLuma = BaseShaderGUI.FindProperty("_PixelLuma", properties, false);
                pixelLayout = BaseShaderGUI.FindProperty("_PixelLayout", properties, false);
                pixelLayoutOffset = BaseShaderGUI.FindProperty("_PixelLayoutOffset", properties, false);
                colorWashout = BaseShaderGUI.FindProperty("_ColorWashout", properties, false);
            }
        }

        public static void DoLCDDisplay(LCDDisplayProperties properties, MaterialEditor materialEditor)
        {
            materialEditor.TexturePropertySingleLine(Styles.pixelMaskText, properties.pixelMask, properties.pixelLuma);

            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            var layout = (int)properties.pixelLayout.floatValue;
            layout = EditorGUILayout.Popup(Styles.pixelLayoutText, layout, Styles.pixelLayoutName);
            if (EditorGUI.EndChangeCheck())
                properties.pixelLayout.floatValue = layout;

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            var offset = properties.pixelLayoutOffset.floatValue;

            if ((PixelLayoutMode)layout == PixelLayoutMode.OffsetSquare)
            {
                offset = EditorGUILayout.Slider(Styles.pixelLayoutOffsetSquareText, offset, -1.0f, 1.0f);
            }
            else if ((PixelLayoutMode)layout == PixelLayoutMode.Arrow)
            {
                offset = EditorGUILayout.Slider(Styles.pixelLayoutArrowText, offset, -2.0f, 2.0f);
            }
            else if ((PixelLayoutMode)layout == PixelLayoutMode.Triangular)
            {
                offset = EditorGUILayout.Slider(Styles.pixelLayoutTriangularText, offset, 1.0f, 3.0f);
            }

            if (EditorGUI.EndChangeCheck())
                properties.pixelLayoutOffset.floatValue = offset;
            
            EditorGUI.indentLevel--;

            if (properties.colorWashout != null)
            {
                DoColorWashout(properties, materialEditor);
            }

            EditorGUI.indentLevel--;
        }

        public static void DoColorWashout(LCDDisplayProperties properties, MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            var colorWashoutMode = (int)properties.colorWashout.floatValue;
            colorWashoutMode = EditorGUILayout.Popup(Styles.colorWashoutText, colorWashoutMode, Styles.colorWashoutModeName);
            if (EditorGUI.EndChangeCheck())
                properties.colorWashout.floatValue = colorWashoutMode;
        }

        public static void SetMaterialKeywords(Material material)
        {
            if (material.HasProperty("_PixelLayout"))
            {
                PixelLayoutMode layoutMode = (PixelLayoutMode)material.GetFloat("_PixelLayout");

                CoreUtils.SetKeyword(material, "_PIXELLAYOUT_SQUARE", layoutMode == PixelLayoutMode.Square);
                CoreUtils.SetKeyword(material, "_PIXELLAYOUT_OFFSET_SQUARE", layoutMode == PixelLayoutMode.OffsetSquare);
                CoreUtils.SetKeyword(material, "_PIXELLAYOUT_ARROW", layoutMode == PixelLayoutMode.Arrow);
                CoreUtils.SetKeyword(material, "_PIXELLAYOUT_TRIANGULAR", layoutMode == PixelLayoutMode.Triangular);
            }

            if (material.HasProperty("_ColorWashout"))
            {
                ColorWashoutMode colorWashoutMode = (ColorWashoutMode)material.GetFloat("_ColorWashout");

                CoreUtils.SetKeyword(material, "_COLORWASHOUT_NONE", colorWashoutMode == ColorWashoutMode.None);
                CoreUtils.SetKeyword(material, "_COLORWASHOUT_SIMPLE", colorWashoutMode == ColorWashoutMode.Simple);
                CoreUtils.SetKeyword(material, "_COLORWASHOUT_IPS", colorWashoutMode == ColorWashoutMode.IPS);
                CoreUtils.SetKeyword(material, "_COLORWASHOUT_VA", colorWashoutMode == ColorWashoutMode.VA);
                CoreUtils.SetKeyword(material, "_COLORWASHOUT_TN", colorWashoutMode == ColorWashoutMode.TN);
            }
        }
    }
}