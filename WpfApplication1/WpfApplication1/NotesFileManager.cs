using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Notes.Data;

namespace WpfApplication1
{
    public class NotesFileManager : INotesManager
    {
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public async Task<IEnumerable<NoteViewModel>> GetNotes(int userId)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var notesPath = Directory.GetFiles(currentDirectory, userId + "_*.note");
            var notes = new List<NoteViewModel>();
            foreach (var path in notesPath)
            {
                var noteData = await ReadTextAsync(path);
                if (string.IsNullOrEmpty(noteData)) continue;
                var storedNote = JsonConvert.DeserializeObject<NoteViewModel>(noteData);
                notes.Add(storedNote);
            }
            return notes;
        }

        public async Task<NoteViewModel> GetNote(NoteViewModel note)
        {
            var filePath = note.UserId + "_" + note.Id.ToString() + ".note";
            if (!File.Exists(filePath))
                throw new ApplicationException("File not found!");
            var noteData = await ReadTextAsync(filePath);
            if (string.IsNullOrEmpty(noteData)) return null;
            var storedNote = JsonConvert.DeserializeObject<NoteViewModel>(noteData);
            return storedNote;
        }

        public async Task<NoteViewModel> UpdateNote(NoteViewModel note)
        {
            await WriteTextAsync(note.UserId + "_" + note.Id.ToString() + ".note", JsonConvert.SerializeObject(note));
            return note;
        }

        public async Task<NoteViewModel> CreateNote(NoteViewModel note)
        {
            int fileId = 0;
            if (note.Id == 0)
            {
                if (note.LocalId == 0)
                    note.LocalId = GenerateLocalId(note.UserId);
                fileId = note.LocalId;
            }
            else
                fileId = note.Id;
            var fileExtn = note.LocalId != 0 ? ".local.note" : ".note";
            await WriteTextAsync(note.UserId + "_" + fileId + fileExtn, JsonConvert.SerializeObject(note));
            return note;
        }

        private int GenerateLocalId(int userId)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var notesPath = Directory.GetFiles(currentDirectory, userId + "_*.local.note");
            int maxNumber = 0;
            if (notesPath.Any())
                maxNumber = notesPath.Select(a =>
                {
                    var fileName = a.Split('.')[0];
                    var number = fileName.Split('_')[1];
                    return int.Parse(number);
                }).Max();
            return maxNumber + 1;
        }

        public async Task<NoteViewModel> DeleteNote(NoteViewModel note)
        {
            var filePath = "";
            if (note.LocalId > 0)
                filePath = note.UserId + "_" + note.LocalId.ToString() + ".local.note";
            else filePath = note.UserId + "_" + note.Id.ToString() + ".note";
            if (File.Exists(filePath))
                File.Delete(filePath);
            return note;
        }

        public string GetSource()
        {
            return "Offline Storage";
        }
        async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                }
            locker.EnterWriteLock();
            try
            {
                using (var sourceStream = new FileStream(filePath,
                               FileMode.Create, FileAccess.Write, FileShare.None,
                               bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                };
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
        async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }
    }
}
