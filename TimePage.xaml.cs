using System.Timers;
using System.Globalization;
using Timer = System.Timers.Timer;

namespace WuDingCard;

public partial class TimePage:ContentPage
{
	private Timer _timer;

	public TimePage() {
		InitializeComponent();
		StartTimer();
	}

	private void StartTimer() {
		_timer = new Timer(100);
		_timer.Elapsed += UpdateTime;
		_timer.AutoReset = true;
		_timer.Start();
	}

	private void UpdateTime(object sender,ElapsedEventArgs e) {
		DateTime now = DateTime.Now;
		DateTime utcNow = DateTime.UtcNow;
		ChineseLunisolarCalendar chineseLunarCalender = new ChineseLunisolarCalendar();
		int lunarYear = chineseLunarCalender.GetYear(now);
		int lunarMonth = chineseLunarCalender.GetMonth(now);
		int lunarDay = chineseLunarCalender.GetDayOfMonth(now);
		int leapMonth = chineseLunarCalender.GetLeapMonth(lunarYear);
		int lunarHour = chineseLunarCalender.GetHour(now);
		int lunarMinutes = chineseLunarCalender.GetMinute(now);
		int lunarSecond = chineseLunarCalender.GetSecond(now);
		double lunarMillisecond = chineseLunarCalender.GetMilliseconds(now);
		bool isLeapMonth = leapMonth > 0 && lunarMonth == leapMonth;
		MainThread.BeginInvokeOnMainThread(() => {
			DateTimeNowLabel.Text = now.ToString($"��ǰʱ��: yyyy��M��d�� H��m��s��.fff����");
			UTCDateTimeNowLabel.Text = utcNow.ToString($"UTCʱ��: yyyy��M��d�� H��m��s��.fff����");
			LunarDateTimeNowLabel.Text = $"ũ��ʱ��: {lunarYear}��{(isLeapMonth ? "��" : "")}{lunarMonth}��{lunarDay}��{lunarHour}��{lunarMinutes}��{lunarSecond}��{lunarMillisecond}΢��";
		});
	}

	protected override void OnDisappearing() {
		base.OnDisappearing();
		_timer?.Stop();
	}

	protected override void OnAppearing() {
		base.OnAppearing();
		if(_timer == null) {
			StartTimer(); // ���´��� Timer
		}
		else {
			_timer.Start(); // ������ʱ
		}
	}
}