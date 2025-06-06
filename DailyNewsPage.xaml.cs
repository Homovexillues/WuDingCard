namespace WuDingCard;

public partial class DailyNewsPage:ContentPage
{
	public DailyNewsPage() {
		InitializeComponent();
	}

	/// <summary>
	/// ͨ����д�˷���������ʵ�����л�����ҳ���ʱ���ִ�з����������õ�������¼�
	/// </summary>
	protected override void OnAppearing() {
		base.OnAppearing();
		GetDailyNews();
	}

	/// <summary>
	/// ��ȡÿ�����ŵķ���
	/// </summary>
	private async void GetDailyNews() {
		var client = new HttpClient();
		var uri = $@"https://api.03c3.cn/api/zb";
		try {
			var response = await client.GetAsync(uri);
			if(response.IsSuccessStatusCode) {
				var imageBytes = await response.Content.ReadAsByteArrayAsync();
				var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
				NewImage.Source = imageSource;
			}
			else {
				await DisplayAlert("����",$"��ȡͼƬʧ��:{response.StatusCode}","ȷ��");
			}
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"��ȡͼƬʧ��:{ex.Message}","ȷ��");
		}
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}