using Backup_service.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Backup_service
{
    public partial class MainForm : Form
    {
        private IniFiles INI = new IniFiles("config.ini");// инициализация ini файла
        public List<string> forDownload = new List<string>();//список ссылок на загрузку
        public static string DOMAIN, USER, PASS, COMMONPASS, DIR;//объявление переменных, необходимых для работы с ftp сервером
        System.Threading.Thread thread = new System.Threading.Thread(delegate () { }); //создание пустого потока

        //принимает пароль, введённый пользователем на форме авторизации
        public MainForm(string pass = "")
        {
            InitializeComponent();
            if (pass != "")
            {
                try
                {
                    COMMONPASS = pass;
                    DOMAIN = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "DOMAIN"), COMMONPASS);
                    USER = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "USER"), COMMONPASS);
                    PASS = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "PASS"), COMMONPASS);
                    DIR = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "DIR"), COMMONPASS);
                }
                catch
                {
                    MessageBox.Show("Ошибка извлечения файла конфигурации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.CloseAllWindows();
                }

                //FileStruct[] FileList = ftp.ListDirectory("/WpfApp1/WpfApp1");
                ListDirectory(treeView1,DOMAIN,USER,PASS);
            }
        }


        
        //Построение дерева файловой системы ftp сервера 
        public static void ListDirectory(TreeView treeView, string Host, string UserName, string password)
        {
            Ftp_Client ftp = new Ftp_Client();
            ftp.Host = Host;
            ftp.UserName = UserName;
            ftp.Password = password;
            treeView.Nodes.Clear();

            var stack = new Stack<TreeNode>();
            var rootDirectory = DOMAIN;
            var node = new TreeNode(rootDirectory) { Tag = "/" };
            stack.Push(node);

            while (stack.Count > 0)
            {
                try
                {
                    var currentNode = stack.Pop();
                    var directoryInfo = ftp.ListDirectory((string)currentNode.Tag);
                    foreach (var directory in directoryInfo)
                    {
                        if (directory.IsDirectory && directory.Name!="?" && !directory.Name.Contains("????")) {
                            var childDirectoryNode = new TreeNode(directory.Name) { Tag = currentNode.Tag+directory.Name+'/'};
                            currentNode.Nodes.Add(childDirectoryNode);
                            stack.Push(childDirectoryNode);
                        }
                    }
                    foreach (var file in directoryInfo)
                        if (!file.IsDirectory && file.Name != "?" && !file.Name.Contains("????"))
                            currentNode.Nodes.Add(new TreeNode(file.Name) { Tag = currentNode.Tag + file.Name + "/f"}); ; //пометка f в конце пути означает, что это файл!
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            treeView.Nodes.Add(node);
        }

        
        //обновление дерева папок
        private void button2_Click(object sender, EventArgs e)
        {
            ListDirectory(treeView1, DOMAIN, USER, PASS);
        }

        //закрытие всех форм при закрытии основной формы
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.CloseAllWindows();
        }

        //остановка процесса tread 
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                thread.Abort();
                progressBar1.Value = 0;
                button1.Enabled = true;
                UpdateInfoLabel("");
            }
            catch 
            {
                MessageBox.Show("Отменено пользователем", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // удаление файлов
        private void button4_Click(object sender, EventArgs e)
        {

            progressBar1.Maximum = forDownload.Count;
            progressBar1.Value = 0;

            thread = (new System.Threading.Thread(delegate () {
                if (forDownload.Count < 1) return;
                //отключаем кнопки
                button1.Invoke((MethodInvoker)(() => button1.Enabled = false));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = false));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = false));
                Ftp_Client ftp = new Ftp_Client();
                ftp.Host = DOMAIN;
                ftp.UserName = USER;
                ftp.Password = PASS;
                foreach (string p in forDownload)
                {
                    try
                    {
                        if (p.LastIndexOf('f') == p.Length - 1)
                        {
                            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value++));
                            UpdateInfoLabel("Удаление " + p.Remove(p.LastIndexOf('f') - 1));
                            ftp.DeleteFile(p.Remove(p.LastIndexOf('f') - 1));
                        }

                        else
                        {
                            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value++));
                            UpdateInfoLabel("Удаление " + p);
                            ftp.RemoveDirectory(p);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                BeginInvoke(new Action(() => ListDirectory(treeView1, DOMAIN, USER, PASS)));
                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                UpdateInfoLabel("");
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = true));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = true));
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form UpForm = new UploadForm();
            UpForm.Show();
            
        }
        //
        //вызов окна настроек
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form ifrm = new SettingsForm();
            ifrm.Show();
        }

        //обновление информационной строки из другого потока
        private void UpdateInfoLabel(string text)
        {
            if (label1.InvokeRequired) //Если обратились к контролу не из того потока, в котором конрол был создан, то...
                label1.Invoke((Action<string>)UpdateInfoLabel, text); //Вызываем этот же метод через Invoke
            else
                label1.Text = text;
        }

        //кнопка начала извлечения данных
        private void button1_Click(object sender, EventArgs e)
        {
            string TMP_DIR;
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = DIR;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                TMP_DIR = FBD.SelectedPath;
            }
            else
                return;
            progressBar1.Maximum = forDownload.Count;
            progressBar1.Value = 0;
            //создаём поток
            thread = (new System.Threading.Thread(delegate () {
                if (forDownload.Count < 1) return;
                //отключаем кнопки
                button1.Invoke((MethodInvoker)(() => button1.Enabled = false));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = false));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = false));

                Ftp_Client ftp = new Ftp_Client();
                ftp.Host = DOMAIN;
                ftp.UserName = USER;
                ftp.Password = PASS;
                foreach (string filepath in forDownload)
                {
                    if (filepath.LastIndexOf('f') == filepath.Length - 1)
                    {
                        string filepath1 = filepath.Remove(filepath.LastIndexOf('f') - 1);
                        string currentfilePath = TMP_DIR + filepath1.Replace('/', '\\');
                        int r = currentfilePath.LastIndexOf('\\', currentfilePath.Length - 2);
                        currentfilePath = currentfilePath.Remove(r);

                        try
                        {
                            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value++));
                            UpdateInfoLabel(filepath1);

                            ftp.DownloadFile(filepath1, currentfilePath);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "Поток находился в процессе прерывания.")
                            {
                                
                                MessageBox.Show("Процесс отменён пользователем", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                System.IO.File.Delete(currentfilePath + @"\" + filepath1.Substring(filepath1.LastIndexOf('/') + 1));
                                return;
                            }
                                
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }
                    }
                    else
                    {
                        progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value++));
                        UpdateInfoLabel(filepath);
                        if (!Directory.Exists(TMP_DIR+ filepath.Replace('/', '\\')))
                            Directory.CreateDirectory(TMP_DIR + filepath.Replace('/', '\\'));
                    }
                }
                UpdateInfoLabel("");
                MessageBox.Show("Файлы скопированы", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value=0));
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = true));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = true));

            }));
            thread.IsBackground = true;
            thread.Start();
        }

        //добавление в лист forDownload путей файлов и папок, отмеченных в дереве
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                forDownload.Add((string)e.Node.Tag);
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode c in e.Node.Nodes)
                    {
                        c.Checked = true;
                    }
                }
            }
            else
            {
                forDownload.Remove((string)e.Node.Tag);
                foreach (TreeNode c in e.Node.Nodes)
                {
                    c.Checked = false;
                }
            }
        }

        public static void UploadFile(string FolderName = "/")
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //получаем путь к файлу
                filePath = openFileDialog.FileName;
                Ftp_Client ftp = new Ftp_Client();
                ftp.Host = DOMAIN;
                ftp.UserName = USER;
                ftp.Password = PASS;
                try
                {
                    ftp.UploadFile(FolderName, filePath);
                }
                catch (System.Net.WebException e)
                {
                    FolderName = FolderName.Replace("/","");
                    ftp.CreateDirectory("/", FolderName);
                    ftp.UploadFile('/'+FolderName+'/', filePath);
                }
                
            }
        }
    }
}