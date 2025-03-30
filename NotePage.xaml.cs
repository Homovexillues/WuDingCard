using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class NotePage:ContentPage
{
	private readonly int _noteId;
	private readonly Note _note;
	public string NoteContent { get; set; }
	public string NotePageTitle { get; set; }

	public NotePage(int noteId) {
		_noteId = noteId;
		_note = QuickPlaySpell.Database.Get<Note>(noteId);
		NotePageTitle = _note.Title;
		NoteContent = _note.Content;
		InitializeComponent();
		BindingContext = this;
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
	}

	protected override void OnDisappearing() {
		SaveNote();
		base.OnDisappearing();
	}

	public async void SaveNote() {
		await QuickPlaySpell.Database.Dbcnn.UpdateAsync(_note);
	}

	public async void SaveNote(object sender,EventArgs e) {
		_note.Content = NoteContent;
		await QuickPlaySpell.Database.Dbcnn.UpdateAsync(_note);
	}
}