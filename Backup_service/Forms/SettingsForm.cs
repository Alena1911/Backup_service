using System;
using System.Windows.Forms;

namespace Backup_service.Forms
{
    public partial class SettingsForm : Form
    {
        IniFiles INI = new IniFiles("config.ini");
        public SettingsForm()
        {
            InitializeComponent();
            textBoxDomain.Text = MainForm.DOMAIN;
            textBoxUser.Text = MainForm.USER;
            textBoxPass.Text = MainForm.PASS;
            textBoxDir.Text = MainForm.DIR;
        }

        private void changePassBtn_Click(object sender, EventArgs e)
        {
            if (INI.ReadINI("MainSettings", "P") == "")
            {
                MessageBox.Show("Файл настроек повреждён!", "Ошибка", MessageBoxButtons.OK);
            }
            else if (EncryptDecrypt.GetHashString(oldPass.Text + "Шифр") == INI.ReadINI("MainSettings","P") && newPass.Text==newPass2.Text)
            {
                INI.Write("MainSettings", "P", EncryptDecrypt.GetHashString(newPass.Text + "Шифр"));
                INI.Write("MainSettings", "DOMAIN", EncryptDecrypt.Shifrovka(MainForm.DOMAIN, newPass.Text));
                INI.Write("MainSettings", "USER", EncryptDecrypt.Shifrovka(MainForm.USER, newPass.Text));
                INI.Write("MainSettings", "PASS", EncryptDecrypt.Shifrovka(MainForm.PASS, newPass.Text));
                MessageBox.Show("Пароль успешно изменён!", "Предупреждение", MessageBoxButtons.OK);
            }
            else if(EncryptDecrypt.GetHashString(oldPass.Text + "Шифр") != INI.ReadINI("MainSettings", "P"))
            {
                MessageBox.Show("Неверно введён старый пароль", "Ошибка", MessageBoxButtons.OK);
            }
            else if (newPass.Text != newPass2.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MainForm.DOMAIN != textBoxDomain.Text)
            {
                INI.Write("MainSettings", "DOMAIN", EncryptDecrypt.Shifrovka(textBoxDomain.Text, MainForm.COMMONPASS));
                MainForm.DOMAIN = textBoxDomain.Text;
            }
            if (MainForm.USER != textBoxUser.Text)
            {
                INI.Write("MainSettings", "USER", EncryptDecrypt.Shifrovka(textBoxUser.Text, MainForm.COMMONPASS));
                MainForm.USER = textBoxUser.Text;
            }
            if (MainForm.PASS != textBoxPass.Text)
            {
                INI.Write("MainSettings", "PASS", EncryptDecrypt.Shifrovka(textBoxPass.Text, MainForm.COMMONPASS));
                MainForm.PASS = textBoxPass.Text;
            }
            if (MainForm.DIR != textBoxDir.Text)
            {
                INI.Write("MainSettings", "DIR", EncryptDecrypt.Shifrovka(textBoxDir.Text, MainForm.COMMONPASS));
                MainForm.DIR = textBoxDir.Text;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBoxDir.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
