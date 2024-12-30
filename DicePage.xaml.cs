namespace WuDingCard;

public partial class DicePage : ContentPage
{
	public DicePage()
	{
		InitializeComponent();
	}
	private void RollOneDiceFour(object sender,EventArgs e) {
		Random random = new();
		OneDiceFour.Text = $"1D4={random.Next(1,5)}";
		SemanticScreenReader.Announce(OneDiceFour.Text);
	}
	private void RollOneDiceSix(object sender,EventArgs e) {
		Random random = new();
		OneDiceSix.Text = $"1D6={random.Next(1,7)}";
		SemanticScreenReader.Announce(OneDiceSix.Text);
	}
	private void RollOneDiceEight(object sender,EventArgs e) {
		Random random = new();
		OneDiceEight.Text = $"1D8={random.Next(1,9)}";
		SemanticScreenReader.Announce(OneDiceEight.Text);
	}
	private void RollOneDiceTen(object sender,EventArgs e) {
		Random random = new();
		OneDiceTen.Text = $"1D10={random.Next(1,11)}";
		SemanticScreenReader.Announce(OneDiceTen.Text);
	}
	private void RollOneDiceTwenty(object sender,EventArgs e) {
		Random random = new();
		OneDiceTwenty.Text = $"1D20={random.Next(1,21)}";
		SemanticScreenReader.Announce(OneDiceTwenty.Text);
	}
	private void RollOneDiceHundred(object sender,EventArgs e) {
		Random random = new();
		OneDiceHundred.Text = $"1D100={random.Next(1,101)}";
		SemanticScreenReader.Announce(OneDiceHundred.Text);
	}
}