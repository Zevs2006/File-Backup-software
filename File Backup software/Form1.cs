using System;
using System.IO;
using System.Windows.Forms;

namespace File_Backup_software
{
    public partial class Form1 : Form
    {
        private string backupDirectory = @"C:\Backup\"; // Путь для сохранения резервных копий

        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик для выбора файлов
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Позволяет выбрать несколько файлов
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    listBoxFiles.Items.Add(file); // Добавляем выбранные файлы в ListBox
                }
            }
        }

        // Обработчик для выбора папки
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                listBoxFiles.Items.Add(folderBrowserDialog.SelectedPath); // Добавляем выбранную папку в ListBox
            }
        }

        // Обработчик для создания резервной копии
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory); // Создаем директорию для резервных копий, если ее нет
            }

            foreach (var item in listBoxFiles.Items)
            {
                string sourcePath = item.ToString();
                if (File.Exists(sourcePath)) // Проверяем, файл это или папка
                {
                    string destinationPath = Path.Combine(backupDirectory, Path.GetFileName(sourcePath));
                    File.Copy(sourcePath, destinationPath, true); // Копируем файл с заменой
                }
                else if (Directory.Exists(sourcePath))
                {
                    string destinationPath = Path.Combine(backupDirectory, new DirectoryInfo(sourcePath).Name);
                    DirectoryCopy(sourcePath, destinationPath, true); // Копируем папку с содержимым
                }
            }

            MessageBox.Show("Резервное копирование завершено!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обработчик для восстановления файлов
        private void btnRestore_Click(object sender, EventArgs e)
        {
            foreach (var item in listBoxFiles.Items)
            {
                string backupPath = Path.Combine(backupDirectory, Path.GetFileName(item.ToString()));
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, item.ToString(), true); // Восстанавливаем файл
                }
                else if (Directory.Exists(backupPath))
                {
                    DirectoryCopy(backupPath, item.ToString(), true); // Восстанавливаем папку
                }
            }

            MessageBox.Show("Восстановление завершено!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Метод для копирования директорий с поддиректориями
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Если целевая папка не существует, создаем ее
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Копируем все файлы в целевую папку
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true); // Копируем с заменой, если файл существует
            }

            // Если нужно копировать поддиректории, копируем их рекурсивно
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
