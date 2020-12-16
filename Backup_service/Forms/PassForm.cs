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
                INI.Write("MainSettings", "P", EncryptDecrypt.Shifrovka("", ""));
                INI.Write("MainSettings", "DOMAIN", "");
                INI.Write("MainSettings", "USER", "");
                INI.Write("MainSettings", "PASS", "");
                INI.Write("MainSettings", "DOMAIN2", "");
                INI.Write("MainSettings", "USER2", "");
                INI.Write("MainSettings", "PASS2", "");
                INI.Write("MainSettings", "DOMAIN3", "");
                INI.Write("MainSettings", "USER3", "");
                INI.Write("MainSettings", "PASS3", "");
                MessageBox.Show("Файл настроек создан. Установите пароль в окне настроек");
                Form ifrm = new MainForm("");
                ifrm.Show();
                this.Hide();
            }
            else if (EncryptDecrypt.GetHashString(PasswordText.Text + "Шифр") == INI.ReadINI("MainSettings", "P"))
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
