using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NotesData;
using NotesData.Data;

namespace Notes.Data
{
    //public interface INotesCore
    //{
    //    Note CreateNote(INotesEntities context, Note note);
    //    void DeleteNote(INotesRepository<Note> repo, Note note);
    //    Note UpdateNote(INotesRepository<Note> repo, Note note, UpdateMode mode);
    //    Note GetNote(INotesRepository<Note> repo, int Id);
    //    IQueryable<Note> GetAll(INotesRepository<Note> repo);
    //    IQueryable<Note> GetAll(INotesRepository<Note> repo, User user);
    //}

    public interface INotesRepository<T> : IDisposable
    {
        T Create(T note);
        void Delete(T note);
        T Update(T note);
        T Get(int Id);
        IQueryable<T> GetAll();
        void SaveChanges();
    }

    public interface INotesApiResolver
    {
        string GetApiLocation();
    }

    public interface INotesEntities
    {
        
    }
    
}
