using Notes.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NotesData.Data;
using System.Configuration;
using System.Threading;
using System.Timers;

namespace WpfApplication1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int userId = 1;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                txtTitle.IsEnabled = txtData.IsEnabled = false;
                NotesFatory.CloudApi = ConfigurationManager.AppSettings["cloudEndpoint"];
                 var dueTime = TimeSpan.FromSeconds(0);
                 var interval = TimeSpan.FromSeconds(5);
                 RunPeriodicAsync(() => SyncAll(), dueTime, interval, CancellationToken.None);

            }
            catch (Exception x)
            {
                lblSync.Content = "Incorrect configuration..";
            }

        }
        private static async Task RunPeriodicAsync(Action action,
                                           TimeSpan dueTime, 
                                           TimeSpan interval, 
                                           CancellationToken token)
            {
              // Initial wait time before we begin the periodic loop.
              if(dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

              // Repeat this loop until cancelled.
              while(!token.IsCancellationRequested)
              {
                // Call our action function.
                action.Invoke();

                // Wait to repeat again.
                if(interval > TimeSpan.Zero)
                  await Task.Delay(interval, token);       
              }
            }

        private void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            txtTitle.IsEnabled = txtData.IsEnabled = true;
            var newNote = NoteViewModel.ToViewModel(new Note() { Id = 0, Data = "", Title = "New Note...", UserId = userId });
            var notes = (DataContext as NotesViewModel);
            if(notes == null)
                notes = new NotesViewModel() { Notes = new ObservableCollection<NoteViewModel>()};
            notes.Notes.Add(newNote);
            var indexOfNewNote = notes.Notes.IndexOf(newNote);
            list.SelectedItem = list.Items.GetItemAt(indexOfNewNote);
            txtTitle.Focus();
        }

        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = list.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    await SyncItem(selectedIndex);
                    list.SelectedIndex = selectedIndex;
                }

            }
            catch (Exception ex)
            {
                lblSync.Content = "Sync Error: " + ex.Message;
            }
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    //To kick start the sync.
            //    await SyncAll();
            //    list.SelectedIndex = 0;
            //}
            //catch (Exception ex)
            //{
            //    lblSync.Content = "Sync Error: " + ex.Message;
            //}
        }

        private async void TxtData_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = list.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    await SyncItem(selectedIndex);
                    list.SelectedIndex = selectedIndex;
                }

            }
            catch (Exception ex)
            {
                lblSync.Content = "Sync Error: " + ex.Message;
            }
        }

        private async Task SyncItem(int selectedIndex)
        {
            var notes = DataContext as NotesViewModel;
            var note = notes.Notes.ElementAt(selectedIndex);
            if (note.Id == 0)
            {
                note = await CreateNotes(NotesManagerType.Offline, note);
                var cloudNote = await CreateNotes(NotesManagerType.Online, note);
                note = await DeleteNotes(NotesManagerType.Offline, note);
                note = await CreateNotes(NotesManagerType.Offline, cloudNote);
            }
            else
            {
                note = await UpdateNotes(NotesManagerType.Offline, note);
                note = await UpdateNotes(NotesManagerType.Online, note);
            }
            notes.Notes[selectedIndex] = note;
            DataContext = notes;
        }
        private async Task SyncAll()
        {

            var offlineNotes = await GetNotes(NotesManagerType.Offline);
            var selectedIndex = list.SelectedIndex > 0 ? list.SelectedIndex : 0;
            DataContext = offlineNotes;
            list.SelectedIndex = selectedIndex;
            var onlineNotes = await GetNotes(NotesManagerType.Online);
            lblSync.Content = "Last Sync time " + DateTime.Now.ToShortTimeString();

            var syncedNotes = offlineNotes.Notes.Join(onlineNotes.Notes, local => local.RowVersion, cloud => cloud.RowVersion, (local, cloud) => local);
            var syncedNoteId = syncedNotes.Select(a => a.Id);
            var unSyncedNotes = offlineNotes.Notes.Where(a => !syncedNoteId.Contains(a.Id));
            var cloudNotes = onlineNotes.Notes.Where(a => !syncedNoteId.Contains(a.Id));
            var conflictNotes = unSyncedNotes.Where(a => onlineNotes.Notes.Select(o => o.Id).Contains(a.Id));

            foreach (var note in cloudNotes)
            {
                if (offlineNotes.Notes.Select(a => a.Id).Contains(note.Id))
                {
                    await UpdateNotes(NotesManagerType.Offline, note);
                    var boundList = (DataContext as NotesViewModel);
                    int index = boundList.Notes.IndexOf(boundList.Notes.FirstOrDefault(a => a.Id == note.Id));
                    boundList.Notes[index] = note;
                }
                else
                {
                    await CreateNotes(NotesManagerType.Offline, note);
                    var boundList = (DataContext as NotesViewModel);
                    boundList.Notes.Add(note);
                }
            }
            foreach (var note in conflictNotes)
            {
                await UpdateNotes(NotesManagerType.Online, note);
            }
            foreach (var note in unSyncedNotes.Where(a => a.Id == 0))
            {
                var cloudNote = await CreateNotes(NotesManagerType.Online, note);
                DeleteNotes(NotesManagerType.Offline, note);
                CreateNotes(NotesManagerType.Offline, cloudNote);
                //Let the screen flickr for new connection to cloud.
                offlineNotes = await GetNotes(NotesManagerType.Offline);
                selectedIndex = list.SelectedIndex > 0 ? list.SelectedIndex : 0;
                DataContext = offlineNotes;
                list.SelectedIndex = selectedIndex;
            }
          
            
           
        }


        private async Task<NotesViewModel> GetNotes(NotesManagerType type)
        {
            var notesManager = NotesFatory.GetManager(type);
            lblSync.Content = "Trying to load the notes from " + notesManager.GetSource();
            var notes = await notesManager.GetNotes(userId);
            lblSync.Content = "";
            return new NotesViewModel(notes);
        }
        private async Task<NoteViewModel> GetNote(NotesManagerType type, NoteViewModel note)
        {
            var notesManager = NotesFatory.GetManager(type);
            var notes = await notesManager.GetNote(note);
            return notes;
        }
        private async Task<NoteViewModel> CreateNotes(NotesManagerType type, NoteViewModel note)
        {
            var notesManager = NotesFatory.GetManager(type);
            lblSync.Content = "Adding new notes to " + notesManager.GetSource();
            var notes = await notesManager.CreateNote(note);
            lblSync.Content = "";
            return notes;
        }
        private async Task<NoteViewModel> UpdateNotes(NotesManagerType type, NoteViewModel note)
        {
            var notesManager = NotesFatory.GetManager(type);
            lblSync.Content = "Updating notes to " + notesManager.GetSource();
            var notes = await notesManager.UpdateNote(note);
            lblSync.Content = "";
            return notes;
        }
        private async Task<NoteViewModel> DeleteNotes(NotesManagerType type, NoteViewModel note)
        {
            var notesManager = NotesFatory.GetManager(type);
            lblSync.Content = "Deleting notes from " + notesManager.GetSource();
            var notes = await notesManager.DeleteNote(note);
            lblSync.Content = "";
            return notes;
        }


        private void List_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtTitle.IsEnabled = txtData.IsEnabled = true;
        }
    }
}
