namespace DatetimePicker.MAUI.Test
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            dtpicker1.Culture = new System.Globalization.CultureInfo("en-US");
            dtpicker2.Culture = new System.Globalization.CultureInfo("en-US");
        }
    }
}