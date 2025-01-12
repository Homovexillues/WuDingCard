using Markdig;

namespace WuDingCard;

public partial class ViewPdf:ContentPage
{
	public ViewPdf() {
		InitializeComponent();
		//byd，安卓端打开文件的话文件名不能有中文
		//OpenPdfWithDefaultApp("book1.pdf");
	}

	/// <summary>
	/// 用安卓的默认应用打开指定文件
	/// </summary>
	private async void OpenPdfWithDefaultApp(string fileName) {
		var pdfPath = Path.Combine(FileSystem.CacheDirectory,fileName);
		await Launcher.Default.OpenAsync(new OpenFileRequest {
			File = new ReadOnlyFile(pdfPath)
		});
	}

	/// <summary>
	/// 检查应用是否有读写文件权限
	/// </summary>
	/// <returns> </returns>
	private async Task CheckAndRequestPermissionsAsync() {
#if ANDROID
		var readStatus = Android.App.Application.Context.CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage);
		var writeStatus = Android.App.Application.Context.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage);
		if(readStatus != Android.Content.PM.Permission.Granted || writeStatus != Android.Content.PM.Permission.Granted) {
			Platform.CurrentActivity.RequestPermissions(new[] { Android.Manifest.Permission.ReadExternalStorage,Android.Manifest.Permission.WriteExternalStorage },0);
		}
#endif
	}
}