//Реализовать приложение, позволяющее искать некоторый набор запрещенных 
//слов в файлах.
//Пользовательский интерфейс приложения должен позволять ввести или
//загрузить из файла набор запрещенных слов. При нажатии на кнопку «Старт»,
//приложение должно начать искать эти слова на всех доступных накопителях
//информации (жесткие диски, флешки).
//Файлы, содержащие запрещенные слова, должны быть скопированы в заданную 
//папку. Кроме оригинального файла, нужно создать новый файл с содержимым
//оригинального файла, в котором запрещенные слова заменены на 7 
// повторяющихся звезд (*******).
//Также нужно создать файл отчета. Он должен содержать информацию о
//всех найденных файлах с запрещенными словами, пути к этим файлам, размер
//файлов, информацию о количестве замен и так далее. В файле отчета нужно
//также отобразить топ-10 самых популярных запрещенных слов.
//Интерфейс программы должен показывать прогресс работы приложения
//с помощью индикаторов (progress bars). Пользователь через интерфейс 
// приложения может приостановить работу алгоритма, возобновить, полностью
//остановить.
//По итогам работы программы необходимо вывести результаты работы в
//элементы пользовательского интерфейса (нужно продумать, какие элементы
//управления понадобятся).
//Программа обязательно должна использовать механизмы многопоточности и синхронизации!
//Программа может быть запущена только в одной копии. Предусмотреть
//возможность запуска приложения из командной строки без отображения
//визуального интерфейса.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysProgrExamRastvorov
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        List<FileInfo> txtFiles;
        string resultPath = "D:\\resultFolder\\";

        int maxFiles, currentFiles = 0, maxFolders, currenttFolders = 0;

        string root = "";

        private void button2_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            // работаем с файлами и каталогами
            try
            {
                maxFolders = int.Parse(MaxFoldersBox.Text);
                maxFiles = int.Parse(MaxFilesBox.Text);
                currentFiles = currenttFolders = 0;
                treeView1.Nodes.Clear();

                //FolderBrowserDialog fbd = new FolderBrowserDialog();
                //fbd.Description = "Выбор каталога для поиска";
                //if (fbd.ShowDialog() == DialogResult.OK)
                //{
                //    // корневой каталог рекурсии
                //    root = fbd.SelectedPath;
                //    TreeNode TreeRoot = treeView1.Nodes.Add(root);
                //    recurseFind(root, TreeRoot);
                //}

                // выбираем директорию и строчные (текстовые) файлы в нем
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    // корневой каталог рекурсии
                    root = fbd.SelectedPath;
                    //TreeNode TreeRoot = treeView1.Nodes.Add(root);   
                    //recurseFind(root, TreeRoot);

                    try
                    {
                        //var txtFiles = Directory.EnumerateFiles(root, "*.txt", SearchOption.AllDirectories);
                        string[] txtFiles = Directory.GetFiles(root, "*.txt", SearchOption.AllDirectories);
                        this.listBox1.Items.AddRange(txtFiles);

                        foreach (string currentFile in txtFiles)
                        {
                            //foreach()
                            //if (currentFile.Contains())
                            //string fileName = currentFile.Substring(root.Length + 1);
                            string fileName = Path.GetFileName(currentFile);
                            //Directory.(currentFile, Path.Combine(resultPath, fileName));
                            File.Copy(Path.Combine(currentFile), Path.Combine(resultPath, fileName), true);
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }

                    System.Windows.Forms.MessageBox.Show("Files found: " + txtFiles.Count.ToString(), "Message");
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nили неправильно заполненные данные");
            }
        }

        Regex reg = new Regex("^<dir>(.*)$");

        void recurseFind(string path, TreeNode node)
        {
            string[] files = { };
            string[] folders = { };
            try
            {
                folders = Directory.GetDirectories(path);
            }
            catch
            {
            }
            try
            {
                files = Directory.GetFiles(path);
            }
            catch
            {
            }
            // список каталогов-узлов дерева
            List<TreeNode> sub = new List<TreeNode>();
            foreach (string i in folders)
            {
                sub.Add(node.Nodes.Add("<dir>" + i));
            }
            foreach (string i in files)
                node.Nodes.Add(i);
            currenttFolders += folders.Length;
            currentFiles += files.Length;
            if (currentFiles >= maxFiles || currenttFolders >= maxFolders)
            {
                return;
            }
            foreach (TreeNode i in sub)
            {
                Match match = reg.Match(i.Text);
                if (match.Success)
                {
                    // выбираем группу связанную со скобками
                    string nodeName = match.Groups[1].Value;
                    // рекурсивно входим в указанную директорию
                    recurseFind(nodeName, i);
                }
            }
        }

        public static void KluchSlova(string stroka, string[] slova) //Вбиваем сюда строку и ключевые слова, заключенные в данный массив
        {
            for (int i = 0; i < slova.Length; i++)
            {
                if (stroka.Contains(slova[i])) Console.WriteLine(slova[i]);
            }

        }
    }
}
