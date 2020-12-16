using System;
using System.Windows.Forms;

namespace Backup_service.Forms
{
    public partial class SettingsForm : Form
    {
        IniFiles INI = new IniFiles("config.ini");
        public static string tmpDOMAIN = MainForm.DOMAIN, tmpUSER = MainForm.USER, tmpPASS = MainForm.PASS;//объявление переменных, необходимых для работы с ftp сервером
        public static string tmpDOMAIN2 = MainForm.DOMAIN2, tmpUSER2 = MainForm.USER2, tmpPASS2 = MainForm.PASS2;
        public static string tmpDOMAIN3 = MainForm.USER3, tmpUSER3 = MainForm.USER3, tmpPASS3 = MainForm.PASS3;

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                tmpDOMAIN = textBoxDomain.Text;
                tmpUSER = textBoxUser.Text;
                tmpPASS = textBoxPass.Text;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                tmpDOMAIN2 = textBoxDomain.Text;
                tmpUSER2 = textBoxUser.Text;
                tmpPASS2 = textBoxPass.Text;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                tmpDOMAIN3 = textBoxDomain.Text;
                tmpUSER3 = textBoxUser.Text;
                tmpPASS3 = textBoxPass.Text;
            }
        }

        public SettingsForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            textBoxDomain.Text = MainForm.DOMAIN;
            textBoxUser.Text = MainForm.USER;
            textBoxPass.Text = MainForm.PASS;
            textBoxDir.Text = MainForm.DIR;
        }

        private void changePassBtn_Click(object sender, EventArgs e)
        {
            if (INI.ReadINI("MainSettings", "P") == "" && MainForm.DOMAIN == "" && MainForm.DOMAIN2 == "" && MainForm.DOMAIN3 == "" && newPass.Text == newPass2.Text)
            {
                INI.Write("MainSettings", "P", EncryptDecrypt.GetHashString(newPass.Text + "Шифр"));
            }
            else if (INI.ReadINI("MainSettings", "P") == "")
            {
                MessageBox.Show("Файл настроек повреждён!", "Ошибка", MessageBoxButtons.OK);
            }
            else if (newPass.Text != newPass2.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK);
            }
            else if (EncryptDecrypt.GetHashString(oldPass.Text + "Шифр") == INI.ReadINI("MainSettings","P") && newPass.Text==newPass2.Text)
            {
                INI.Write("MainSettings", "P", EncryptDecrypt.GetHashString(newPass.Text + "Шифр"));
                INI.Write("MainSettings", "DOMAIN", EncryptDecrypt.Shifrovka(MainForm.DOMAIN, newPass.Text));
                INI.Write("MainSettings", "USER", EncryptDecrypt.Shifrovka(MainForm.USER, newPass.Text));
                INI.Write("MainSettings", "PASS", EncryptDecrypt.Shifrovka(MainForm.PASS, newPass.Text));
                INI.Write("MainSettings", "DOMAIN2", EncryptDecrypt.Shifrovka(MainForm.DOMAIN2, newPass.Text));
                INI.Write("MainSettings", "USER2", EncryptDecrypt.Shifrovka(MainForm.USER2, newPass.Text));
                INI.Write("MainSettings", "PASS2", EncryptDecrypt.Shifrovka(MainForm.PASS2, newPass.Text));
                INI.Write("MainSettings", "DOMAIN3", EncryptDecrypt.Shifrovka(MainForm.DOMAIN3, newPass.Text));
                INI.Write("MainSettings", "USER3", EncryptDecrypt.Shifrovka(MainForm.USER3, newPass.Text));
                INI.Write("MainSettings", "PASS3", EncryptDecrypt.Shifrovka(MainForm.PASS3, newPass.Text));
                MessageBox.Show("Пароль успешно изменён!", "Предупреждение", MessageBoxButtons.OK);
            }
            else if(EncryptDecrypt.GetHashString(oldPass.Text + "Шифр") != INI.ReadINI("MainSettings", "P"))
            {
                MessageBox.Show("Неверно введён старый пароль", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                tmpDOMAIN = textBoxDomain.Text;
                tmpUSER = textBoxUser.Text;
                tmpPASS = textBoxPass.Text;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                tmpDOMAIN2 = textBoxDomain.Text;
                tmpUSER2 = textBoxUser.Text;
                tmpPASS2 = textBoxPass.Text;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                tmpDOMAIN3 = textBoxDomain.Text;
                tmpUSER3 = textBoxUser.Text;
                tmpPASS3 = textBoxPass.Text;
            }
            if (MainForm.DOMAIN != tmpDOMAIN)
            {
                INI.Write("MainSettings", "DOMAIN", EncryptDecrypt.Shifrovka(tmpDOMAIN, MainForm.COMMONPASS));
                MainForm.DOMAIN = tmpDOMAIN;
            }
            if (MainForm.USER != tmpUSER)
            {
                INI.Write("MainSettings", "USER", EncryptDecrypt.Shifrovka(tmpUSER, MainForm.COMMONPASS));
                MainForm.USER = tmpUSER;
            }
            if (MainForm.PASS != tmpPASS)
            {
                INI.Write("MainSettings", "PASS", EncryptDecrypt.Shifrovka(tmpPASS, MainForm.COMMONPASS));
                MainForm.PASS = tmpPASS;
            }
            if (MainForm.DOMAIN2 != tmpDOMAIN2)
            {
                INI.Write("MainSettings", "DOMAIN2", EncryptDecrypt.Shifrovka(tmpDOMAIN2, MainForm.COMMONPASS));
                MainForm.DOMAIN2 = tmpDOMAIN2;
            }
            if (MainForm.USER2 != tmpUSER2)
            {
                INI.Write("MainSettings", "USER2", EncryptDecrypt.Shifrovka(tmpUSER2, MainForm.COMMONPASS));
                MainForm.USER2 = tmpUSER2;
            }
            if (MainForm.PASS2 != tmpPASS2)
            {
                INI.Write("MainSettings", "PASS2", EncryptDecrypt.Shifrovka(tmpPASS2, MainForm.COMMONPASS));
                MainForm.PASS2 = tmpPASS2;
            }
            if (MainForm.DOMAIN3 != tmpDOMAIN3)
            {
                INI.Write("MainSettings", "DOMAIN3", EncryptDecrypt.Shifrovka(tmpDOMAIN3, MainForm.COMMONPASS));
                MainForm.DOMAIN3 = tmpDOMAIN3;
            }
            if (MainForm.USER3 != tmpUSER3)
            {
                INI.Write("MainSettings", "USER3", EncryptDecrypt.Shifrovka(tmpUSER3, MainForm.COMMONPASS));
                MainForm.USER3 = tmpUSER3;
            }
            if (MainForm.PASS3 != tmpPASS3)
            {
                INI.Write("MainSettings", "PASS3", EncryptDecrypt.Shifrovka(tmpPASS3, MainForm.COMMONPASS));
                MainForm.PASS3 = tmpPASS3;
            }

            if (MainForm.DIR != textBoxDir.Text)
            {
                INI.Write("MainSettings", "DIR", EncryptDecrypt.Shifrovka(textBoxDir.Text, MainForm.COMMONPASS));
                MainForm.DIR = textBoxDir.Text;
            }
            MessageBox.Show("Настройки сохранены!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBoxDir.Text = folderBrowserDialog1.SelectedPath;
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                textBoxDomain.Text = tmpDOMAIN;
                textBoxUser.Text = tmpUSER;
                textBoxPass.Text = tmpPASS;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBoxDomain.Text = tmpDOMAIN2;
                textBoxUser.Text = tmpUSER2;
                textBoxPass.Text = tmpPASS2;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                textBoxDomain.Text = tmpDOMAIN3;
                textBoxUser.Text = tmpUSER3;
                textBoxPass.Text = tmpPASS3;
            }

        }
    }
}
