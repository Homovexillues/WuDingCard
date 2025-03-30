using SQLite;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WuDingCard;

public partial class LibraryPage:ContentPage
{
	public ObservableCollection<Notebook> Notebooks { get; }

	public LibraryPage() {
		InitializeComponent();
		Notebooks = new();
		BindingContext = this;
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await LoadNotebooks(); // 改为异步方法
	}

	private async Task LoadNotebooks() {
		try {
			var notbooks = await QuickPlaySpell.Database.Dbcnn.Table<Notebook>().ToListAsync();
			Notebooks.Clear();
			foreach(var notebook in notbooks) {
				Notebooks.Add(notebook);
			}
		}
		catch(Exception ex) {
			throw new Exception(ex.Message);
		}
	}

	private async void EditNoteBook(object sender,EventArgs e) {
		if(sender is Border border) {
			var tapGesture = border.GestureRecognizers.OfType<TapGestureRecognizer>().FirstOrDefault();
			if(tapGesture != null && int.TryParse(tapGesture.CommandParameter?.ToString(),out int notebookId)) {
				await Navigation.PushAsync(new NotebookPage(notebookId));
			}
		}
	}

	private async void DeleteNoteBook(object sender,EventArgs e) {
		bool delete = await DisplayAlert("警告","此操作将删除本笔记本,是否确定?","确定","取消");
		if(delete) {
		}
	}

	private async void AddNotebook(object sender,EventArgs e) {
		string title = await DisplayPromptAsync("新建笔记本","请输入标题");
		if(!string.IsNullOrWhiteSpace(title)) {
			var newNotebook = new Notebook { Title = title };
			await QuickPlaySpell.Database.Dbcnn.InsertAsync(newNotebook);
			Notebooks.Add(newNotebook); // 直接添加到ObservableCollection
		}
	}
}

internal interface ICRUD
{
	public async void Add() { }

	public async void Delete() { }

	public async void Edit() { }

	public async void Find() { }
}

public class Notebook
{
	[PrimaryKey, AutoIncrement]
	public int NotebookId { get; set; }

	[NotNull]
	public string Title { get; set; }

	public string Description { get; set; }

	public string CreatedDate { get; set; } = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
}