using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Data;

namespace WpfApplication1
{
    public interface INotesManager
    {
        Task<IEnumerable<NoteViewModel>> GetNotes(int userId);
        Task<NoteViewModel> GetNote(NoteViewModel note);
        Task<NoteViewModel> UpdateNote(NoteViewModel note);
        Task<NoteViewModel> CreateNote(NoteViewModel note);
        Task<NoteViewModel> DeleteNote(NoteViewModel note);
        string GetSource();

    }

    public enum NotesManagerType
    {
        Online,
        Offline
    }

    public class NotesFatory
    {
        public static string CloudApi { get; set; }

        public static INotesManager GetManager(NotesManagerType type)
        {
            switch (type)
            {
                case NotesManagerType.Online:
                    return new NotesSyncManager(NotesFatory.CloudApi);
                case NotesManagerType.Offline:
                    return new NotesFileManager();
                default:
                    return null;
            }
        }
    }
}
