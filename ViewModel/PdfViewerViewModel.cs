using System.Reflection;
using System.ComponentModel;

namespace WuDingCard.ViewModel;

internal class PdfViewerViewModel:INotifyPropertyChanged
{
	private Stream? m_pdfDocumentSream;

	public event PropertyChangedEventHandler PropertyChanged;

	public Stream PdfDocumentStream {
		get => m_pdfDocumentSream;
		set {
			m_pdfDocumentSream = value;
			OnPropertyChanged("PdfDocumentStream");
		}
	}

	public PdfViewerViewModel() {
		m_pdfDocumentSream = typeof(App).GetType().Assembly.GetManifestResourceStream("PdfViewerExample.Assets.PDF_Succinctly.pdf");
	}

	public void OnPropertyChanged(string name) {
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name));
	}
}