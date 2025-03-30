using SQLite;
using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class SectionPage:ContentPage
{
	private readonly int _sectionId;
	private readonly Section _section;
	public string Title { get; set; }
	public ObservableCollection<Note> Notes { get; } = [];

	public SectionPage(int sectionId) {
		_sectionId = sectionId;
		_section = QuickPlaySpell.Database.Get<Section>(sectionId);
		Title = _section.Title;
		InitializeComponent();
		BindingContext = this;
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await LoadNotes(_sectionId); // 改为异步方法
	}

	private async Task LoadNotes(int sectionId) {
		try {
			var notes = await QuickPlaySpell.Database.Dbcnn.Table<Note>().ToListAsync();
			Notes.Clear();
			foreach(var note in notes) {
				Notes.Add(note);
			}
		}
		catch(Exception ex) {
			throw new Exception(ex.Message);
		}
	}

	public async void AddNote(object sender,EventArgs e) {
	}

	public async void EditNote(object sender,EventArgs e) {
		if(sender is Border border) {
			var tapGesture = border.GestureRecognizers.OfType<TapGestureRecognizer>().FirstOrDefault();
			if(tapGesture != null && int.TryParse(tapGesture.CommandParameter?.ToString(),out int noteId)) {
				await Navigation.PushAsync(new NotePage(noteId));
			}
		}
	}

	public async void DeleteNote(object sender,EventArgs e) {
	}
}

public class Note
{
	[PrimaryKey, AutoIncrement]
	public int NoteId { get; set; }

	[NotNull]
	public int SectionId { get; set; }

	public string Title { get; set; }
	public string Content { get; set; }
	public string CreatedDate { get; set; } = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
	public string LastModifiedDate { get; set; } = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
}