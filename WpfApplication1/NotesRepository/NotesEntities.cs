using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Data;

namespace NotesRepository
{
    public partial class NotesEntities  : INotesEntities
    {
        public NotesEntities(string connectionString)
            : base("name=" + connectionString)
        {
            
        }
    }
}
