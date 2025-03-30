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
			foreach(var note in notes.Where(n => int.Equals(n.SectionId,sectionId))) {
				Notes.Add(note);
			}
		}
		catch(Exception ex) {
			throw new Exception(ex.Message);
		}
	}

	public async void AddNote(object sender,EventArgs e) {
		try {
			var title = await DisplayPromptAsync("新建笔记","请输入标题","确认","取消");
			if(!string.IsNullOrWhiteSpace(title)) {
				var note = new Note { SectionId = _sectionId,Title = title };
				await QuickPlaySpell.Database.Dbcnn.InsertAsync(note);
				Notes.Add(note);
			}
		}
		catch(Exception ex) { throw new Exception(ex.Message,ex); }
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
		bool delete = await DisplayAlert("警告","此操作将删除本笔记本,是否确定?","确定","取消");
		if(delete) {
			if(sender is Button button && button.CommandParameter is int noteId) {
				var note = Notes.Where(n => int.Equals(n.NoteId,noteId));
				await QuickPlaySpell.Database.Dbcnn.DeleteAsync(note);
			}
		}
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