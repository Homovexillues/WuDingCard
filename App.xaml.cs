namespace WuDingCard;

public partial class App:Application
{
	public App() {
		InitializeComponent();
		Task.Run(async () => { await QuickPlaySpell.Database.InitializeAsync(); });
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