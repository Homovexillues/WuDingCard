namespace WuDingCard;

public partial class NotePage:ContentPage
{
	public Note Note { get; set; }

	public NotePage(Note note = null) {
		InitializeComponent();
		Note = note ?? new Note();
		BindingContext = this;
	}
}