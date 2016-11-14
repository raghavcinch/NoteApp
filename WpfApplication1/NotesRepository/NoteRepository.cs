using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Data;
using NotesData.Data;

namespace NotesRepository
{
    public class NoteRepository : INotesRepository<Note>
    {
        public NotesEntities Context { get; set; }
        public NoteRepository()
        {
            Context = new NotesEntities();
        }
        public NoteRepository(string connectionString)
        {
            Context = new NotesEntities(connectionString);
        }
        public Note Create(Note note)
        {
            return Context.Notes.Add(new Note()
            {
                Data = note.Data,
                Title = note.Title,
                UserId = note.UserId
            });
        }

        public void Delete(Note note)
        {
            throw new NotImplementedException();
        }

        public Note Update(Note note)
        {
            return note;
        }

        public Note Get(int Id)
        {
            return Context.Notes.FirstOrDefault(a => a.Id == Id);
        }

        public IQueryable<Note> GetAll()
        {
            return Context.Notes.AsQueryable();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
