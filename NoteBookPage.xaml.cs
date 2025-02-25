using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class NoteBookPage:ContentPage
{
	public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();

	public NoteBookPage() {
		InitializeComponent();
		BindingContext = this;
		LoadNotes();
	}

	private void LoadNotes() {
		Notes.Add(new Note { Title = "±ãÇ©1",Content = "ÄÚÈÝ1", CreateAt=DateTime.Now});
		Notes.Add(new Note { Title = "±ãÇ©2",Content = "ÄÚÈÝ2",CreateAt = DateTime.Now });
	}

	private async void AddNewNote(object sender,EventArgs e) {
		await Navigation.PushAsync(new NotePage());
	}

	private async void ViewNote(object sender,EventArgs e) {
		var noteId = (sender as Button)?.CommandParameter as string;
		var note = Notes.FirstOrDefault(n => n.Id == noteId);
		if(note != null) {
			await Navigation.PushAsync(new NotePage(note));
		}
	}
}

public class Note
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string Title { get; set; }
	public string Content { get; set; }
	public DateTime CreateAt { get; set; } = DateTime.Now;
}