using System.Windows;
using System.Windows.Input;
using WpfAppResources = GrapeCity.ActiveReports.Viewer.Wpf.WpfResources;

namespace ActiveReports.Samples.WPFViewer
{
	/// <summary>
	/// Interaction logic for CredentialsDialog.xaml
	/// </summary>
	public partial class CredentialsDialog : Window
	{
		/// <summary>
		/// Allows internal callers to access to user name value entered.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Allows internal callers to access to password value entered.
		/// </summary>
		public string Password { get; private set; }
		private static readonly RoutedCommand OKCommand = new RoutedCommand();
		private static readonly RoutedCommand CancelCommand = new RoutedCommand();

		public CredentialsDialog(string dataSourceName, string prompt)
		{
			InitializeComponent();

			Title = "Credentials";
			CommandBindings.Add(new CommandBinding(OKCommand, OKCommand_Executed));
			OKCommand.InputGestures.Add(new KeyGesture(Key.Enter));
			btnOk.Command = OKCommand;

			CommandBindings.Add(new CommandBinding(CancelCommand, CancelCommand_Executed));
			CancelCommand.InputGestures.Add(new KeyGesture(Key.Escape));
			btnCancel.Command = CancelCommand;

			txtPrompt.Text = prompt;
			Title = string.Format(Title, dataSourceName);
		}

		private void OKCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			UserName = txtUser.Text;
			Password = txtPassword.Password;
			DialogResult = true;
			Close();
		}

		private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
