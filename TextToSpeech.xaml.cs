namespace WuDingCard;

public partial class TextToSpeechPage:ContentPage
{
	public TextToSpeechPage() {
		InitializeComponent();
	}

	internal async void TTS(object sender,EventArgs e) {
		string text = InputEditor.Text;
		if(string.IsNullOrEmpty(text)) {
			await DisplayAlert("��ʾ","������Ҫ�ϳɵ��ı�","ȷ��");
			return;
		}
		await TextToSpeech.SpeakAsync(text);
	}
}