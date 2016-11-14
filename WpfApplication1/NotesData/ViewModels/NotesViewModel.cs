using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Notes.Data;
using NotesData;
using NotesData.Data;

namespace Notes.Data
{
    public sealed class NotesViewModel
    {
        public ObservableCollection<NoteViewModel> Notes { get; set; }

        public NotesViewModel()
        {
            Notes = new ObservableCollection<NoteViewModel>();
        }

        public NotesViewModel(IEnumerable<NoteViewModel> notes)
        {
            Notes = new ObservableCollection<NoteViewModel>(notes);

        }
    }
    public class NoteViewModel : INotesApiResolver
    {
        public string Title { get; set; }
        public string Data { get; set; }
        public int Id { get; set; }
        public string RowVersion { get; set; }
        public int UserId { get; set; }
        public int LocalId { get; set; }
        public int UpdateModeId { get; set; }

        public NoteViewModel()
        {
        }

        public NoteViewModel(Note note)
        {
            this.Id = note.Id;
            this.Data = note.Data;
            this.Title = note.Title;
            this.UserId = note.UserId;
            this.UpdateModeId = note.UpdateModeId;

            this.RowVersion = note.Rowversion != null ? BitConverter.ToInt64(note.Rowversion, 0).ToString() : "-1";
        }

        public static NoteViewModel ToViewModel(Note note)
        {
            var noteVM = new NoteViewModel(note);
            return noteVM;
        }

        public static Note FromViewModel(NoteViewModel noteViewModel)
        {
            if (noteViewModel.RowVersion == null)
                noteViewModel.RowVersion = "-1";
            var note = new Note
            {
                Id = noteViewModel.Id,
                Data = noteViewModel.Data,
                Title = noteViewModel.Title,
                Rowversion = BitConverter.GetBytes(long.Parse(noteViewModel.RowVersion)),
                UserId = noteViewModel.UserId,
                UpdateModeId = noteViewModel.UpdateModeId
            };
            return note;
        }


        public string GetApiLocation()
        {
            return "Note";
        }
    }
}