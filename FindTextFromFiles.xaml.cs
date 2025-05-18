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
				// 选择文件
				var options = new PickOptions {
					PickerTitle = "请选择要搜索的文件"
				};

				var result = await FilePicker.PickMultipleAsync(options);
				if(result != null && result.Count() > 0) {
					PathEntry.Text = string.Join(", ",result.Select(f => f.FileName));
					// 存储完整路径以供后续使用
					PathEntry.AutomationId = string.Join("|",result.Select(f => f.FullPath));
				}
			}
			else {
				// 选择文件夹
#if WINDOWS
                    var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                    folderPicker.FileTypeFilter.Add("*");

                    // 获取当前窗口句柄
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
				await DisplayAlert("提示","当前平台不支持文件夹选择功能","确定");
#endif
			}
		}
		catch(Exception ex) {
			await DisplayAlert("错误",$"选择文件时出错: {ex.Message}","确定");
		}
	}

	private async void OnSearchClicked(object sender,EventArgs e) {
		if(string.IsNullOrWhiteSpace(SearchContentEntry.Text)) {
			await DisplayAlert("提示","请输入要搜索的内容","确定");
			return;
		}

		if(string.IsNullOrWhiteSpace(PathEntry.Text)) {
			await DisplayAlert("提示","请选择文件或文件夹","确定");
			return;
		}

		_searchResults.Clear();
		SearchButton.IsEnabled = false;
		SearchButton.Text = "搜索中...";

		try {
			var searchContent = SearchContentEntry.Text;
			var extensions = ParseFileExtensions(FileExtensionsEntry.Text);

			if(FileRadioButton.IsChecked) {
				// 搜索选定的文件
				var filePaths = PathEntry.AutomationId.Split('|');
				foreach(var filePath in filePaths) {
					if(ShouldProcessFile(filePath,extensions)) {
						await SearchInFile(filePath,searchContent);
					}
				}
			}
			else {
				// 搜索文件夹
				var folderPath = PathEntry.AutomationId;
				var includeSubFolders = IncludeSubFoldersCheckBox.IsChecked;

				await SearchInFolder(folderPath,searchContent,extensions,includeSubFolders);
			}
		}
		catch(Exception ex) {
			await DisplayAlert("错误",$"搜索时出错: {ex.Message}","确定");
		}
		finally {
			SearchButton.IsEnabled = true;
			SearchButton.Text = "开始搜索";
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
			System.Diagnostics.Debug.WriteLine($"读取文件 {filePath} 时出错: {ex.Message}");
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

			var action = await DisplayActionSheet("文件操作","取消",null,"打开文件","打开所在文件夹");

			switch(action) {
				case "打开文件":
					OpenFile(selectedResult.FilePath);
					break;

				case "打开所在文件夹":
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
			DisplayAlert("错误",$"无法打开文件: {ex.Message}","确定");
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
			DisplayAlert("错误",$"无法打开文件夹: {ex.Message}","确定");
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