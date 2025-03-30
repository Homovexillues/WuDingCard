using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class NotePage:ContentPage
{
	private readonly int _noteId;
	private readonly Note _note;
	public string Content { get; set; }
	public string Title { get; set; }

	public NotePage(int noteId) {
		_noteId = noteId;
		_note = QuickPlaySpell.Database.Get<Note>(noteId);
		Title = _note.Title;
		Content = _note.Content;
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
	}

	public async void SaveNote(object sender,EventArgs e) {
	}
}