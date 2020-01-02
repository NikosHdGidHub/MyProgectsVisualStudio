using System;
using System.Collections.Generic;

namespace Lib.ConsoleModel
{

	public struct DialogVariants
	{
		public string Message { get; set; }
		public Action Action { get; set; }
	}
	public interface IConsoleModel
	{
		/// <summary>
		/// Если цикл правда, то этот метод всегда вернет правду
		/// </summary>
		/// <param name="str"></param>
		/// <param name="result"></param>
		/// <param name="errorAction"></param>
		/// <param name="cycle"></param>
		/// <returns></returns>
		bool ConsoleReadLineParse(out int result, Action errorAction, bool cycle = true);
		bool ConsoleReadLineParse(out byte result, Action errorAction, bool cycle = true);
		bool ConsoleReadLineParse(out ushort result, Action errorAction, bool cycle = true);


		int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants);
		int PrintDialogCharKey(string keysRule, string title, DialogVariants[] dialogVariants, Action<int> BefoteAction);

		/// <summary>
		/// Возвращает индекс нажатого символа или группы символов "(abc)". Если -1, то был нажат другой символ
		/// </summary>
		/// <param name="charArr">Масив проверяемых символов</param>
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
	public class MainModel : IConsoleModel
	{
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

		public bool ConsoleReadLineParse(out int result, Action errorAction, bool cycle = true)
		{
			do
			{
				if (int.TryParse(Console.ReadLine(), out result))
				{
					return true;
				}
				else
				{
					errorAction();
				}
			} while (cycle);
			return false;
		}
		public bool ConsoleReadLineParse(out byte result, Action errorAction, bool cycle = true)
		{
			do
			{
				if (byte.TryParse(Console.ReadLine(), out result))
				{
					return true;
				}
				else
				{
					errorAction();
				}
			} while (cycle);
			return false;
		}
		public bool ConsoleReadLineParse(out ushort result, Action errorAction, bool cycle = true)
		{
			do
			{
				if (ushort.TryParse(Console.ReadLine(), out result))
				{
					return true;
				}
				else
				{
					errorAction();
				}
			} while (cycle);
			return false;
		}



		public event Action<string> PrintMessage;
		#endregion
	}
}
