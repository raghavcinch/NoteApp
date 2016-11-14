using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Notes.Core;
using Notes.Data;
using Notes.DIContainer;
using NotesData;
using NotesData.Data;
using AttributeRouting.Web.Mvc;

namespace Notes.API.Controllers
{
    public class NoteController : ApiController
    {

        public IEnumerable<NoteViewModel> GetUserNotes(int userId)
        {
            Thread.Sleep(10000);
            var repo = TypesContainer.GetRepository<Note>("NotesEntities");
            var notes = new NoteManager().GetAll(repo).Where(a => a.UserId == userId);
            return notes.ToList().Select(NoteViewModel.ToViewModel).AsEnumerable();
        }
        [HttpGet]
        public NoteViewModel GetNote(int id)
        {
            var repo = TypesContainer.GetRepository<Note>("NotesEntities");
            var note = repo.Get(id);

            return new NoteViewModel(note);
        }
        [HttpPut]
        public NoteViewModel PutNote(NoteViewModel note)
        {

            try
            {
                var repo = TypesContainer.GetRepository<Note>("NotesEntities");
                var updatedNote = new NoteManager().UpdateNote(repo, NoteViewModel.FromViewModel(note));
                repo.SaveChanges();
                return NoteViewModel.ToViewModel(updatedNote);
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unknown system error has occured");
            }

        }

        [HttpPost]
        public NoteViewModel PostNote(NoteViewModel note)
        {
            try
            {
                var repo = TypesContainer.GetRepository<Note>("NotesEntities");
                var updatedNote = new NoteManager().CreateNote(repo, NoteViewModel.FromViewModel(note));
                repo.SaveChanges();
                return NoteViewModel.ToViewModel(updatedNote);

            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unknown system error has occured");
            }

        }

        // DELETE api/Note/5
        [HttpDelete]
        public HttpResponseMessage DeleteNote(int id)
        {
            throw new NotImplementedException();
        }

    }
}