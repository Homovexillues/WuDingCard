using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace WuDingCard;

public partial class DailyNewsPage:ContentPage
{
	public ObservableCollection<string> DailyNews { get; set; }

	public DailyNewsPage() {
		InitializeComponent();
		DailyNews = [];
		this.BindingContext = this;
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
		var uri = $@"https://www.zhihu.com/api/v4/columns/c_1715391799055720448/items?limit=1";
		client.DefaultRequestHeaders.Add("Cookie","_zap=ed155700-5a7c-4b66-90da-ad8d43b623c8; d_c0=ACCd_sEOYhiPTkqQ-XuBM-IAnCCxQj-rnVg=|1711677397; q_c1=64c0ea40306541ffa97b0d26fcb1753f|1714444533000|1714444533000; _xsrf=f7Mr1DWHnacT1BxyCzStIjX5iY0XGJ68; Hm_lvt_98beee57fd2ef70ccdd5ca52b9740c49=1733977057,1735547916; HMACCOUNT=9CBDCBC7ECDFC06A; __zse_ck=004_ieKMtSiFFIrofF0Z7a3aZwY1n2CMxZ8kF1lKVypHee6UmJJ5ShKu/HllmBovKV3644GzJvCN8=v/49fIsl1TcMA=IdBj/vBeTo125MiiCaSfguRAjdvLTtYP8=f8Nql3-6Es/64AJMwS6mK3hhnJklYylb39fsiYML4vCaDQ0XOZckz002PM0Rm4h6j4Qea2jtzuBK7s8mT43RvswT5YQWykl4aKXuHYCQ9LHcaznl/TIULIytVO7yx0e/uyVdSDQ; Hm_lpvt_98beee57fd2ef70ccdd5ca52b9740c49=1735614913; z_c0=2|1:0|10:1735614911|4:z_c0|80:MS4xUVRtYUV3QUFBQUFtQUFBQVlBSlZUUVdxWDJoNHRNa0lJTGFRbXBGZnlNWUZ4ZnZiazdqdFRBPT0=|33feaa2a6f5370e762d9a14b5b5959b54d885eb10608af3272a67800b857ce71; BEC=69a31c4b51f80d1feefe6d6caeac6056");
		client.DefaultRequestHeaders.Add("User-Agent","Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Mobile Safari/537.36 Edg/131.0.0.0");
		var jsonContent = await client.GetFromJsonAsync<Root>(uri,Options);
		var content = jsonContent.Data[0].Content;
		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(content);
		var paragraphs = htmlDoc.DocumentNode.SelectNodes("//p").Where(p => !string.IsNullOrEmpty(p.InnerText));
		DailyNews.Clear();
		foreach(var paragraph in paragraphs) {
			DailyNews.Add(paragraph.InnerText);
		}
	}

	#region Poco类

	public class Root
	{
		public ArticleData[] Data { get; set; }
	}

	public class ArticleData
	{
		public string Content { get; set; }
	}

	#endregion Poco类

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}

	internal JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions {
		PropertyNameCaseInsensitive = true, // 忽略大小写
		Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 防止特殊字符转义
	};
}