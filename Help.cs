using System;
using System.IO;
using System.Net;

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


