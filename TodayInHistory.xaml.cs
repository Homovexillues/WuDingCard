using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace WuDingCard;

public partial class TodayInHistory:ContentPage
{
	public ObservableCollection<Day> DaysInHistory { get; set; }

	public TodayInHistory() {
		InitializeComponent();
		DaysInHistory = new ObservableCollection<Day>();
		this.BindingContext = this;
	}

	protected override void OnAppearing() {
		base.OnAppearing();
		GetTodayInHistory();
	}

	public async void GetTodayInHistory() {
		var client = new HttpClient();
		var url = @"https://api.03c3.cn/api/history";
		try {
			var response = await client.GetFromJsonAsync<Root>(url);
			if(response is not null) {
				DaysInHistory.Clear();
				foreach(var day in response.Data) {
					DaysInHistory.Add(day);
				}
			}
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"��ȡ����ʧ��:{ex.Message}","ȷ��");
		}
	}

	public class Root
	{
		public int Code { get; set; }
		public string Msg { get; set; }
		public List<Day> Data { get; set; }
	}

	public class Day
	{
		public string Year { get; set; }
		public string Description { get; set; }
	}
}