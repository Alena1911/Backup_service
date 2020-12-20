using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backup_service.Forms
{
    public partial class UploadForm : Form
    {
        static System.Threading.Thread thread = new System.Threading.Thread(delegate () { }); //создание пустого потока
        MainForm main;
        public UploadForm(MainForm f)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            main = f;
        }

        //загрузка в корень
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, openFileDialog.FileNames);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, openFileDialog.FileNames);
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, openFileDialog.FileNames);
                }
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            textBox1.Visible = true;
            button3.Visible = true;
        }

        //загрузка в папку
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    if (textBox1.Text == "")
                        UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, openFileDialog.FileNames);
                    else if (textBox1.Text[0] == '/') UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, openFileDialog.FileNames, textBox1.Text + '/');
                    else UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, openFileDialog.FileNames, '/' + textBox1.Text + '/');
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    if (textBox1.Text == "")
                        UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, openFileDialog.FileNames);
                    else if (textBox1.Text[0] == '/') UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, openFileDialog.FileNames, textBox1.Text + '/');
                    else UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, openFileDialog.FileNames, '/' + textBox1.Text + '/');
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    if (textBox1.Text == "")
                        UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, openFileDialog.FileNames);
                    else if (textBox1.Text[0] == '/') UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, openFileDialog.FileNames, textBox1.Text + '/');
                    else UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, openFileDialog.FileNames, '/' + textBox1.Text + '/');
                }
                Close();
            }   
            
        }

        public void UploadFile(string domain, string user, string pass, string[] filePaths, string FolderName = "/")
        {
            thread = (new System.Threading.Thread(delegate () {
                //отключаем кнопки
                main.button1.Invoke((MethodInvoker)(() => main.button1.Enabled = false));
                main.button2.Invoke((MethodInvoker)(() => main.button2.Enabled = false));
                main.button4.Invoke((MethodInvoker)(() => main.button4.Enabled = false));
                main.button5.Invoke((MethodInvoker)(() => main.button5.Enabled = false));
                main.progressBar1.Invoke((MethodInvoker)(() => main.progressBar1.Maximum = filePaths.Length));
                Ftp_Client ftp = new Ftp_Client();
                ftp.Host = domain;
                ftp.UserName = user;
                ftp.Password = pass;
                foreach (string filePath in filePaths)
                {
                    try
                    {
                        main.progressBar1.Invoke((MethodInvoker)(() => main.progressBar1.Value++));
                        main.UpdateInfoLabel(System.IO.Path.GetFileName(filePath));
                        ftp.UploadFile(FolderName, filePath);
                    }
                    catch (System.Net.WebException e)
                    {
                        try
                        {
                            if (FolderName == "/")
                            {
                                MessageBox.Show(e.Message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            else
                            {
                                FolderName = FolderName.Replace("/", "");
                                ftp.CreateDirectory("/", FolderName);
                                ftp.UploadFile('/' + FolderName + '/', filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                    }
                    Task.Delay(100).GetAwaiter().GetResult();
                }
                main.button1.Invoke((MethodInvoker)(() => main.button1.Enabled = true));
                main.button2.Invoke((MethodInvoker)(() => main.button2.Enabled = true));
                main.button4.Invoke((MethodInvoker)(() => main.button4.Enabled = true));
                main.button5.Invoke((MethodInvoker)(() => main.button5.Enabled = true));
                main.progressBar1.Invoke((MethodInvoker)(() => main.progressBar1.Value = 0));
                main.UpdateInfoLabel("");
                main.updateList();

            }));
            thread.IsBackground = true;
            thread.Start();
        }

    }
}
