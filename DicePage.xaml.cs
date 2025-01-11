using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WuDingCard;

public partial class DicePage:ContentPage, INotifyPropertyChanged
{
	//#region  Ù–‘
	private string _diceCount;

	private string _sidedCount;
	private string _result;
	private string _buttonText;

	public string DiceCount {
		get => _diceCount;
		set {
			if(_diceCount != value) {
				_diceCount = value;
				OnPropertyChanged();
				UpdateButtonText();
			}
		}
	}

	public string SidedCount {
		get => _sidedCount;
		set {
			if(_sidedCount != value) {
				_sidedCount = value;
				OnPropertyChanged();
				UpdateButtonText();
			}
		}
	}

	public string Result {
		get => _result;
		set {
			if(_result != value) {
				_result = value;
				OnPropertyChanged();
			}
		}
	}

	public string ButtonText {
		get => _buttonText;
		set {
			if(_buttonText != value) {
				_buttonText = value;
				OnPropertyChanged();
				UpdateButtonText();
			}
		}
	}

	//#endregion  Ù–‘
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
	}

	public DicePage() {
		InitializeComponent();
		BindingContext = this;
		UpdateButtonText();
	}

	private void UpdateButtonText() {
		ButtonText = $"{DiceCount ?? ""}D{SidedCount ?? ""}=";
	}

	private void RollCustomizeDice(Object sender,EventArgs e) {
		Random random = new();
		OutputResult.Text = string.Empty;
		_ = int.TryParse(DiceCount,out int diceCount) ? diceCount : 0;
		_ = int.TryParse(SidedCount,out int sidedCount) ? sidedCount : 0;
		int sumNumber = 0;
		for(int i = 0;i < diceCount;i++) {
			int randomNumber = random.Next(1,sidedCount + 1);
			OutputResult.Text += $"{randomNumber}";
			if(i != diceCount - 1) OutputResult.Text += "+";
			sumNumber += randomNumber;
		}
		OutputResult.Text += $"={sumNumber}";
		SemanticScreenReader.Announce(OutputResult.Text);
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

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}