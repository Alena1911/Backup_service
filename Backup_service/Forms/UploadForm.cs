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
        public UploadForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                if (comboBox1.SelectedIndex == 0)
                {
                    MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS,filePath);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, filePath);
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, filePath);
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

        private void button3_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                if (comboBox1.SelectedIndex == 0)
                {
                    if (textBox1.Text == "")
                        MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, filePath);
                    if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, filePath, textBox1.Text + '/');
                    else MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, filePath, '/' + textBox1.Text + '/');
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    if (textBox1.Text == "")
                        MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, filePath);
                    if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, filePath, textBox1.Text + '/');
                    else MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, filePath, '/' + textBox1.Text + '/');
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    if (textBox1.Text == "")
                        MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, filePath);
                    if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, filePath, textBox1.Text + '/');
                    else MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, filePath, '/' + textBox1.Text + '/');
                }
                Close();
            }   
            
        }


    }
}
