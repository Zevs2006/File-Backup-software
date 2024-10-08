using System;
using System.IO;
using System.Windows.Forms;

namespace File_Backup_software
{
    public partial class Form1 : Form
    {
        private string backupDirectory = @"C:\Backup\"; // ���� ��� ���������� ��������� �����

        public Form1()
        {
            InitializeComponent();
        }

        // ���������� ��� ������ ������
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // ��������� ������� ��������� ������
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    listBoxFiles.Items.Add(file); // ��������� ��������� ����� � ListBox
                }
            }
        }

        // ���������� ��� ������ �����
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                listBoxFiles.Items.Add(folderBrowserDialog.SelectedPath); // ��������� ��������� ����� � ListBox
            }
        }

        // ���������� ��� �������� ��������� �����
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory); // ������� ���������� ��� ��������� �����, ���� �� ���
            }

            foreach (var item in listBoxFiles.Items)
            {
                string sourcePath = item.ToString();
                if (File.Exists(sourcePath)) // ���������, ���� ��� ��� �����
                {
                    string destinationPath = Path.Combine(backupDirectory, Path.GetFileName(sourcePath));
                    File.Copy(sourcePath, destinationPath, true); // �������� ���� � �������
                }
                else if (Directory.Exists(sourcePath))
                {
                    string destinationPath = Path.Combine(backupDirectory, new DirectoryInfo(sourcePath).Name);
                    DirectoryCopy(sourcePath, destinationPath, true); // �������� ����� � ����������
                }
            }

            MessageBox.Show("��������� ����������� ���������!", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ���������� ��� �������������� ������
        private void btnRestore_Click(object sender, EventArgs e)
        {
            foreach (var item in listBoxFiles.Items)
            {
                string backupPath = Path.Combine(backupDirectory, Path.GetFileName(item.ToString()));
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, item.ToString(), true); // ��������������� ����
                }
                else if (Directory.Exists(backupPath))
                {
                    DirectoryCopy(backupPath, item.ToString(), true); // ��������������� �����
                }
            }

            MessageBox.Show("�������������� ���������!", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ����� ��� ����������� ���������� � ���������������
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // ���� ������� ����� �� ����������, ������� ��
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // �������� ��� ����� � ������� �����
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true); // �������� � �������, ���� ���� ����������
            }

            // ���� ����� ���������� �������������, �������� �� ����������
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
