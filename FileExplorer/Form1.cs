using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileExplorer
{
    public partial class Form1 : Form
    {
        List<FileInfo> actualFiles = new List<FileInfo>();

        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"..\\..");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            actualFiles = new List<FileInfo>();
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item,""),
                    new ListViewItem.ListViewSubItem(item,"Directory"),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString()) };

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                actualFiles.Add(file);
                item = new ListViewItem(file.Name);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, file.Extension),
                    new ListViewItem.ListViewSubItem(item,"File"),
                    new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()),
                     };

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            // lblHovered.Text = e.Item.Text;
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            lblHovered.Text = e.Item.Text;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFile();
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileInfo thisFile = actualFiles.Where(x => x.Name == listView1.FocusedItem.Text).FirstOrDefault();
            thisFile.Delete();
            listView1.FocusedItem.Remove();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string path = "";
            string combined = "";

            FileInfo thisFile = actualFiles.Where(x => x.Name == listView1.FocusedItem.Text).FirstOrDefault();

            //Open new form
            EditFile editFile = new EditFile();
            editFile.ShowDialog();
            if (editFile.DialogResult == DialogResult.OK)
            {
                fileName = editFile.FileName;
                path = thisFile.Directory.ToString();
                combined = path + "\\" + fileName;

                File.Copy(thisFile.FullName, combined);
                listView1.Refresh();
            }
        }

        private void OpenFile()
        {
            FileInfo thisFile = actualFiles.Where(x => x.Name == listView1.FocusedItem.Text).FirstOrDefault();
            tbContent.Text = File.ReadAllText(thisFile.FullName, Encoding.UTF7);
        }


    }
}
