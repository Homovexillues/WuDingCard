namespace WuDingCard;

public partial class TextToSpeechPage:ContentPage
{
	public TextToSpeechPage() {
		InitializeComponent();
	}

	internal async void TTS(object sender,EventArgs e) {
		string text = InputEditor.Text;
		if(string.IsNullOrEmpty(text)) {
			await DisplayAlert("提示","请输入要合成的文本","确定");
			return;
		}
		await TextToSpeech.SpeakAsync(text);
	}
}