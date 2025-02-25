namespace WuDingCard;

public partial class DailyNewsPage:ContentPage
{
	public DailyNewsPage() {
		InitializeComponent();
	}

	/// <summary>
	/// 通过重写此方法，可以实现在切换到此页面的时候就执行方法，而不用点击触发事件
	/// </summary>
	protected override void OnAppearing() {
		base.OnAppearing();
		GetDailyNews();
	}

	/// <summary>
	/// 获取每日新闻的方法
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
				await DisplayAlert("错误",$"获取图片失败:{response.StatusCode}","确定");
			}
		}
		catch(Exception ex) {
			await DisplayAlert("错误",$"获取图片失败:{ex.Message}","确定");
		}
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}