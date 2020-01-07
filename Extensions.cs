using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lib.Extensions
{
    public static class Helper
    {
        /// <summary>
        /// Переводит методом TryParse строку в число
        /// </summary>
        /// <typeparam name="T">Тип итогового числа</typeparam>
        /// <param name="input">Строка</param>
        /// <param name="resultNum">Итоговое число</param>
        /// <returns>Правда - если успешный перевод</returns>
        public static bool ParseTo<T>(this string input, out T resultNum)
        {
            var type = default(T).GetType();
            resultNum = default;
            if (input == null) return false;
            object result;
            if (type == typeof(byte) && byte.TryParse(input, out byte res)) result = res;
            else if (type == typeof(sbyte) && sbyte.TryParse(input, out sbyte res0)) result = res0;
            else if (type == typeof(short) && short.TryParse(input, out short res1)) result = res1;
            else if (type == typeof(int) && int.TryParse(input, out int res2)) result = res2;
            else if (type == typeof(long) && long.TryParse(input, out long res3)) result = res3;
            else if (type == typeof(ushort) && ushort.TryParse(input, out ushort res4)) result = res4;
            else if (type == typeof(uint) && uint.TryParse(input, out uint res5)) result = res5;
            else if (type == typeof(ulong) && ulong.TryParse(input, out ulong res6)) result = res6;
            else if (type == typeof(float) && float.TryParse(input, out float res7)) result = res7;
            else if (type == typeof(double) && double.TryParse(input, out double res8)) result = res8;
            else if (type == typeof(decimal) && decimal.TryParse(input, out decimal res9)) result = res9;
            else
            {
                return false;
            }
            resultNum = (T)result;
            return true;
        }


        /// <summary>
        /// Checked Range
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">Min - inclusive</param>
        /// <param name="max">Max - not inclusive</param>
        /// <returns></returns>
        public static bool InRange(this int num, int min, int max)
        {
            return num >= min && num < max;
        }
        public static int GetIndex(this string[] arr, string key)
        {
            for (int i = 0; i < arr?.Length; i++)
            {
                if (arr[i] == key)
                {
                    return i;
                }
            }
            return -1;
        }


        public static T[] CloneAll<T>(this T[] array)
        {
            var newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            return newArray;
        }
        public static List<T> CloneAll<T>(this List<T> array)
        {
            var newArray = new List<T>();
            for (int i = 0; i < array.Count; i++)
            {
                newArray.Add(array[i]);
            }
            return newArray;
        }
        public static T[] CloneAll<T>(this T[] array, Func<T, T> func)
        {
            var newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = func(array[i]);
            }
            return newArray;
        }

        /// <summary>
        /// У всех классов должен быть конструктор по умолчанию!!
        /// Не клонирует многомерные массивы глубоким Методом,
        /// Не клонирует Dictionary!!
        /// </summary>
        /// <param name="classObject"></param>
        /// <param name="clonePrivateFields">Копировать приватные поля</param>
        /// <returns>Скопированный object</returns>
        public static object CloneClass(this object baseClass, bool clonePrivateFields = false)
        {
            var metaBaseClass = baseClass.GetType();
            if (!metaBaseClass.IsClass) return baseClass;
            if (baseClass is string) return string.Copy(baseClass as string);
            if (metaBaseClass.IsArray)
            {
                var EType = metaBaseClass.GetElementType();

                if (!(baseClass is Array arr))
                    throw new ArgumentNullException(nameof(baseClass));

                var newArr = arr.Clone() as Array;
                if (EType.IsPrimitive || newArr.Rank > 1) return newArr;

                var len = newArr.Length;
                for (int i = 0; i < len; i++)
                {
                    newArr.SetValue(newArr.GetValue(i).CloneClass(clonePrivateFields), i);
                }
                return newArr as Array;
            }

            bool onlyPrivateFields = clonePrivateFields;
            if (metaBaseClass.IsGenericType) onlyPrivateFields = true;

            var constructor = metaBaseClass.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new FieldAccessException();

            var newClass = constructor.Invoke(new object[0]);


            var fields = metaBaseClass.GetFields(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.NonPublic);
            foreach (var prop in fields)
            {
                if (!onlyPrivateFields)
                {
                    var searchParticle = "k__BackingField";
                    var len = searchParticle.Length;
                    if (prop.Name.Length <= len) continue;

                    if (prop.Name.Remove(0, prop.Name.Length - len) != searchParticle)
                        continue;
                }

                var value = prop.GetValue(baseClass);
                if (value != null)
                {
                    value = value.CloneClass(clonePrivateFields);
                }

                prop.SetValue(newClass, value);
            }
            return newClass;
        }
    }
}
