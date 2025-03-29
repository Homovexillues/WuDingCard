using SQLite;
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
		Notes.Add(new Note { Title = "�ʼǱ�1",Content = "����1",CreateDate = DateTime.Now });
		Notes.Add(new Note { Title = "�ʼǱ�2",Content = "����2",CreateDate = DateTime.Now });
	}

	private async void AddNewNote(object sender,EventArgs e) {
		await Navigation.PushAsync(new NotePage());
	}

	private async void ViewSections(object sender,EventArgs e) {
		var noteId = (sender as Button)?.CommandParameter as string;
		var note = Notes.FirstOrDefault(n => n.Id == noteId);
		if(note != null) {
			await Navigation.PushAsync(new SectionPage());
		}
	}
}

public class Notebook
{
	[PrimaryKey, AutoIncrement]
	public int NotebookId { get; set; }

	[NotNull]
	public string Title { get; set; }

	public string Description { get; set; }

	[Default(value: "CURRENT_TIMESTAMP")]
	public DateTime CreatedDate { get; set; }
}