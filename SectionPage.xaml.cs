namespace WuDingCard;

public partial class SectionPage:ContentPage
{
	public SectionPage() {
		InitializeComponent();
	}

	public void OnAddBubbleClicked(object sender,EventArgs e) {
		Navigation.PushAsync(new NotePage());
	}
	// 随机颜色生成
	private Color GetRandomColor() {
		Random rand = new Random();
		return Color.FromRgb(rand.Next(256),rand.Next(256),rand.Next(256));
	}
}