namespace WuDingCard;

public partial class App:Application
{
	public App() {
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) {
		return new Window(new AppShell());
	}

	public static void HandleGlobalSwipe(SwipedEventArgs e) {
		if(e.Direction == SwipeDirection.Right) {
			Shell.Current.FlyoutIsPresented = true;
		}
	}
}