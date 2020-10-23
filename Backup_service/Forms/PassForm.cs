using System;
using System.Windows.Forms;

namespace Backup_service
{
    public partial class PassForm : Form
    {
        IniFiles INI = new IniFiles("config.ini");
        public PassForm()
        {
            InitializeComponent();
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (INI.ReadINI("MainSettings", "P") == "")
            {
                MessageBox.Show("Файл настроек повреждён!", "Ошибка", MessageBoxButtons.OK);
                Program.CloseAllWindows();
            }
            if (EncryptDecrypt.GetHashString(PasswordText.Text + "Шифр") == INI.ReadINI("MainSettings", "P"))
            {
                label2.Text = "Подключение к файловому хранилищу...";
                label2.Refresh();
                Form ifrm = new MainForm(PasswordText.Text);
                ifrm.Show();
                this.Hide();
            }
            else
            {
                PasswordText.Text = "";
                MessageBox.Show("Введён неверный пароль!", "Предупреждение", MessageBoxButtons.OK);
            }
        }

        private void PasswordText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(sender, e);
            }
        }
        private void PassForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form ifrm = new MainForm(); //при закрытии формы, закрывается главная форма
            ifrm.Close();
        }

    }
}
