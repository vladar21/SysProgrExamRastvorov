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
//с помощью индикаторов (progress bars). 
// Пользователь через интерфейс приложения может приостановить работу 
// алгоритма, возобновить, полностью остановить.
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
            progressBar1.Minimum = 0;
            // делаем выравнивание в первом столбце по содержимому
            listView1.Columns[0].Width = 650;
            listView1.Columns[1].Width = 80;
            listView1.Columns[2].Width = -2;
        }
               
        string[] txtFiles;
        string resultPath;// = "D:\\resultFolder\\";
        List<string[]> slova = new List<string[]>();
        List<string[]> resultList = new List<string[]>();
        string root = "";

        private void button1_Click(object sender, EventArgs e)
        {
            // чистим listView
            listView1.Items.Clear();

            // формируем список запрещенных слов         
            string inputStopWords = textBox1.Text;
            // убираем все пробелы
            //inputStopWords = Regex.Replace(inputStopWords, @"(?<=\d+)\s+(?=\d+)", "");
            string[] iSW = textBox1.Text.Split(',');
            foreach(string i in iSW)
            {
                slova.Add(new string[] { i, "0" });
            }

            // работаем с файлами и каталогами            
            try
            {
                // выбираем рабочую директорию
                using (var fbd = new FolderBrowserDialog())
                {
                    MessageBox.Show("Выбор каталога, в котором мы ищем запрещенные слова в файлах.");
                    DialogResult result = fbd.ShowDialog();

                    // корневой каталог
                    root = fbd.SelectedPath;

                    // выбираем директорию для копий файлов и отчета
                    using (var fbdResult = new FolderBrowserDialog())
                    {
                        MessageBox.Show("Выбор каталога для копий файлов и файла отчета");
                        DialogResult resultFolder = fbdResult.ShowDialog();
                        resultPath = fbdResult.SelectedPath;
                    }

                    try
                    {
                        // выбираем все текстовые файлы из директории и поддиректорий
                        //var txtFiles = Directory.EnumerateFiles(root, "*.txt", SearchOption.AllDirectories);
                        txtFiles = Directory.GetFiles(root, "*.txt", SearchOption.AllDirectories);

                        // запускаем прогресс баз
                        progressBar1.Value = 0;                        
                        progressBar1.Maximum = txtFiles.Count();
                        
                        // ищем запрещенные слова, формируем listView и папку с путями запрещенных файлов
                        foreach (string currentFile in txtFiles)
                        {
                            // общее количество повторов в файле запрещенных слов
                            int qtyStopWordsInFile = 0;
                            
                            bool addToResult = false;
                            foreach (string slovo in slova[0].Distinct().ToArray())
                            {
                                // счетчик частоты слова в файле
                                int qtyWordFrequency = 0;
                                // содержимое файла
                                string[] containsCurrentFile = File.ReadAllLines(currentFile, Encoding.Default);
                                foreach (string stroka in containsCurrentFile)
                                {                                                                           
                                    if (stroka.Contains(slovo))
                                    {
                                        qtyStopWordsInFile++;
                                        qtyWordFrequency++;
                                        addToResult = true;

                                        // копируем файл содержащий запрещенные слова в результирующий каталог
                                        string fileName = Path.GetFileName(currentFile);
                                        //File.Copy(Path.Combine(currentFile), Path.Combine(resultPath, fileName), true);
                                        File.Copy(currentFile, resultPath + "\\" + fileName, true);

                                        // делаем копию скопированного файла
                                        //string copyFilePath = Path.Combine(resultPath, Path.GetFileNameWithoutExtension(currentFile) + "_copy" + Path.GetExtension(currentFile));
                                        string copyFilePath = resultPath + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "_copy" + Path.GetExtension(currentFile);
                                        //File.Copy(Path.Combine(currentFile), copyFilePath, true);
                                        File.Copy(currentFile, copyFilePath, true);

                                        // меняем в копии запрещенного файла символы запрещенного слова на звездочки
                                        string text = File.ReadAllText(copyFilePath, Encoding.Default);
                                        string zvezdochki = "";
                                        for (int i = 0; i < slovo.Count(); i++)
                                        {
                                            zvezdochki += "*";
                                        }
                                        text = text.Replace(slovo, zvezdochki);
                                        File.WriteAllText(copyFilePath, text);
                                    }
                                    
                                }
                                try
                                {
                                    // добавляем для выбранного слова в файл отчета информацию о частоте его встречи в текущем файле
                                    int sum = Convert.ToInt32(slovo[1]) + qtyWordFrequency;
                                    //string[] temp = slova.Find(x=>x == slovo);
                                    // находим слово в списке запрещенных слов и меняем значение кол-ва повторов на новое
                                    slova.Where(i => i[0] == slovo).FirstOrDefault()[1] = sum.ToString();
                                }
                                catch(Exception err)
                                {
                                    Log.Write(err);
                                }

                            }
                            // добавляем путь к скопированому файлу в список для вывода на экран через listView
                            if (addToResult)
                            {
                                //resultList.Add(currentFile);
                                // получам размер файла
                                var fi1 = new FileInfo(currentFile);

                                // формируем строчный массив для вставки в результирующий массив и в листвью
                                string[] item = { currentFile, (fi1.Length/1000).ToString() + " kB", qtyStopWordsInFile.ToString() };
                                resultList.Add(item);
                                // выводим список путей к файлам, содержащим запрещенные слова
                                //this.listView1.Items.Add(currentFile);
                                this.listView1.Items.Add(new ListViewItem(item));
                            }                            

                            // наращиваем прогресс бар
                            progressBar1.Value++;
                        }
                       
                    }
                    catch (Exception err)
                    {
                        //Console.WriteLine(err.Message);
                        Log.Write(err);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\nили неправильно заполненные данные");
                Log.Write(ex);
            }

            // формируем файл отчета и записываем его в файл
            writeReport(resultPath, slova);
        }

        public void writeReport(string p, List<string[]> list)
        {
            //DateTime date1 = new DateTime();
            string path = p + "\\report.txt";// + date1.ToString();
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                foreach(var l in list)
                {
                    var lines = l.ToList();
                    File.WriteAllLines(path, lines, Encoding.UTF8);
                } 
                
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            string appendText = "This is extra text" + Environment.NewLine;
            File.AppendAllText(path, appendText, Encoding.UTF8);

            // Open the file to read from.
            //string[] readText = File.ReadAllLines(path, Encoding.UTF8);
            //foreach (string s in readText)
            //{
            //    Console.WriteLine(s);
            //}
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

    }

    // класс под лог
    public class Log
    {
        private static object sync = new object();
        public static void Write(Exception ex)
        {
            try
            {
                // Путь .\\Log
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n",
                DateTime.Now, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }
    }
}
