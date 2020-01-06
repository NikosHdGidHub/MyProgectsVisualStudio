using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace Lib
{

    public delegate void SuccessAjax(string resutl);

    public delegate void ErrorMessageMulti(string errorMessage, params string[] errorArr);
    public delegate void ErrorMessageSimple(string errorMessage);
    public static class Help
    {


        public static object DynamicIndexator(string keyInput, object obj)
        {
            var prop = obj.GetType().GetProperty(keyInput);
            return prop?.GetValue(obj) ?? null;
        }
        public static void DynamicIndexator(string keyInput, object obj, object value)
        {
            var prop = obj.GetType().GetProperty(keyInput);
            if (prop == null)
            {
                throw new Exception(keyInput + " - не существует");
            }
            if (value == null)
            {
                prop?.SetValue(obj, null);
                return;
            }
            if (prop.PropertyType != value.GetType())
            {
                throw new Exception(keyInput + " - Типы не совпадают");
            }
            prop?.SetValue(obj, value);
        }


        public static string Http(string url, string data, string metod = "POST")
        {
            string result = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = metod; // для отправки используется метод Post
                                        // данные для отправки
                                        // преобразуем данные в массив байтов
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);

                // устанавливаем тип содержимого - параметр ContentType
                //request.ContentType = "application/x-www-form-urlencoded";

                // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
                request.ContentLength = byteArray.Length;

                //записываем данные в поток запроса
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                response.Close();
            }
            catch (WebException ex)
            {
                // получаем статус исключения
                WebExceptionStatus status = ex.Status;

                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    Console.WriteLine("Статусный код ошибки: {0} - {1}   ",
                            (int)httpResponse.StatusCode, httpResponse.StatusCode);
                }
            }
            return result;
        }
        public static async void Ajax(string url, string data, SuccessAjax success, string metod = "POST")
        {
            string result = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = metod; // для отправки используется метод Post
                                        // данные для отправки
                                        // преобразуем данные в массив байтов
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
                // устанавливаем тип содержимого - параметр ContentType
                request.ContentType = "application/x-www-form-urlencoded";
                // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
                request.ContentLength = byteArray.Length;

                //записываем данные в поток запроса
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                        success(result);
                    }
                }
                response.Close();
            }
            catch (WebException ex)
            {
                // получаем статус исключения
                WebExceptionStatus status = ex.Status;

                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    Console.WriteLine("Статусный код ошибки: {0} - {1}",
                            (int)httpResponse.StatusCode, httpResponse.StatusCode);
                }
            }
        }
    }

}

namespace Lib.Extensions
{
    public static class Helper
    {
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
