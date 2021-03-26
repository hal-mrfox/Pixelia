﻿using System;
using System.Collections;
using UnityEngine;

namespace Pigeon
{
    public static class EaseFunctions
    {
        public delegate float EvaluateMode(float x);

        /// <summary>
        /// Exponential, Circle, and Elastic functions are slower because they use Pow and/or Sqrt. Exponential can be estimated with Quintic.
        /// </summary>
        public enum EaseMode
        {
            EaseInSin, EaseOutSin, EaseInOutSin, EaseInQuadratic, EaseOutQuadratic, EaseInOutQuadratic, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseInQuartic,
            EaseOutQuartic, EaseInOutQuartic, EaseInQuintic, EaseOutQuintic, EaseInOutQuintic, EaseInExponential, EaseOutExponential, EaseInOutExponential,
            EaseInCircle, EaseOutCircle, EaseInOutCircle, EaseInBack, EaseOutBack, EaseInOutBack, EaseInElastic, EaseOutElastic, EaseInOutElastic,
            EaseInBounce, EaseOutBounce, EaseInOutBounce, AnimationCurve
        }

        public static EvaluateMode SetEaseMode(EaseMode mode)
        {
            return mode switch
            {
                EaseMode.EaseInSin => EaseInSin,
                EaseMode.EaseOutSin => EaseOutSin,
                EaseMode.EaseInOutSin => EaseInOutSin,
                EaseMode.EaseInQuadratic => EaseInQuadratic,
                EaseMode.EaseOutQuadratic => EaseOutQuadratic,
                EaseMode.EaseInOutQuadratic => EaseInOutQuadratic,
                EaseMode.EaseInCubic => EaseInCubic,
                EaseMode.EaseOutCubic => EaseOutCubic,
                EaseMode.EaseInOutCubic => EaseInOutCubic,
                EaseMode.EaseInQuartic => EaseInQuartic,
                EaseMode.EaseOutQuartic => EaseOutQuartic,
                EaseMode.EaseInOutQuartic => EaseInOutQuartic,
                EaseMode.EaseInQuintic => EaseInQuintic,
                EaseMode.EaseOutQuintic => EaseOutQuintic,
                EaseMode.EaseInOutQuintic => EaseInOutQuintic,
                EaseMode.EaseInExponential => EaseInExponential,
                EaseMode.EaseOutExponential => EaseOutExponential,
                EaseMode.EaseInOutExponential => EaseInOutExponential,
                EaseMode.EaseInCircle => EaseInCircle,
                EaseMode.EaseOutCircle => EaseOutCircle,
                EaseMode.EaseInOutCircle => EaseInOutCircle,
                EaseMode.EaseInBack => EaseInBack,
                EaseMode.EaseOutBack => EaseOutBack,
                EaseMode.EaseInOutBack => EaseInOutBack,
                EaseMode.EaseInElastic => EaseInElastic,
                EaseMode.EaseOutElastic => EaseOutElastic,
                EaseMode.EaseInOutElastic => EaseInOutElastic,
                EaseMode.EaseInBounce => EaseInBounce,
                EaseMode.EaseOutBounce => EaseOutBounce,
                EaseMode.EaseInOutBounce => EaseInOutBounce,
                EaseMode.AnimationCurve => throw new Exception("I didn't feel like adding this."),
                _ => EaseOutQuintic,
            };
        }

        public static IEnumerator LerpValueOverTime(Action<float> OnValueChanged, float value, float targetValue, float speed, EvaluateMode EvaluateMode)
        {
            float initValue = value;
            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime * speed;
                if (time > 1f)
                {
                    time = 1f;
                }
                value = Mathf.LerpUnclamped(initValue, targetValue, EvaluateMode(time));
                OnValueChanged?.Invoke(value);
                yield return null;
            }
        }

        public static float EaseInSin(float x)
        {
            return 1f - Mathf.Cos(x * Mathf.PI / 2f);
        }

        public static float EaseOutSin(float x)
        {
            return 1f - Mathf.Sin(x * Mathf.PI / 2f);
        }

        public static float EaseInOutSin(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }

        public static float EaseInQuadratic(float x)
        {
            return x * x;
        }

        public static float EaseOutQuadratic(float x)
        {
            return 1f - ((1f - x) * (1f - x));
        }

