using UnityEngine;
using DG.Tweening;
using System;

namespace AppCore
{
    public static class UnsortedExtensions
    {
        public static int ToInt(this bool val)
        {
            return val ? 1 : 0;
        }
        public static bool ToBool(this int val)
        {
            return val != 0;
        }

        public static bool HasFlag(this byte val, byte valToCheck)
        {
            return (val & valToCheck) == valToCheck;
        }

        public static bool HasFlag(this short val, short valToCheck)
        {
            return (val & valToCheck) == valToCheck;
        }

        public static bool HasFlag(this int val, int valToCheck)
        {
            return (val & valToCheck) == valToCheck;
        }

        public static bool HasFlag(this long val, long valToCheck)
        {
            return (val & valToCheck) == valToCheck;
        }

        public static T StringToEnum<T>(string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static int GetEnumElementsCount<T>()
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static int GetEnumElementsCount(this Enum enumValue)
        {
            return Enum.GetNames(enumValue.GetType()).Length;
        }
    }
}