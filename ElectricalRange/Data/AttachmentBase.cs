using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Windows.MessageWindows;

using Microsoft.Data.SqlClient;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace ProjectsNow.Data
{
    public class AttachmentBase : Base
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }

    public static class Attachment
    {
        private static string GetFileType(string fileType)
        {
            if (fileType == null)
            {
                fileType = "Pdf Files|*.pdf";
            }
            else
            {
                fileType = $"Files | *.{fileType}";
            }

            return fileType;
        }
        public static bool SaveFile<T>(AttachmentBase attachment, string fileType = null) where T : class
        {
            fileType = GetFileType(fileType);
            OpenFileDialog path = new() { Filter = fileType };
            _ = path.ShowDialog();

            if (string.IsNullOrWhiteSpace(path.SafeFileName))
                return false;

            attachment.Name = path.SafeFileName;
            using (var stream = new FileStream(path.FileName, FileMode.Open, FileAccess.Read))
            {
                using var reader = new BinaryReader(stream);
                attachment.Data = reader.ReadBytes((int)stream.Length);
            }

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Insert(attachment as T);

            return true;
        }
        public static bool GetFileReady<T>(AttachmentBase attachment, string fileType = null) where T : class
        {
            fileType = GetFileType(fileType);
            OpenFileDialog path = new() { Filter = fileType };
            _ = path.ShowDialog();

            if (string.IsNullOrWhiteSpace(path.SafeFileName))
                return false;

            attachment.Name = path.SafeFileName;
            using var stream = new FileStream(path.FileName, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            attachment.Data = reader.ReadBytes((int)stream.Length);

            return true;
        }
        public static bool UpdateFile<T>(AttachmentBase attachment, string fileType = null) where T : class
        {
            fileType = GetFileType(fileType);
            OpenFileDialog path = new() { Filter = fileType };
            _ = path.ShowDialog();

            if (string.IsNullOrWhiteSpace(path.SafeFileName))
                return false;

            attachment.Name = path.SafeFileName;
            using (var stream = new FileStream(path.FileName, FileMode.Open, FileAccess.Read))
            {
                using var reader = new BinaryReader(stream);
                attachment.Data = reader.ReadBytes((int)stream.Length);
            }

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(attachment as T);

            return true;
        }
        public static void DeleteFile<T>(AttachmentBase attachment) where T : class
        {
            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Delete(attachment as T);
        }
        private static AttachmentBase GetAttachment<T>(AttachmentBase attachment) where T: class
        {
            string query = $"Select * from Table Where Id = {attachment.Id}";

            if (typeof(T).GetCustomAttribute(typeof(TableAttribute)) is TableAttribute checkTableAttribute)
            {
                query = query.Replace("Table", checkTableAttribute.Name);
            }

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                attachment = connection.QueryFirstOrDefault<T>(query) as AttachmentBase;
            }

            return attachment;
        }
        public static bool DownloadFile<T>(AttachmentBase attachment) where T : class
        {
            if (attachment.Data == null)
                attachment = GetAttachment<T>(attachment);

            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Pdf Files|*.pdf";
            saveFileDialog.FileName = attachment.Name;
            saveFileDialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                return false;

            using FileStream fileStream = new(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
            fileStream.Write(attachment.Data, 0, attachment.Data.Length);

            return true;
        }
        public static void OpenFile<T>(AttachmentBase attachment) where T : class
        {
            try
            {
                if (attachment.Data == null)
                    attachment = GetAttachment<T>(attachment);

                string type = attachment.Name.Substring(attachment.Name.LastIndexOf('.') + 1);

                if (File.Exists($"Data.{type}"))
                    File.Delete($"Data.{type}");

                attachment.Name = $"Data.{type}";

                using FileStream fileStream = new(attachment.Name, FileMode.Create, FileAccess.Write);
                fileStream.Write(attachment.Data, 0, attachment.Data.Length);
                System.Diagnostics.Process.Start(fileStream.Name);
            }
            catch (Exception ex)
            {
                MessageWindow.Show("Error", ex.Message, MessageWindowButton.OK, MessageWindowImage.Error);
            }
        }
    }
}
