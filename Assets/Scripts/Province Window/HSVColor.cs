using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigeon
{
    public struct HSVColor
    {
        public float h;
        public float s;
        public float v;

        public static implicit operator HSVColor(Color color)
        {
            //float max = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
            int maxChannel = color.r > color.g ? (color.r > color.b ? 0 : 2) : (color.g > color.b ? 1 : 2);
            float min = Mathf.Min(color.r, Mathf.Min(color.g, color.b));
            float max;
            float h;

            switch (maxChannel)
            {
                case 0:
                    max = color.r;
                    h = (color.g - color.b) / (max - min);
                    break;
                case 1:
                    max = color.g;
                    h = 2f + (color.b - color.r) / (max - min);
                    break;
                case 2:
                    max = color.b;
                    h = 4f + (color.r - color.g) / (max - min);
                    break;
                default:
                    max = 0f;
                    h = 0f;
                    break;
            }

            return new HSVColor(h, (max == 0) ? 0 : 1f - (1f * min / max), max);
        }

        public static implicit operator Color(HSVColor color)
        {
            return Color.HSVToRGB(color.h, color.s, color.v);
        }

        public HSVColor(float h, float s, float v)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }

        public HSVColor (Color color)
        {
            int maxChannel = color.r > color.g ? (color.r > color.b ? 0 : 2) : (color.g > color.b ? 1 : 2);
            float min = Mathf.Min(color.r, Mathf.Min(color.g, color.b));
            float max;

            switch (maxChannel)
            {
                case 0:
                    max = color.r;
                    h = (color.g - color.b) / (max - min);
                    break;
                case 1:
                    max = color.g;
                    h = 2f + (color.b - color.r) / (max - min);
                    break;
                case 2:
                    max = color.b;
                    h = 4f + (color.r - color.g) / (max - min);
                    break;
                default:
                    max = 0f;
                    h = 0f;
                    break;
            }

            s = (max == 0) ? 0 : 1f - (1f * min / max);
            //v = max / 255f;
            v = max;
        }

        public Color RGB()
        {
            return Color.HSVToRGB(h, s, v);

            //int hi = (int)Mathf.Floor(h / 60) % 6;
            //float f = h / 60 - Mathf.Floor(h / 60);
            //
            //float value = this.v * 255;
            //int v =(int)(value);
            //int p =(int)(value * (1 - s));
            //int q =(int)(value * (1 - f * s));
            //int t =(int)(value * (1 - (1 - f) * s));
            //
            //if (hi == 0)
            //    return Color.FromArgb(255, v, t, p);
            //else if (hi == 1)
            //    return Color.FromArgb(255, q, v, p);
            //else if (hi == 2)
            //    return Color.FromArgb(255, p, v, t);
            //else if (hi == 3)
            //    return Color.FromArgb(255, p, q, v);
            //else if (hi == 4)
            //    return Color.FromArgb(255, t, p, v);
            //else
            //    return Color.FromArgb(255, v, p, q);
        }
    }
}