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
            if (comboBox1.SelectedIndex == 0)
            {
                MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS);
            }
            if (comboBox1.SelectedIndex == 1)
            {
                MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2);
            }
            if (comboBox1.SelectedIndex == 2)
            {
                MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3);
            }
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            textBox1.Visible = true;
            button3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                if (textBox1.Text == "") 
                    MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS);
                if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, textBox1.Text + '/');
                else MainForm.UploadFile(MainForm.DOMAIN, MainForm.USER, MainForm.PASS, '/' + textBox1.Text + '/');
            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (textBox1.Text == "") 
                    MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2);
                if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, textBox1.Text + '/');
                else MainForm.UploadFile(MainForm.DOMAIN2, MainForm.USER2, MainForm.PASS2, '/' + textBox1.Text + '/');
            }
            if (comboBox1.SelectedIndex == 2)
            {
                if (textBox1.Text == "") 
                    MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3);
                if (textBox1.Text[0] == '/') MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, textBox1.Text + '/');
                else MainForm.UploadFile(MainForm.DOMAIN3, MainForm.USER3, MainForm.PASS3, '/' + textBox1.Text + '/');
            }
        }


    }
}
