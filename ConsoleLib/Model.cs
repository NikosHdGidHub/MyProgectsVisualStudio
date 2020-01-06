using System;
using System.Collections.Generic;

namespace Lib.ConsoleLib
{

	public struct DialogVariants
	{
		public string Message { get; set; }
		public Action Action { get; set; }
	}
	public interface IConsoleModel: IDisposable
	{
		/// <summary>
		/// Запрашивает ввод с консоли у пользователя, и переводит строку в число
		/// </summary>
		/// <typeparam name="T">Тип числа</typeparam>
		/// <param name="resultNum">Переведенное число</param>
		/// <param name="errorAction">Метод срабатывает, когда пользователь ввел не число</param>
		/// <param name="cycle">Повторный запрос ввода у пользователя</param>
		/// <returns>Ложь - если, cycle - Ложь, и пользователь ввел не число</returns>
		bool ConsoleReadLineParse<T>(out T resultNum, Action errorAction = null, bool cycle = true);

		int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants);
		int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants, Action<int> BefoteAction);

		/// <summary>
		/// Возвращает индекс нажатого символа или группы символов "(abc)". Если -1, то был нажат другой символ
		/// </summary>
		/// <param name="title">Первая строка. Заголовок</param>
		/// <param name="symbols">Символы разделители.Пример ( - ) : 1 - Вариант 1/r/n2 - Вариант 2</param>
		/// <param name="keysRule">Правила нажатых символов</param>
		/// <param name="dialogVariants">Варианты диалога</param>
		/// <param name="BefoteAction">Метод, выполняемый до вызова метода диалога</param>
		/// <param name="charArr">Массив проверяемых символов</param>
		/// <param name="charPressed">Нажатый символ</param>
		/// <param name="cycle">Цикл пока не нажат один из требуемых символов</param>
		/// <returns>индекс нажатого символа. Если -1, то был нажат другой символ</returns>
		int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants, Action<int> BefoteAction, out char charPressed, bool cycle, string symbols);
		/// <summary>
		/// Возвращает индекс нажатого символа или группы символов "(abc)". Если -1, то был нажат другой символ
		/// </summary>
		/// <param name="charArr">Масив проверяемых символов</param>
		/// <param name="charPressed">Нажатый символ</param>
		/// <param name="cycle">Цикл пока не нажат один из требуемых символов</param>
		/// <returns>индекс нажатого символа. Если -1, то был нажат другой символ</returns>
		int CheckCharInput(string charArr, out char charPressed, bool cycle = true);
		/// <summary>
		/// Вызывается в конце программы
		/// </summary>
		void EndProgramm();
		event Action<string> PrintMessage;
	}
	public sealed class ConsoleModel : IConsoleModel
	{
		#region Один экземпляр
		/// <summary>
		/// Хранит ссылку на единственный экземпляр
		/// </summary>
		private static ConsoleModel Instance = null;
		/// <summary>
		/// Создает или возвращает ссылку на единственный экземпляр.
		/// </summary>
		/// <param name="name">Имя вида.</param>
		/// <returns>Ссылку на единственный экземпляр.</returns>
		public static IConsoleModel GetView(string name = null)
		{
			return Instance ?? (Instance = new ConsoleModel());
		}
		
		#endregion
		private char[] GetArrayChar(in string charArr)
		{
			bool reedFlag = true;
			List<char> arr = new List<char>();
			int strLen = charArr.Length;
			for (int i = 0; i < strLen; i++)
			{
				if (charArr[i] == '(')
				{
					if (!reedFlag)
					{
						throw new ArithmeticException("Встретилась '(' до ')' : " + i);
					}

					if (++i >= strLen)
					{
						throw new ArithmeticException("Встретилась '(' после которой ничего нет");
					}

					arr.Add(charArr[i]);
					reedFlag = false;
					continue;
				}
				if (charArr[i] == ')')
				{
					if (reedFlag)
					{
						throw new ArithmeticException("Встретилась ')' до '('\nИндекс : " + i);
					}

					reedFlag = true;
					continue;
				}
				if (reedFlag)
				{
					arr.Add(charArr[i]);
				}
			}
			return arr.ToArray();
		}
		private bool CheckChars(char key, in string charArr, out int index)
		{
			bool flag = false;
			int j = 0;
			for (int i = 0; i < charArr.Length; i++)
			{
				if (charArr[i] == '(')
				{
					if (flag)
					{
						throw new ArithmeticException("Встретилась '(' до ')' : " + i);
					}

					flag = true;
					continue;
				}
				if (charArr[i] == ')')
				{
					if (!flag)
					{
						throw new ArithmeticException("Встретилась ')' до '('\nИндекс : " + i);
					}

					flag = false;
					j++;
					continue;
				}

				if (key == charArr[i])
				{
					index = j;
					return true;
				}
				if (!flag)
				{
					j++;
				}
			}
			index = -1;
			return false;
		}


		#region IConsoleModel
		public int CheckCharInput(string charArr, out char charPressed, bool cycle = true)
		{
			int index = -1;
			do
			{
				char key = Console.ReadKey().KeyChar;
				Console.CursorLeft = 0;
				Console.Write(" ");
				Console.CursorLeft = 0;
				charPressed = key;
				if (charArr != null && CheckChars(key, charArr, out index))
				{
					break;
				}
			}
			while (cycle);
			return index;
		}
		public void EndProgramm()
		{
			PrintMessage?.Invoke("Конец работы программы");
			Console.ReadKey();
		}

		public int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants, Action<int> BefoteAction, out char charPressed, bool cycle, string symbols)
		{
			if (keysRule == null)
			{
				throw new Exception("Keys is null");
			}

			if (dialogVariants == null)
			{
				throw new Exception("dialogVariants is null");
			}

			char[] charArr = GetArrayChar(keysRule);
			if (dialogVariants.Length != charArr.Length)
			{
				throw new Exception("Количество символов charArr не совпадает с dialogVariants");
			}

			PrintMessage?.Invoke(title);

			for (int i = 0; i < charArr.Length; i++)
			{
				PrintMessage?.Invoke(charArr[i] + symbols + dialogVariants[i].Message);
			}
			int index = CheckCharInput(keysRule, out charPressed, cycle);

			BefoteAction?.Invoke(index);

			if (index == -1)
			{
				return index;
			}

			dialogVariants[index].Action?.Invoke();
			return index;
		}
		public int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants)
		{
			return PrintDialogCharKey(keysRule, title, dialogVariants, null);
		}
		public int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants, Action<int> BefoteAction)
		{
			char a = 's';
			return PrintDialogCharKey(keysRule, title, dialogVariants, BefoteAction, out a, true, ") ");
		}
		
		public bool ConsoleReadLineParse<T>(out T resultNum, Action errorAction = null, bool cycle = true)
		{
			Type type = default(T).GetType();
			object result = null;
		CheckPoint:
			string dataStr = Console.ReadLine();
			if (type == typeof(byte) && byte.TryParse(dataStr, out byte res)) result = res;
			else if (type == typeof(sbyte) && sbyte.TryParse(dataStr, out sbyte res0)) result = res0;
			else if (type == typeof(short) && short.TryParse(dataStr, out short res1)) result = res1;
			else if (type == typeof(int) && int.TryParse(dataStr, out int res2)) result = res2;
			else if (type == typeof(long) && long.TryParse(dataStr, out long res3)) result = res3;
			else if (type == typeof(ushort) && ushort.TryParse(dataStr, out ushort res4)) result = res4;
			else if (type == typeof(uint) && uint.TryParse(dataStr, out uint res5)) result = res5;
			else if (type == typeof(ulong) && ulong.TryParse(dataStr, out ulong res6)) result = res6;
			else if (type == typeof(float) && float.TryParse(dataStr, out float res7)) result = res7;
			else if (type == typeof(double) && double.TryParse(dataStr, out double res8)) result = res8;
			else if (type == typeof(decimal) && decimal.TryParse(dataStr, out decimal res9)) result = res9;
			else
			{
				errorAction?.Invoke();
				if (cycle) goto CheckPoint;

				resultNum = default;
				return false;
			}
			resultNum = (T)result;
			return true;
		}


		public event Action<string> PrintMessage;
		#endregion

		public void Dispose()
		{
			Instance = null;
		}
	}


}
