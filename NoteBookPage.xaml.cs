using SQLite;
using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class NotebookPage:ContentPage
{
	private readonly int _notebookId;
	private readonly Notebook _notebook;
	public string Title { get; set; }
	public ObservableCollection<Section> Sections { get; }

	public NotebookPage(int notebookId) {
		Sections = new();
		_notebookId = notebookId;
		_notebook = QuickPlaySpell.Database.Get<Notebook>(notebookId);
		Title = _notebook.Title;
		InitializeComponent();
		BindingContext = this;
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await LoadSections(_notebookId); // 改为异步方法
	}

	private async Task LoadSections(int notebookId) {
		try {
			var sections = await QuickPlaySpell.Database.Dbcnn.Table<Section>().ToListAsync();
			Sections.Clear();
			foreach(var section in sections.Where(s => int.Equals(s.NotebookId,notebookId))) {
				Sections.Add(section);
			}
		}
		catch(Exception ex) {
			throw new Exception(ex.Message);
		}
	}

	public async void AddSection(object sender,EventArgs e) {
		try {
			var title = await DisplayPromptAsync("新建主题","请输入标题","确认","取消");
			if(!string.IsNullOrWhiteSpace(title)) {
				var note = new Section { NotebookId = _notebookId,Title = title };
				await QuickPlaySpell.Database.Dbcnn.InsertAsync(note);
				Sections.Add(note);
			}
		}
		catch(Exception ex) { throw new Exception(ex.Message,ex); }
	}

	public async void DeleteSection(object sender,EventArgs e) {
		bool delete = await DisplayAlert("警告","此操作将删除本笔记本,是否确定?","确定","取消");
		if(delete) {
			if(sender is Button button && button.CommandParameter is int sectionId) {
				var section = Sections.Where(n => int.Equals(n.SectionId,sectionId));
				await QuickPlaySpell.Database.Dbcnn.DeleteAsync(section);
			}
		}
	}

	public async void EditSection(object sender,EventArgs e) {
		if(sender is Border border) {
			var tapGesture = border.GestureRecognizers.OfType<TapGestureRecognizer>().FirstOrDefault();
			if(tapGesture != null && int.TryParse(tapGesture.CommandParameter?.ToString(),out int sectionId)) {
				await Navigation.PushAsync(new SectionPage(sectionId));
			}
		}
	}

	// 随机颜色生成
	private Color GetRandomColor() {
		Random rand = new Random();
		return Color.FromRgb(rand.Next(256),rand.Next(256),rand.Next(256));
	}
}

public class Section
{
	[PrimaryKey, AutoIncrement]
	public int SectionId { get; set; }

	[NotNull]
	public int NotebookId { get; set; }

	[NotNull]
	public string Title { get; set; }

	public string CreatedDate { get; set; } = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
}