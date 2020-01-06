using System;

namespace Lib.ConsoleLib
{
    public abstract class View : IDisposable
    {
        
        /// <summary>
        /// Имя вида.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пока правда цикл вида будет работать.
        /// </summary>
        public bool LoopFlag { get; private set; }
        /// <summary>
        /// Действие внутри цикла вида.
        /// </summary>
        public abstract void ActView();
        /// <summary>
        /// Старт, запуск вида
        /// </summary>
        public void Run()
        {
            Setup();
            Loop(ActView);
            After();
        }
        /// <summary>
        /// Используется для установки и настройки параметров
        /// </summary>
        protected virtual void Setup()
        {
            Console.WriteLine("View - " + Name + " is Started");
        }
        /// <summary>
        /// Цикл вида.
        /// </summary>
        /// <param name="act">Действие</param>
        private void Loop(Action act)
        {
            while (LoopFlag)
            {
                act();
            }
        }
        /// <summary>
        /// Используется для работы после выхода из цикла
        /// </summary>
        protected virtual void After()
        {
            Console.WriteLine("View - " + Name + " is Ended");
        }


        #region Dispose
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }
            disposed = true;
        }
        #endregion
    }
}