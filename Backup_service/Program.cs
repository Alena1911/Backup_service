using System;
using System.Windows.Forms;

namespace Backup_service
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PassForm());
            
        }
        // процедура закрытия всех окон 
        public static void CloseAllWindows()
        {
            FormCollection ifrm = Application.OpenForms;  //Закрываем все окна программы
            while (ifrm.Count > 0)
            {
                ifrm[0].Close();
            }
        }

    }
}
