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
		await LoadNotebooks(); // ��Ϊ�첽����
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
		bool delete = await DisplayAlert("����","�˲�����ɾ�����ʼǱ�,�Ƿ�ȷ��?","ȷ��","ȡ��");
		if(delete) {
		}
	}

	private async void AddNotebook(object sender,EventArgs e) {
		string title = await DisplayPromptAsync("�½��ʼǱ�","���������");
		if(!string.IsNullOrWhiteSpace(title)) {
			var newNotebook = new Notebook { Title = title };
			await QuickPlaySpell.Database.Dbcnn.InsertAsync(newNotebook);
			Notebooks.Add(newNotebook); // ֱ����ӵ�ObservableCollection
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