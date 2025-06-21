using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace WuDingCard;

public partial class NetTools:ContentPage
{
	TcpListener tcpServer;
	TcpClient tcpClient;
	UdpClient udpClient;

	public NetTools() {
		InitializeComponent();
	}

	private async void OnStartClicked(object sender,EventArgs e) {
		string selectedMode = ModePicker.SelectedItem?.ToString();
		string ip = IpEntry.Text;
		int port = int.TryParse(PortEntry.Text,out int parsedPort) ? parsedPort : 0;

		if(string.IsNullOrEmpty(selectedMode) || string.IsNullOrEmpty(ip) || port == 0) {
			await DisplayAlert("����","����д������Ϣ��ѡ��ģʽ","ȷ��");
			return;
		}

		switch(selectedMode) {
			case "TCP Server":
				await StartTcpServer(ip,port);
				break;

			case "TCP Client":
				await StartTcpClient(ip,port);
				break;

			case "UDP":
				await StartUdp(ip,port);
				break;

			default:
				await DisplayAlert("����","δ֪ģʽ","ȷ��");
				break;
		}
	}

	private async void OnSendClicked(object sender,EventArgs e) {
		string selectedMode = ModePicker.SelectedItem?.ToString();
		string dataToSend = DataEditor.Text;

		if(string.IsNullOrEmpty(selectedMode) || string.IsNullOrEmpty(dataToSend)) {
			await DisplayAlert("����","����дҪ���͵����ݲ�ѡ��ģʽ","ȷ��");
			return;
		}

		switch(selectedMode) {
			case "TCP Client":
				await TcpClientSend(dataToSend);
				break;

			case "UDP":
				await UdpSend(dataToSend);
				break;

			default:
				await DisplayAlert("����","��ǰģʽ��֧�ַ�������","ȷ��");
				break;
		}
	}

	private async Task StartTcpServer(string ip,int port) {
		try {
			tcpServer = new TcpListener(IPAddress.Parse(ip),port);
			tcpServer.Start();
			ReceivedDataLabel.Text = $"TCP Server ������������ {ip}:{port}";

			// �ȴ��ͻ�������
			_ = Task.Run(async () => {
				while(true) {
					var client = await tcpServer.AcceptTcpClientAsync();
					using var stream = client.GetStream();
					byte[] buffer = new byte[1024];
					int bytesRead;

					while((bytesRead = await stream.ReadAsync(buffer,0,buffer.Length)) > 0) {
						string received = Encoding.UTF8.GetString(buffer,0,bytesRead);
						MainThread.BeginInvokeOnMainThread(() => {
							ReceivedDataLabel.Text += $"\n����: {received}";
						});
					}
				}
			});
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"�޷����� TCP Server: {ex.Message}","ȷ��");
		}
	}

	private async Task StartTcpClient(string ip,int port) {
		try {
			tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(IPAddress.Parse(ip),port);
			ReceivedDataLabel.Text = $"TCP Client �����ӵ� {ip}:{port}";
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"�޷����ӵ� TCP Server: {ex.Message}","ȷ��");
		}
	}

	private async Task TcpClientSend(string data) {
		try {
			if(tcpClient == null || !tcpClient.Connected) {
				await DisplayAlert("����","TCP Client ��δ����","ȷ��");
				return;
			}

			var stream = tcpClient.GetStream();
			byte[] buffer = Encoding.UTF8.GetBytes(data);
			await stream.WriteAsync(buffer,0,buffer.Length);
			ReceivedDataLabel.Text += $"\n����: {data}";
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"����ʧ��: {ex.Message}","ȷ��");
		}
	}

	private async Task StartUdp(string ip,int port) {
		try {
			udpClient = new UdpClient(port);
			ReceivedDataLabel.Text = $"UDP ģʽ�������������˿� {port}";

			// ��������
			_ = Task.Run(async () => {
				while(true) {
					var result = await udpClient.ReceiveAsync();
					string received = Encoding.UTF8.GetString(result.Buffer);
					MainThread.BeginInvokeOnMainThread(() => {
						ReceivedDataLabel.Text += $"\n����: {received}";
					});
				}
			});
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"�޷����� UDP ģʽ: {ex.Message}","ȷ��");
		}
	}

	private async Task UdpSend(string data) {
		try {
			if(udpClient == null) {
				await DisplayAlert("����","UDP ��δ����","ȷ��");
				return;
			}

			string targetIp = IpEntry.Text;
			int targetPort = int.Parse(PortEntry.Text);

			byte[] buffer = Encoding.UTF8.GetBytes(data);
			await udpClient.SendAsync(buffer,buffer.Length,targetIp,targetPort);
			ReceivedDataLabel.Text += $"\n����: {data}";
		}
		catch(Exception ex) {
			await DisplayAlert("����",$"����ʧ��: {ex.Message}","ȷ��");
		}
	}
}