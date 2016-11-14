using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Notes.Data;
using NotesData;
using NotesData.Data;

namespace WpfApplication1
{
    public class NotesClient : WebClient
    {
        public string Url { get; set; }

        public NotesClient(string url)
        {
            this.Url = url;
        }
    }

    public class NotesSyncManager : INotesManager
    {
        private NotesClient client;
        public NotesSyncManager(string cloudApi)
        {
            client = new NotesClient(cloudApi);
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
        }

        public async Task<IEnumerable<NoteViewModel>> GetNotes(int userId)
        {
            var url = client.Url + new NoteViewModel().GetApiLocation() + "?userId=" + userId;
            var data = await client.DownloadStringTaskAsync(url);
            var noteViewModelCollection = JsonConvert.DeserializeObject<IEnumerable<NoteViewModel>>(data);
            return noteViewModelCollection;
        }


        public async Task<NoteViewModel> GetNote(NoteViewModel note)
        {
            var url = client.Url + new NoteViewModel().GetApiLocation() + "/" + note.Id;
            var data = await client.DownloadStringTaskAsync(url);
            var noteViewModelCollection = JsonConvert.DeserializeObject<NoteViewModel>(data);
            return noteViewModelCollection;
        }

        public async Task<NoteViewModel> UpdateNote(NoteViewModel note)
        {
            note.UpdateModeId = (int) UpdateMode.Destop;
            var url = client.Url + new NoteViewModel().GetApiLocation() + "/" + note.Id;
            var noteString = JsonConvert.SerializeObject(note);
            var data = await client.UploadStringTaskAsync(url, "PUT", noteString);
            var noteViewModelCollection = JsonConvert.DeserializeObject<NoteViewModel>(data);
            return noteViewModelCollection;
        }

        public async Task<NoteViewModel> CreateNote(NoteViewModel note)
        {
            note.UpdateModeId = (int)UpdateMode.Destop;
            var url = client.Url + new NoteViewModel().GetApiLocation() + "/" + note.Id;
            var noteString = JsonConvert.SerializeObject(note);
            var data = await client.UploadStringTaskAsync(url, "POST", noteString);
            var noteViewModelCollection = JsonConvert.DeserializeObject<NoteViewModel>(data);
            return noteViewModelCollection;
        }

        public async Task<NoteViewModel> DeleteNote(NoteViewModel note)
        {
            throw new NotImplementedException();
        }


        public string GetSource()
        {
            return "Online Storage";
        }
    }
}
