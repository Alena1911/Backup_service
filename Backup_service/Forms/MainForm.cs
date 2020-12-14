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
        public List<string> forDownload2 = new List<string>();//список ссылок на загрузку
        public List<string> forDownload3 = new List<string>();//список ссылок на загрузку
        public static string DOMAIN, USER, PASS, COMMONPASS, DIR;//объявление переменных, необходимых для работы с ftp сервером
        public static string DOMAIN2, USER2, PASS2;
        public static string DOMAIN3, USER3, PASS3;

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
                    DOMAIN2 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "DOMAIN2"), COMMONPASS);
                    USER2 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "USER2"), COMMONPASS);
                    PASS2 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "PASS2"), COMMONPASS);
                    DOMAIN3 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "DOMAIN3"), COMMONPASS);
                    USER3 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "USER3"), COMMONPASS);
                    PASS3 = EncryptDecrypt.DeShifrovka(INI.ReadINI("MainSettings", "PASS3"), COMMONPASS);
                }
                catch
                {
                    MessageBox.Show("Ошибка извлечения файла конфигурации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.CloseAllWindows();
                }
                //строим деревья для каждого сервера
                ListDirectory(treeView1,DOMAIN,USER,PASS);
                ListDirectory(treeView1, DOMAIN2, USER2, PASS2);
                ListDirectory(treeView1, DOMAIN3, USER3, PASS3);


            }
        }


        
        //Построение дерева файловой системы ftp сервера 
        public static void ListDirectory(TreeView treeView, string Host, string UserName, string password)
        {
            Ftp_Client ftp = new Ftp_Client();
            ftp.Host = Host;
            ftp.UserName = UserName;
            ftp.Password = password;
            //treeView.Nodes.Clear();

            var stack = new Stack<TreeNode>();
            var rootDirectory = Host;
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
            treeView1.Nodes.Clear();
            ListDirectory(treeView1, DOMAIN, USER, PASS);
            ListDirectory(treeView1, DOMAIN2, USER2, PASS2);
            ListDirectory(treeView1, DOMAIN3, USER3, PASS3);
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
            progressBar1.Value = 0;

            thread = (new System.Threading.Thread(delegate () {
                if (forDownload.Count < 1) return;
                //отключаем кнопки
                button1.Invoke((MethodInvoker)(() => button1.Enabled = false));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = false));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = false));
                Ftp_Client ftp = new Ftp_Client();
                if (forDownload.Count > 0)
                {
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = forDownload.Count));
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
                    forDownload.Clear();
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }

                if (forDownload2.Count > 0)
                {
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = forDownload2.Count));
                    ftp.Host = DOMAIN2;
                    ftp.UserName = USER2;
                    ftp.Password = PASS2;
                    foreach (string p in forDownload2)
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
                    forDownload2.Clear();
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }

                if (forDownload3.Count > 0)
                {
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = forDownload3.Count));
                    ftp.Host = DOMAIN3;
                    ftp.UserName = USER3;
                    ftp.Password = PASS3;
                    foreach (string p in forDownload3)
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
                    forDownload3.Clear();
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }
                
                treeView1.Invoke((MethodInvoker)(() => treeView1.Nodes.Clear()));
                BeginInvoke(new Action(() => ListDirectory(treeView1, DOMAIN, USER, PASS)));
                BeginInvoke(new Action(() => ListDirectory(treeView1, DOMAIN2, USER2, PASS2)));
                BeginInvoke(new Action(() => ListDirectory(treeView1, DOMAIN3, USER3, PASS3)));
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
                if (forDownload.Count > 0)
                {
                    ftp.Host = DOMAIN;
                    ftp.UserName = USER;
                    ftp.Password = PASS;
                    foreach (string filepath in forDownload)
                    {
                        if (filepath.LastIndexOf('f') == filepath.Length - 1)
                        {

                            string filepath1 = filepath.Remove(filepath.LastIndexOf('f') - 1);
                            string currentfilePath;
                            if (forDownload.Contains(Path.GetDirectoryName(filepath1).Replace('\\', '/') + '/'))
                                currentfilePath = TMP_DIR + filepath1.Replace('/', '\\');
                            else
                                currentfilePath = TMP_DIR + '\\' + Path.GetFileName(filepath1);
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
                            if (!Directory.Exists(TMP_DIR + filepath.Replace('/', '\\')))
                                Directory.CreateDirectory(TMP_DIR + filepath.Replace('/', '\\'));
                        }
                    }
                    UpdateInfoLabel("");
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }

                if (forDownload2.Count > 0)
                {
                    ftp.Host = DOMAIN2;
                    ftp.UserName = USER2;
                    ftp.Password = PASS2;
                    foreach (string filepath in forDownload2)
                    {
                        if (filepath.LastIndexOf('f') == filepath.Length - 1)
                        {

                            string filepath1 = filepath.Remove(filepath.LastIndexOf('f') - 1);
                            string currentfilePath;
                            if (forDownload2.Contains(Path.GetDirectoryName(filepath1).Replace('\\', '/') + '/'))
                                currentfilePath = TMP_DIR + filepath1.Replace('/', '\\');
                            else
                                currentfilePath = TMP_DIR + '\\' + Path.GetFileName(filepath1);
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
                            if (!Directory.Exists(TMP_DIR + filepath.Replace('/', '\\')))
                                Directory.CreateDirectory(TMP_DIR + filepath.Replace('/', '\\'));
                        }
                    }
                    UpdateInfoLabel("");
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }

                if (forDownload3.Count > 0)
                {
                    ftp.Host = DOMAIN3;
                    ftp.UserName = USER3;
                    ftp.Password = PASS3;
                    foreach (string filepath in forDownload3)
                    {
                        if (filepath.LastIndexOf('f') == filepath.Length - 1)
                        {

                            string filepath1 = filepath.Remove(filepath.LastIndexOf('f') - 1);
                            string currentfilePath;
                            if (forDownload3.Contains(Path.GetDirectoryName(filepath1).Replace('\\', '/') + '/'))
                                currentfilePath = TMP_DIR + filepath1.Replace('/', '\\');
                            else
                                currentfilePath = TMP_DIR + '\\' + Path.GetFileName(filepath1);
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
                            if (!Directory.Exists(TMP_DIR + filepath.Replace('/', '\\')))
                                Directory.CreateDirectory(TMP_DIR + filepath.Replace('/', '\\'));
                        }
                    }
                    UpdateInfoLabel("");
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = 0));
                }

                MessageBox.Show("Файлы скопированы", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = true));
                button4.Invoke((MethodInvoker)(() => button4.Enabled = true));

            }));
            thread.IsBackground = true;
            thread.Start();
        }

        //добавление в лист forDownload, forDownload1, forDownload2 путей файлов и папок, отмеченных в дереве
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.FullPath.Substring(0, e.Node.FullPath.IndexOf('\\')) == DOMAIN)
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
             else if (e.Node.FullPath.Substring(0, e.Node.FullPath.IndexOf('\\')) == DOMAIN2)
            {
                if (e.Node.Checked)
                {
                    forDownload2.Add((string)e.Node.Tag);
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
                    forDownload2.Remove((string)e.Node.Tag);
                    foreach (TreeNode c in e.Node.Nodes)
                    {
                        c.Checked = false;
                    }
                }
            }
            else if (e.Node.FullPath.Substring(0, e.Node.FullPath.IndexOf('\\')) == DOMAIN3)
            {
                if (e.Node.Checked)
                {
                    forDownload3.Add((string)e.Node.Tag);
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
                    forDownload3.Remove((string)e.Node.Tag);
                    foreach (TreeNode c in e.Node.Nodes)
                    {
                        c.Checked = false;
                    }
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
                    ListDirectory(treeView1, DOMAIN, USER, PASS);
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