using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WuDingCard;

public partial class GetIpPage:ContentPage
{
	public ObservableCollection<NetworkInterfaceInfo> InterfaceItems { get; set; }

	public GetIpPage() {
		InitializeComponent();
		InterfaceItems = [];
		this.BindingContext = this; //����ǰҳ���BindingContext��Ϊ�Լ�
	}

	/// <summary>
	/// ��ȡ�豸��������IP��Ϣ
	/// </summary>
	public void GetIps(object sender,EventArgs e) {
		InterfaceItems.Clear();
		var interfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach(var netInterface in interfaces) {
			var ipProps = netInterface.GetIPProperties();
			foreach(var unicastAddr in ipProps.UnicastAddresses) {
				if(unicastAddr.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastAddr.Address)) {
					InterfaceItems.Add(new NetworkInterfaceInfo {
						Name = netInterface.Name,
						Address = $"{unicastAddr.Address}"
					});
				}
			}
		}
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}

	#region Poco��

	public class NetworkInterfaceInfo
	{
		public string Name { get; set; }
		public string Address { get; set; }
	}

	#endregion Poco��
}