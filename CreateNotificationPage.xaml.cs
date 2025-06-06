using System.Collections.ObjectModel;
using System.Text.Json;

namespace WuDingCard;

public partial class CreateNotificationPage:ContentPage
{
	public CreateNotificationPage() {
		InitializeComponent();
		BindingContext = this;
		Notifications.Add(new MyNotification { Title = "²âÊÔÍæÒâ" });
	}

	private async void AddNotification(object sender,EventArgs e) {
		Notifications.Add(new());
	}

	private async void SaveNotification(object sender,EventArgs e) {
	}

	private async void DeleteNotification(object sender,EventArgs e) {
	}

	private async void ViewNotification(object sender,EventArgs e) {
		var notificationId = (sender as Button)?.CommandParameter as string;
		var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
		if(notification != null) {
			await Navigation.PushAsync(new NotificationPage(notification));
		}
	}

	public ObservableCollection<MyNotification> Notifications { get; set; }
}