using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Lab2_PavA_IKT
{
    public partial class DirectoryForm : Form
    {
        public DirectoryForm()
        {
            InitializeComponent();
        }

        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) 
        { 
            Build(e.Node); 
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            string fullPath = selectedNode.FullPath;
            DirectoryInfo di = new DirectoryInfo(fullPath);
            FileInfo[] fiArray;
            DirectoryInfo[] diArray;
            try
            {
                fiArray = di.GetFiles();
                diArray = di.GetDirectories();
            }
            catch { return; }
            DataTable.Items.Clear(); //очищаем таблицу перед каждым "обновлением"
            foreach (DirectoryInfo dirInfo in diArray) //папки
            {
                ListViewItem list = new ListViewItem(dirInfo.Name);
                list.Checked = true;
                list.SubItems.Add(" ");
                list.SubItems.Add("Folder with files");
                DataTable.Items.Add(list);
            }
            long small = 0, mid = 0, big = 0;
            long count = 0;
            foreach (FileInfo fileInfo in fiArray) //файлы
            {
                count += fileInfo.Length;
                string name = fileInfo.Name;
                ListViewItem list = new ListViewItem(name);
                list.Checked = true;
                list.Tag = fileInfo.FullName;
                list.SubItems.Add(fileInfo.Length.ToString() + " B"); //размер файла
                string type;
                Regex reg = new Regex(@"(\.\w{1,4}$)"); //ищет в конце строки от 1 до 4 символом, перед которыми стоит "."
                foreach (Match wrd in reg.Matches(name))
                {
                    type = wrd.ToString(); //тип файла 
                    list.SubItems.Add(type);
                    DataTable.Items.Add(list);
                    //окрашивание строк таблицы
                    if (type == ".png" || type == ".jpg" || type == ".bmp" || type == ".gif" || type == ".JPG")
                        list.BackColor = Color.Thistle; //светло-фиолетовый
                    else if (type == ".docx" || type == ".xlsx" || type == ".pdf" || type == ".txt")
                        list.BackColor = Color.LightGreen;
                    else if (type == ".zip" || type == ".rar" || type == ".7z")
                        list.BackColor = Color.Khaki;
                    else if (type == ".exe" || type == ".dll" || type == ".ini")
                        list.BackColor = Color.Pink;
                }

                //график (6 вариант)
                chart1.Series.Clear();
                chart1.Series.Add("Max size");
                if (fileInfo.Length <= 10000)
                {
                    if (fileInfo.Length > small)
                        small = fileInfo.Length;
                }
                if (fileInfo.Length > 10000 && fileInfo.Length < 100000)
                {
                    if (fileInfo.Length > mid)
                        mid = fileInfo.Length;
                }
                if (fileInfo.Length >= 100000)
                {
                    if (fileInfo.Length > big)
                        big = fileInfo.Length;
                }
                chart1.Series[0].Points.AddXY("Small", small);
                chart1.Series[0].Points.AddXY("Mid", mid);
                chart1.Series[0].Points.AddXY("Big", big);

            }
            statusStrip1.Items[0].Text = "Total bytes: " + count;
            statusStrip1.Items[1].Text = DataTable.CheckedItems.Count + " of " + DataTable.CheckedItems.Count + " items selected"; 
        }
        

        private void Build(TreeNode parent) //формирование дерева
        {
            var path = parent.Tag as string;
            parent.Nodes.Clear();
            foreach (var dir in Directory.GetDirectories(path))
                parent.Nodes.Add(new TreeNode(Path.GetFileName(dir), new[] { new TreeNode("...") }) { Tag = dir });
            foreach (var file in Directory.GetFiles(path))
                parent.Nodes.Add(new TreeNode(Path.GetFileName(file), 1, 1) { Tag = file });
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) //open file
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TreeNode node = new TreeNode() { Text = fbd.SelectedPath.ToString(), Tag = fbd.SelectedPath };
                treeView1.Nodes.Add(node);
                Build(node);
                node.Expand();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) //exit
        {
            string caption = "Directory Form"; // имя шапки окна
            string message = "Do you want close this form?"; // содержимое окна
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == DialogResult.Yes)
                this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) //save
        {
            saveFileDialog1.InitialDirectory = "C:/Users/admin/Desktop/icons";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e) //font
        {
            FontDialog font = new FontDialog();
            if (font.ShowDialog() == DialogResult.OK)
                DataTable.Font = font.Font;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e) //color
        {
            ColorDialog color = new ColorDialog();
            if (color.ShowDialog() == DialogResult.OK)
                DataTable.ForeColor = color.Color;
        }
        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "C:/Users/admin/Desktop/icons/Новый Текстовый Документ.txt");
            File.WriteAllText(path, "Содержимое файла");
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            { 
                    System.Diagnostics.Process.Start("C:/Users/admin/Desktop/icons/Новый Текстовый Документ.txt");      
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string caption = "Directory Form";
            string message = "Если не работает перезапусти." +"\n Если работает не ломай!";
            MessageBox.Show(message, caption);
        }      
    }
}
