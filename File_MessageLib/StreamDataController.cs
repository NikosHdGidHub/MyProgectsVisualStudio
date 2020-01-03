using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Lib.StreamDataController
{
	public interface IStreamDataController<T>
	{
		/// <summary>
		/// Событие отправляет строку с сообщением: Читаемый файл не был найден по пути.
		/// </summary>
		event System.EventHandler<string> ReadFileNotFined;
		/// <summary>
		/// Событие отправляет строку с сообщением ошибки.
		/// </summary>
		event System.EventHandler<string> ErrorEvent;
		/// <summary>
		/// Полный путь к файлу
		/// </summary>
		T FullName { get; set; }
		/// <summary>
		/// Создает или перезаписывает файл по указанному пути.
		/// </summary>
		/// <param name="path"> Путь, записывающийся в свойство FullName</param>
		void Create(T path);
		/// <summary>
		/// Создает или перезаписывает файл по свойству FullName.
		/// </summary>
		void Create();
		/// <summary>
		/// Определяет существование файла по пути FullName.
		/// </summary>
		/// <returns>Правда, если есть файл</returns>
		bool IsExist();
		/// <summary>
		/// Получает весь текст из файла.
		/// </summary>
		/// <param name="encoding">Кодировка.</param>
		/// <returns>Текст из файла.</returns>
		string LoadTextContent(Encoding encoding);
		/// <summary>
		/// Получает весь текст из файла.
		/// </summary>
		/// <returns>Текст из файла.</returns>
		string LoadTextContent();
		/// <summary>
		/// Сохраняет текст в файл.
		/// </summary>
		/// <param name="content">Текст.</param>
		/// <param name="encoding">Кодировка.</param>
		void SaveTextContent(string content, Encoding encoding);
		/// <summary>
		/// Сохраняет текст в файл.
		/// </summary>
		/// <param name="content">Текст.</param>
		void SaveTextContent(string content);
		/// <summary>
		/// Стериализует объект в файл.
		/// </summary>
		/// <param name="obj">Объект.</param>
		void SaveBinaryFormatter(object obj);
		/// <summary>
		/// Дестериализует объект из файла.
		/// </summary>
		/// <returns>Объект.</returns>
		object LoadBinaryFormatter();
	}
	public class FileController : IStreamDataController<string>
	{
		private readonly Encoding _defaultEncoding = Encoding.UTF8;


		#region IFileMeneger
		public event System.EventHandler<string> ReadFileNotFined;
		public event System.EventHandler<string> ErrorEvent;

		public string FullName { get; set; } = null;

		public void Create(string path)
		{
			FullName = path;
			Create();
		}
		public void Create()
		{
			try
			{
				using (FileStream stream = File.Create(FullName))
				{
					stream.Close();
				}
			}
			catch (System.Exception ex)
			{
				ErrorEvent?.Invoke(this, ex.Message);
				System.Console.WriteLine(ex.Message);
			}
		}
		public bool IsExist()
		{
			return File.Exists(FullName);
		}
		public string LoadTextContent()
		{
			return LoadTextContent(_defaultEncoding);
		}
		public string LoadTextContent(Encoding encoding)
		{
			try
			{
				if (IsExist())
				{
					return File.ReadAllText(FullName, encoding);
				}
				else
				{
					ReadFileNotFined?.Invoke(this, "Файл, который вы собирались прочитать, не найден по пути: " + FullName);
					return null;
				}
			}
			catch (System.Exception ex)
			{
				ErrorEvent?.Invoke(this, ex.Message);
				System.Console.WriteLine(ex.Message);
				return null;
			}
		}

		public void SaveTextContent(string content)
		{
			SaveTextContent(content, _defaultEncoding);
		}
		public void SaveTextContent(string content, Encoding encoding)
		{
			try
			{
				File.WriteAllText(FullName, content, encoding);
			}
			catch (System.Exception ex)
			{
				ErrorEvent?.Invoke(this, ex.Message);
				System.Console.WriteLine(ex.Message);
			}
		}

		public void SaveBinaryFormatter(object obj)
		{
			if (obj == null)
			{
				ErrorEvent?.Invoke(this, "Передаваемый объект является NULL");
				return;
			}
			try
			{
				// создаем объект BinaryFormatter
				var formatter = new BinaryFormatter();
				using (FileStream fs = new FileStream(FullName, FileMode.OpenOrCreate))
				{
					formatter.Serialize(fs, obj);
				}
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ErrorEvent?.Invoke(this, ex.Message);
			}
		}

		public object LoadBinaryFormatter()
		{
			try
			{
				var formatter = new BinaryFormatter();
				using (FileStream fs = new FileStream(FullName, FileMode.OpenOrCreate))
				{
					if (fs.Length > 0)
						return formatter.Deserialize(fs);
				}
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ErrorEvent?.Invoke(this, ex.Message);
			}
			return null;

		}
		#endregion
	}
}
