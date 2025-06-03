using System.Security.Cryptography;
using System.Text.Json;

namespace WuDingCard;

public partial class TextFormattingPage:ContentPage
{
	public TextFormattingPage() {
		InitializeComponent();
	}

	private void ConvertToVertical(object sender,EventArgs e) {
		var result = string.Empty;
		foreach(var font in InputEditor.Text) {
			if(char.Equals(font,' ')) continue;
			result += font + "\n";
		}
		OutputEditor.Text = result;
		SemanticScreenReader.Announce(OutputEditor.Text);
	}

	private async void ConvertToMarFont(object sender,EventArgs e) {
		var result = string.Empty;
		using var stream = await FileSystem.OpenAppPackageFileAsync("Resources/Config/MarFont.json");
		using var reader = new StreamReader(stream);
		string configText = await reader.ReadToEndAsync();
		var referenceDic = JsonSerializer.Deserialize<Dictionary<string,string>>(configText);
		foreach(var font in InputEditor.Text) {
			if(!referenceDic.ContainsKey($"{font}")) result += $"{font}";
			else result += referenceDic[$"{font}"];
		}
		OutputEditor.Text = result;
		SemanticScreenReader.Announce(OutputEditor.Text);
	}

	private async void ConvertToMD5Hash(object sender,EventArgs e) {
		OutputEditor.Text = QuickPlaySpell.CalculateMD5(InputEditor.Text);
		SemanticScreenReader.Announce(OutputEditor.Text);
	}

	private async void GenerateStrongPassword(object sender,EventArgs e) {
		const int passwordLength = 16;
		const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[]{}|;:,.<>?";

		using var rng = RandomNumberGenerator.Create();
		var bytes = new byte[passwordLength];
		rng.GetBytes(bytes);

		var passwordChars = new char[passwordLength];
		for(int i = 0;i < passwordLength;i++) {
			passwordChars[i] = validChars[bytes[i] % validChars.Length];
		}

		var password = new string(passwordChars);
		OutputEditor.Text = password;
		SemanticScreenReader.Announce(OutputEditor.Text);
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}