        public static float EaseInOutQuadratic(float x)
        {
            return x < 0.5f ? 2f * x * x : 1f - (-2f * x + 2f) * (-2f * x + 2f) / 2f;
        }

        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        public static float EaseOutCubic(float x)
        {
            return 1f - ((1f - x) * (1f - x) * (1f - x));
        }

        public static float EaseInOutCubic(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) / 2f;
        }

        public static float EaseInQuartic(float x)
        {
            return x * x * x * x;
        }

        public static float EaseOutQuartic(float x)
        {
            return 1f - ((1f - x) * (1f - x) * (1f - x) * (1f - x));
        }

        public static float EaseInOutQuartic(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1f - (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) / 2f;
        }

        public static float EaseInQuintic(float x)
        {
            return x * x * x * x * x;
        }

        public static float EaseOutQuintic(float x)
        {
            return 1f - ((1f - x) * (1f - x) * (1f - x) * (1f - x) * (1f - x));
        }

        public static float EaseInOutQuintic(float x)
        {
            return x < 0.5f ? 16f * x * x * x * x * x : 1f - (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) * (-2f * x + 2f) / 2f;
        }

        public static float EaseInExponential(float x)
        {
            return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        }

        public static float EaseOutExponential(float x)
        {
            return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }

        public static float EaseInOutExponential(float x)
        {
            return x == 0f ? 0f : x == 1f ? 1f : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
        }

        public static float EaseInCircle(float x)
        {
            return 1f - Mathf.Sqrt(1f - (x * x));
        }

        public static float EaseOutCircle(float x)
        {
            return Mathf.Sqrt(1f - ((x - 1f) * (x - 1f)));
        }

        public static float EaseInOutCircle(float x)
        {
            return x < 0.5f ? (1f - Mathf.Sqrt(1f - (2f * x * 2f * x))) / 2f : (Mathf.Sqrt(1f - ((-2f * x + 2f) * (-2f * x + 2f))) + 1f) / 2f;
        }

        public static float EaseInBack(float x)
        {
            return 2.70158f * x * x * x - 1.70158f * x * x;
        }

        public static float EaseOutBack(float x)
        {
            return 1f + 2.70158f * ((x - 1f) * (x - 1f) * (x - 1f)) + 1.70158f * ((x - 1f) * (x - 1f));
        }

        public static float EaseInOutBack(float x)
        {
            return x < 0.5f ? (2f * x) * (2f * x) * ((2.59491f + 1f) * 2f * x - 2.59491f) / 2f : ((2f * x - 2f) * (2f * x - 2f) * ((2.59491f + 1f) * (x * 2f - 2f) + 2.59491f) + 2f) / 2f;
        }

        public static float EaseInElastic(float x)
        {
            return x == 0f ? 0f : x == 1f ? 1f : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * 2.09439f);
        }

        public static float EaseOutElastic(float x)
        {
            return x == 0f ? 0f : x >= 1f ? 1f : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * (2f * Mathf.PI / 3f)) + 1f;
        }

        public static float EaseInOutElastic(float x)
        {
            return x == 0f ? 0f : x >= 1f ? 1f : x < 0.5f ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * (2f * Mathf.PI) / 4.5f)) / 2f
                : Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * (2f * Mathf.PI) / 4.5f) / 2f + 1f;
        }

        public static float EaseInBounce(float x)
        {
            return 1f - EaseOutBounce(1f - x);
        }

        public static float EaseOutBounce(float x)
        {
            if (x < 1f / 2.75f)
            {
                return 7.5625f * x * x;
            }
            else if (x < 2f / 2.75f)
            {
                return 7.5625f * (x -= 1.5f / 2.75f) * x + 0.75f;
            }
            else if (x < 2.5f / 2.75f)
            {
                return 7.5625f * (x -= 2.25f / 2.75f) * x + 0.9375f;
            }
            else
            {
                return 7.5625f * (x -= 2.625f / 2.75f) * x + 0.984375f;
            }
        }

        public static float EaseInOutBounce(float x)
        {
            return x < 0.5f ? (1f - EaseOutBounce(1f - 2f * x)) / 2f : (1f + EaseOutBounce(2f * x - 1f)) / 2f;
        }
    }
}