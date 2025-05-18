using System;
using Microsoft.Maui.Controls;

namespace WuDingCard;

public partial class ScientificCalculator:ContentPage
{
	private double currentValue = 0;
	private double memoryValue = 0;
	private string pendingOperation = "";
	private bool isNewCalculation = true;
	private int currentBase = 10; // 默认十进制

	public ScientificCalculator() {
		InitializeComponent();
		UpdateDisplay(currentValue);
		UpdateHexButtons();
	}

	private void NumberButton_Clicked(object sender,EventArgs e) {
		Button button = (Button)sender;
		string digit = button.Text;

		if(isNewCalculation) {
			DisplayTextBox.Text = digit;
			isNewCalculation = false;
		}
		else {
			DisplayTextBox.Text += digit;
		}
	}

	private void HexButton_Clicked(object sender,EventArgs e) {
		if(currentBase == 16) {
			Button button = (Button)sender;
			string digit = button.Text;

			if(isNewCalculation) {
				DisplayTextBox.Text = digit;
				isNewCalculation = false;
			}
			else {
				DisplayTextBox.Text += digit;
			}
		}
	}

	private void OperatorButton_Clicked(object sender,EventArgs e) {
		Button button = (Button)sender;
		string operation = button.Text;

		if(!isNewCalculation) {
			CalculateResult();
		}

		pendingOperation = operation;
		isNewCalculation = true;
	}

	private void EqualsButton_Clicked(object sender,EventArgs e) {
		CalculateResult();
		pendingOperation = "";
		isNewCalculation = true;
	}

	private void CalculateResult() {
		double newValue = ConvertFromCurrentBase(DisplayTextBox.Text);

		switch(pendingOperation) {
			case "+":
				currentValue += newValue;
				break;

			case "-":
				currentValue -= newValue;
				break;

			case "*":
				currentValue *= newValue;
				break;

			case "/":
				if(newValue != 0)
					currentValue /= newValue;
				else
					DisplayAlert("错误","除数不能为零！","确定");
				break;

			default:
				currentValue = newValue;
				break;
		}

		UpdateDisplay(currentValue);
	}

	private void ScientificButton_Clicked(object sender,EventArgs e) {
		Button button = (Button)sender;
		string function = button.Text;
		double value = ConvertFromCurrentBase(DisplayTextBox.Text);

		try {
			switch(function) {
				case "sin":
					currentValue = Math.Sin(value * Math.PI / 180); // 角度转弧度
					break;

				case "cos":
					currentValue = Math.Cos(value * Math.PI / 180);
					break;

				case "tan":
					currentValue = Math.Tan(value * Math.PI / 180);
					break;

				case "log":
					if(value <= 0)
						throw new ArgumentException("对数函数的参数必须为正数");
					currentValue = Math.Log10(value);
					break;

				case "ln":
					if(value <= 0)
						throw new ArgumentException("对数函数的参数必须为正数");
					currentValue = Math.Log(value);
					break;

				case "x²":
					currentValue = Math.Pow(value,2);
					break;

				case "x³":
					currentValue = Math.Pow(value,3);
					break;

				case "√":
					if(value < 0)
						throw new ArgumentException("平方根的参数不能为负数");
					currentValue = Math.Sqrt(value);
					break;

				case "∛":
					currentValue = Math.Pow(value,1.0 / 3.0);
					break;

				case "1/x":
					if(value == 0)
						throw new DivideByZeroException("除数不能为零");
					currentValue = 1 / value;
					break;
			}

			UpdateDisplay(currentValue);
			isNewCalculation = true;
		}
		catch(Exception ex) {
			DisplayAlert("错误",$"计算错误: {ex.Message}","确定");
		}
	}

	private void MemoryButton_Clicked(object sender,EventArgs e) {
		Button button = (Button)sender;
		string operation = button.Text;
		double displayValue = ConvertFromCurrentBase(DisplayTextBox.Text);

		switch(operation) {
			case "MC": // Memory Clear
				memoryValue = 0;
				break;

			case "MR": // Memory Recall
				currentValue = memoryValue;
				UpdateDisplay(currentValue);
				isNewCalculation = true;
				break;

			case "MS": // Memory Store
				memoryValue = displayValue;
				isNewCalculation = true;
				break;

			case "M+": // Memory Add
				memoryValue += displayValue;
				isNewCalculation = true;
				break;

			case "M-": // Memory Subtract
				memoryValue -= displayValue;
				isNewCalculation = true;
				break;
		}
	}

	private void ClearButton_Clicked(object sender,EventArgs e) {
		Button button = (Button)sender;
		string operation = button.Text;

		if(operation == "CE") // Clear Entry
		{
			DisplayTextBox.Text = "0";
		}
		else if(operation == "C") // Clear All
		{
			DisplayTextBox.Text = "0";
			currentValue = 0;
			pendingOperation = "";
		}

		isNewCalculation = true;
	}

	private void DecimalButton_Clicked(object sender,EventArgs e) {
		if(currentBase == 10 && !DisplayTextBox.Text.Contains(".")) {
			if(isNewCalculation) {
				DisplayTextBox.Text = "0.";
				isNewCalculation = false;
			}
			else {
				DisplayTextBox.Text += ".";
			}
		}
	}

	private void BaseRadioButton_CheckedChanged(object sender,CheckedChangedEventArgs e) {
		RadioButton radioButton = sender as RadioButton;

		if(radioButton != null && radioButton.IsChecked) {
			double currentDecimalValue = ConvertFromCurrentBase(DisplayTextBox.Text);

			if(radioButton == DecRadioButton)
				currentBase = 10;
			else if(radioButton == BinRadioButton)
				currentBase = 2;
			else if(radioButton == OctRadioButton)
				currentBase = 8;
			else if(radioButton == HexRadioButton)
				currentBase = 16;

			UpdateDisplay(currentDecimalValue);
			UpdateHexButtons();
		}
	}

	private void UpdateHexButtons() {
		bool enableHexButtons = currentBase == 16;
		ButtonA.IsEnabled = enableHexButtons;
		ButtonB.IsEnabled = enableHexButtons;
		ButtonC.IsEnabled = enableHexButtons;
		ButtonD.IsEnabled = enableHexButtons;
		ButtonE.IsEnabled = enableHexButtons;
		ButtonF.IsEnabled = enableHexButtons;
	}

	private double ConvertFromCurrentBase(string text) {
		try {
			if(currentBase == 10) {
				return double.Parse(text);
			}
			else {
				// 对于非十进制，我们只处理整数部分
				return Convert.ToInt64(text,currentBase);
			}
		}
		catch {
			return 0;
		}
	}

	private void UpdateDisplay(double value) {
		if(currentBase == 10) {
			DisplayTextBox.Text = value.ToString();
		}
		else {
			// 对于非十进制，我们只显示整数部分
			long intValue = (long)value;

			switch(currentBase) {
				case 2:
					DisplayTextBox.Text = Convert.ToString(intValue,2);
					break;

				case 8:
					DisplayTextBox.Text = Convert.ToString(intValue,8);
					break;

				case 16:
					DisplayTextBox.Text = Convert.ToString(intValue,16).ToUpper();
					break;
			}
		}
	}
}