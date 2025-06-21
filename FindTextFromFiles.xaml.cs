#if ANDROID

using Android.App.AppSearch;

#endif

using System.Collections.ObjectModel;

namespace WuDingCard;

public partial class FindTextFromFiles:ContentPage
{
	private ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>();

	public FindTextFromFiles() {
		InitializeComponent();
		ResultsCollectionView.ItemsSource = _searchResults;
	}

	private void OnSearchTypeChanged(object sender,CheckedChangedEventArgs e) {
		SubFolderLayout.IsVisible = FolderRadioButton.IsChecked;
	}

	private async void OnBrowseClicked(object sender,EventArgs e) {
		try {
			if(FileRadioButton.IsChecked) {
				// ѡ���ļ�
				var options = new PickOptions {
					PickerTitle = "��ѡ��Ҫ�������ļ�"
				};

				var result = await FilePicker.PickMultipleAsync(options);
				if(result != null && result.Count() > 0) {
					PathEntry.Text = string.Join(", ",result.Select(f => f.FileName));
					// �洢����·���Թ�����ʹ��
					PathEntry.AutomationId = string.Join("|",result.Select(f => f.FullPath));
				}
			}
			else {
				// ѡ���ļ���
#if WINDOWS
                    var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                    folderPicker.FileTypeFilter.Add("*");

                    // ��ȡ��ǰ���ھ��
                    var window = App.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

                    var folder = await folderPicker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        PathEntry.Text = folder.Name;
                        PathEntry.AutomationId = folder.Path;
                    }
#else
				await DisplayAlert("��ʾ","��ǰƽ̨��֧���ļ���ѡ����","ȷ��");
#endif
			}
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"ѡ���ļ�ʱ����: {ex.Message}","ȷ��");
		}
	}

	private async void OnSearchClicked(object sender,EventArgs e) {
		if(string.IsNullOrWhiteSpace(SearchContentEntry.Text)) {
			await DisplayAlert("��ʾ","������Ҫ����������","ȷ��");
			return;
		}

		if(string.IsNullOrWhiteSpace(PathEntry.Text)) {
			await DisplayAlert("��ʾ","��ѡ���ļ����ļ���","ȷ��");
			return;
		}

		_searchResults.Clear();
		SearchButton.IsEnabled = false;
		SearchButton.Text = "������...";

		try {
			var searchContent = SearchContentEntry.Text;
			var extensions = ParseFileExtensions(FileExtensionsEntry.Text);

			if(FileRadioButton.IsChecked) {
				// ����ѡ�����ļ�
				var filePaths = PathEntry.AutomationId.Split('|');
				foreach(var filePath in filePaths) {
					if(ShouldProcessFile(filePath,extensions)) {
						await SearchInFile(filePath,searchContent);
					}
				}
			}
			else {
				// �����ļ���
				var folderPath = PathEntry.AutomationId;
				var includeSubFolders = IncludeSubFoldersCheckBox.IsChecked;

				await SearchInFolder(folderPath,searchContent,extensions,includeSubFolders);
			}
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"����ʱ����: {ex.Message}","ȷ��");
		}
		finally {
			SearchButton.IsEnabled = true;
			SearchButton.Text = "��ʼ����";
		}
	}

	private List<string> ParseFileExtensions(string extensionsText) {
		if(string.IsNullOrWhiteSpace(extensionsText))
			return new List<string>();

		return extensionsText.Split(',',';')
			.Select(ext => ext.Trim())
			.Where(ext => !string.IsNullOrWhiteSpace(ext))
			.Select(ext => ext.StartsWith(".") ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant())
			.ToList();
	}

	private bool ShouldProcessFile(string filePath,List<string> extensions) {
		if(extensions.Count == 0)
			return true;

		var extension = Path.GetExtension(filePath).ToLowerInvariant();
		return extensions.Contains(extension);
	}

	private async Task SearchInFile(string filePath,string searchContent) {
		try {
			var fileContent = await File.ReadAllTextAsync(filePath);
			if(fileContent.Contains(searchContent)) {
				_searchResults.Add(new SearchResult {
					FileName = Path.GetFileName(filePath),
					FilePath = filePath
				});
			}
		}
		catch(Exception ex) {
			System.Diagnostics.Debug.WriteLine($"��ȡ�ļ� {filePath} ʱ����: {ex.Message}");
		}
	}

	private async Task SearchInFolder(string folderPath,string searchContent,List<string> extensions,bool includeSubFolders) {
		var searchOption = includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		var files = Directory.GetFiles(folderPath,"*.*",searchOption);

		foreach(var file in files) {
			if(ShouldProcessFile(file,extensions)) {
				await SearchInFile(file,searchContent);
			}
		}
	}

	private async void OnResultSelected(object sender,SelectionChangedEventArgs e) {
		if(e.CurrentSelection.FirstOrDefault() is SearchResult selectedResult) {
			ResultsCollectionView.SelectedItem = null;

			var action = await DisplayActionSheet("�ļ�����","ȡ��",null,"���ļ�","�������ļ���");

			switch(action) {
				case "���ļ�":
					OpenFile(selectedResult.FilePath);
					break;

				case "�������ļ���":
					OpenFolder(Path.GetDirectoryName(selectedResult.FilePath));
					break;
			}
		}
	}

	private void OpenFile(string filePath) {
		try {
#if WINDOWS
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processStartInfo);
#else
			Launcher.OpenAsync(new Uri($"file://{filePath}"));
#endif
		}
		catch(Exception ex) {
			DisplayAlert("����",$"�޷����ļ�: {ex.Message}","ȷ��");
		}
	}

	private void OpenFolder(string folderPath) {
		try {
#if WINDOWS
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{folderPath}\"",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processStartInfo);
#else
			Launcher.OpenAsync(new Uri($"file://{folderPath}"));
#endif
		}
		catch(Exception ex) {
			DisplayAlert("����",$"�޷����ļ���: {ex.Message}","ȷ��");
		}
	}

	public static void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}

public class SearchResult
{
	public string FileName { get; set; }
	public string FilePath { get; set; }
}