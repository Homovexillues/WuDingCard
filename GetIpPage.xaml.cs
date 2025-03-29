using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace WuDingCard;

public partial class GetIpPage:ContentPage
{
	public ObservableCollection<NetworkInterfaceInfo> InterfaceItems { get; set; }
	public string IpToPing { get; set; }

	public GetIpPage() {
		InitializeComponent();
		InterfaceItems = [];
		this.BindingContext = this; //����ǰҳ���BindingContext��Ϊ�Լ�
		GetIps();
	}

	/// <summary>
	/// ��ȡ�豸��������IP��Ϣ
	/// </summary>
	public void GetIps() {
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

	public void TryPing(object sender,EventArgs e) {
		Ping ping = new Ping();
		if(string.IsNullOrEmpty(IpToPing)) {
			OutputEditor.Text += $"ip��ַ����Ϊ��";
		}
		PingReply reply = ping.Send(IpToPing);
		if(reply.Status == IPStatus.Success) {
			OutputEditor.Text += $"\nPing {IpToPing} �ɹ�";
		}
		else {
			OutputEditor.Text += $"\nPing {IpToPing} ʧ��";
		}
		SemanticScreenReader.Announce(OutputEditor.Text);
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