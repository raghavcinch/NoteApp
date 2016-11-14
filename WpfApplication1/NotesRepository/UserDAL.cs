using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Data;
using NotesData.Data;
using NotesRepository;

namespace NotesRepository
{
    public class UserDAL : INotesRepository<User>
    {
        public NotesEntities Context { get; set; }
        public UserDAL()
        {
            Context = new NotesEntities();
        }
        public UserDAL(string connectionString)
        {
            Context = new NotesEntities(connectionString);
        }
        public User Create(User user)
        {
            Context.Users.Add(new User()
            {
               Username = user.Username
            });
            return user;
        }

        public void Delete(User User)
        {
            throw new NotImplementedException();
        }

        public User Update(User User)
        {
            return User;
        }

        public User Get(int Id)
        {
            return Context.Users.FirstOrDefault(a => a.Id == Id);
        }

        public IQueryable<User> GetAll()
        {
            return Context.Users.AsQueryable();
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
