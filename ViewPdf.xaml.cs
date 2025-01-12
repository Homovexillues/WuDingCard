using Markdig;

namespace WuDingCard;

public partial class ViewPdf:ContentPage
{
	public ViewPdf() {
		InitializeComponent();
		//byd����׿�˴��ļ��Ļ��ļ�������������
		//OpenPdfWithDefaultApp("book1.pdf");
	}

	/// <summary>
	/// �ð�׿��Ĭ��Ӧ�ô�ָ���ļ�
	/// </summary>
	private async void OpenPdfWithDefaultApp(string fileName) {
		var pdfPath = Path.Combine(FileSystem.CacheDirectory,fileName);
		await Launcher.Default.OpenAsync(new OpenFileRequest {
			File = new ReadOnlyFile(pdfPath)
		});
	}

	/// <summary>
	/// ���Ӧ���Ƿ��ж�д�ļ�Ȩ��
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