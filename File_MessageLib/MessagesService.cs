using System;
using System.Windows.Forms;

namespace Lib.MessageLib
{
	public interface IMessagesService
	{
		void ShowMessage(string mes);
		void ShowExlamation(string exclamation);
		void ShowError(string error);
	}
	public class MessagesServiceForm : IMessagesService
	{
		public void ShowMessage(string mes)
		{
			MessageBox.Show(mes, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		public void ShowExlamation(string Exclamation)
		{
			MessageBox.Show(Exclamation, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		public void ShowError(string error)
		{
			MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	public class MessageServiceSimpleConsole : IMessagesService
	{
		private const string BEFORE_MES = " > ";
		private const string BEFORE_EXL = " ВНИМАНИЕ! > ";
		private const string BEFORE_ERROR = " ОШИБКА!!!:\r\n > ";
		public void ShowMessage(string mes)
		{
			Console.WriteLine(BEFORE_MES + mes);
		}
		public void ShowExlamation(string exclamation)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(BEFORE_EXL + exclamation);
			Console.ResetColor();
		}
		public void ShowError(string error)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(BEFORE_ERROR + error);
			Console.ResetColor();
		}
	}
}
