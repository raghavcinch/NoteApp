using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;
using Notes.Data;
using NotesData.Data;
using NotesRepository;

namespace Notes.DIContainer
{
    public static class TypesContainer
    {
        private static IKernel _kernal;

        static TypesContainer()
        {
            IKernel container = new StandardKernel();

            container.Bind<INotesRepository<Note>>().To<NoteRepository>();//.WithConstructorArgument("fakeConnection");
            container.Bind<INotesRepository<User>>().To<UserDAL>();
            _kernal = container;
        }

        public static INotesRepository<T> GetRepository<T>(string connectionString)
        {
            INotesRepository<T> objRepo = _kernal.Get<INotesRepository<T>>();
            return objRepo;
        }

    }
}
