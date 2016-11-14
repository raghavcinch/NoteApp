using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Data;
using NotesData;
using NotesData.Data;

namespace Notes.Core
{
    public class NoteManager //: INotesCore
    {

        public Note CreateNote(INotesRepository<Note> repo, Note note)
        {
            return repo.Create(note);
        }

        public void DeleteNote(INotesRepository<Note> repo, Note note)
        {
            repo.Delete(note);
        }

        public Note UpdateNote(INotesRepository<Note> repo, Note note)
        {
            var currentNote = GetNote(repo, note.Id);
            if (note == null)
                throw new ArgumentNullException("Note cannot be found!");
            if (note.UpdateModeId != (int)UpdateMode.Destop)
                if (BitConverter.ToInt64(note.Rowversion, 0).ToString() != BitConverter.ToInt64(currentNote.Rowversion, 0).ToString())
                    throw new ApplicationException("Stale note!");
             currentNote.Data = note.Data;
            return currentNote;
        }

        public Note GetNote(INotesRepository<Note> repo, int Id)
        {
            return repo.Get(Id);
        }

        public IQueryable<Note> GetAll(INotesRepository<Note> repo)
        {
            return repo.GetAll();
        }

        public IQueryable<Note> GetAll(INotesRepository<Note> repo, User user)
        {
            return repo.GetAll().Where(a => a.User.Id == user.Id);
        }
    }
}
