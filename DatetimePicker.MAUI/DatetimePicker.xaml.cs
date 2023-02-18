using Microsoft.Maui.Controls.Shapes;
using Spinner.MAUI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DateTimePicker.MAUI;

public partial class DateTimePicker : ContentView
{
    //Default styles
    private static bool STYLESINITIATED = false;
    private static readonly Style SPINNERSELECTIONBORDERSTYLE = new(typeof(Border));
    private static readonly Style EDITIMAGESTYLE = new(typeof(Image));
    private static readonly Style OKIMAGESTYLE = new(typeof(Image));
    private static readonly Style CANCELIMAGESTYLE = new(typeof(Image));
    private static readonly Style LABELSTYLE = new(typeof(Label));
    private static readonly Style DAYSPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style MONTHSPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style YEARSPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style HOURSPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style MINUTESPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style SECONDSPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style TIMESPINNERSTYLE = new(typeof(Spinner.MAUI.Spinner));
    private static readonly Style SEPARATORLABELSTYLE = new(typeof(Label));
    private static readonly Style CALENDARSTYLE = new(typeof(CalendarView.MAUI.CalendarView));
    //DateTime properties
    public static readonly BindableProperty SelectedDateTimeProperty = BindableProperty.Create(nameof(SelectedDateTime), typeof(DateTime), typeof(DateTimePicker), DateTime.Now, BindingMode.TwoWay, propertyChanged: SelectedDateChanged);
    public static readonly BindableProperty DateTimeFormatProperty = BindableProperty.Create(nameof(DateTimeFormat), typeof(string), typeof(DateTimePicker), "dd/MM/yyyy HH:mm:ss", propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDate(((DateTimePicker)b).SelectedDateTime); });
    public static readonly BindableProperty InputDateFormatProperty = BindableProperty.Create(nameof(InputDateFormat), typeof(InputDateFormat), typeof(DateTimePicker), InputDateFormat.DMY, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty InputTimeFormatProperty = BindableProperty.Create(nameof(InputTimeFormat), typeof(InputTimeFormat), typeof(DateTimePicker), InputTimeFormat.TwentyFourHours, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetTimeFormat(); });
    public static readonly BindableProperty MinSelectedDateProperty = BindableProperty.Create(nameof(MinSelectedDate), typeof(DateTime), typeof(DateTimePicker), DateTime.Now, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).FillYears(); });
    public static readonly BindableProperty MaxSelectedDateProperty = BindableProperty.Create(nameof(MaxSelectedDate), typeof(DateTime), typeof(DateTimePicker), DateTime.Now.AddYears(10), propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).FillYears(); });
    public static readonly BindableProperty MonthSpinnerFormatProperty = BindableProperty.Create(nameof(MonthSpinnerFormat), typeof(string), typeof(DateTimePicker), "MMM", propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).FillMonths(); });
    public static readonly BindableProperty YearSpinnerFormatProperty = BindableProperty.Create(nameof(YearSpinnerFormat), typeof(string), typeof(DateTimePicker), "yyyy", propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).FillYears(); });
    public static readonly BindableProperty CultureProperty = BindableProperty.Create(nameof(Culture), typeof(CultureInfo), typeof(DateTimePicker), Thread.CurrentThread.CurrentCulture, propertyChanged: (b, o, n) => { if (o != n && n != null && n is CultureInfo ci) ((DateTimePicker)b).FillMonths(); });
    public static readonly BindableProperty DateSeparatorTextProperty = BindableProperty.Create(nameof(DateSeparatorText), typeof(string), typeof(DateTimePicker), "/");
    public static readonly BindableProperty TimeSeparatorTextProperty = BindableProperty.Create(nameof(TimeSeparatorText), typeof(string), typeof(DateTimePicker), ":");

    //Animation properties
    public static readonly BindableProperty EditAtTapProperty = BindableProperty.Create(nameof(EditAtTap), typeof(bool), typeof(DateTimePicker), true);
    public static readonly BindableProperty UseAnimationProperty = BindableProperty.Create(nameof(UseAnimation), typeof(bool), typeof(DateTimePicker), false, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    public static readonly BindableProperty EditWidthProperty = BindableProperty.Create(nameof(EditWidth), typeof(double), typeof(DateTimePicker), -1d, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    public static readonly BindableProperty EditHeightProperty = BindableProperty.Create(nameof(EditHeight), typeof(double), typeof(DateTimePicker), -1d, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    public static readonly BindableProperty NoEditWidthProperty = BindableProperty.Create(nameof(NoEditWidth), typeof(double), typeof(DateTimePicker), -1d, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    public static readonly BindableProperty NoEditHeightProperty = BindableProperty.Create(nameof(NoEditHeight), typeof(double), typeof(DateTimePicker), -1d, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    //Show/hide spinners properties
    public static readonly BindableProperty EditModeProperty = BindableProperty.Create(nameof(EditMode), typeof(bool), typeof(DateTimePicker), false, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetEditMode(); });
    public static readonly BindableProperty ShowDaySpinnerProperty = BindableProperty.Create(nameof(ShowDaySpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowMonthSpinnerProperty = BindableProperty.Create(nameof(ShowMonthSpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowYearSpinnerProperty = BindableProperty.Create(nameof(ShowYearSpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowHourSpinnerProperty = BindableProperty.Create(nameof(ShowYearSpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowMinuteSpinnerProperty = BindableProperty.Create(nameof(ShowMinuteSpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowSecondSpinnerProperty = BindableProperty.Create(nameof(ShowSecondSpinner), typeof(bool), typeof(DateTimePicker), true, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetDateFormat(); });
    public static readonly BindableProperty ShowEditImageProperty = BindableProperty.Create(nameof(ShowEditImage), typeof(bool), typeof(DateTimePicker), true);
    public static readonly BindableProperty ShowOkImageProperty = BindableProperty.Create(nameof(ShowOkImage), typeof(bool), typeof(DateTimePicker), true);
    public static readonly BindableProperty ShowCancelImageProperty = BindableProperty.Create(nameof(ShowCancelImage), typeof(bool), typeof(DateTimePicker), true);
    //Styles properties
    public static readonly BindableProperty PickerTypeProperty = BindableProperty.Create(nameof(PickerType), typeof(DateTimePIckerType), typeof(DateTimePicker), DateTimePIckerType.CenterSpinner, propertyChanged: (b, o, n) => { if (o != n) ((DateTimePicker)b).SetLayout(); });
    public static readonly BindableProperty BorderStyleProperty = BindableProperty.Create(nameof(BorderStyle), typeof(Style), typeof(DateTimePicker), null);
    public static readonly BindableProperty SpinnerSelectionBorderStyleProperty = BindableProperty.Create(nameof(SpinnerSelectionBorderStyle), typeof(Style), typeof(DateTimePicker), SPINNERSELECTIONBORDERSTYLE);
    public static readonly BindableProperty LabelStyleProperty = BindableProperty.Create(nameof(LabelStyle), typeof(Style), typeof(DateTimePicker), LABELSTYLE);
    public static readonly BindableProperty DaySpinnerStyleProperty = BindableProperty.Create(nameof(DaySpinnerStyle), typeof(Style), typeof(DateTimePicker), DAYSPINNERSTYLE);
    public static readonly BindableProperty MonthSpinnerStyleProperty = BindableProperty.Create(nameof(MonthSpinnerStyle), typeof(Style), typeof(DateTimePicker), MONTHSPINNERSTYLE);
    public static readonly BindableProperty YearSpinnerStyleProperty = BindableProperty.Create(nameof(YearSpinnerStyle), typeof(Style), typeof(DateTimePicker), YEARSPINNERSTYLE);
    public static readonly BindableProperty HourSpinnerStyleProperty = BindableProperty.Create(nameof(HourSpinnerStyle), typeof(Style), typeof(DateTimePicker), HOURSPINNERSTYLE);
    public static readonly BindableProperty MinuteSpinnerStyleProperty = BindableProperty.Create(nameof(MinuteSpinnerStyle), typeof(Style), typeof(DateTimePicker), MINUTESPINNERSTYLE);
    public static readonly BindableProperty SecondSpinnerStyleProperty = BindableProperty.Create(nameof(SecondSpinnerStyle), typeof(Style), typeof(DateTimePicker), SECONDSPINNERSTYLE);
    public static readonly BindableProperty TimeSpinnerStyleProperty = BindableProperty.Create(nameof(TimeSpinnerStyle), typeof(Style), typeof(DateTimePicker), TIMESPINNERSTYLE);
    public static readonly BindableProperty SpinnerItemStyleProperty = BindableProperty.Create(nameof(SpinnerItemStyle), typeof(Style), typeof(DateTimePicker));
    public static readonly BindableProperty SeparatorLabelStyleProperty = BindableProperty.Create(nameof(SeparatorLabelStyle), typeof(Style), typeof(DateTimePicker), SEPARATORLABELSTYLE);
    public static readonly BindableProperty CalendarStyleProperty = BindableProperty.Create(nameof(CalendarStyle), typeof(Style), typeof(DateTimePicker), CALENDARSTYLE);
    //Images properties
    public static readonly BindableProperty EditImageProperty = BindableProperty.Create(nameof(EditImage), typeof(ImageSource), typeof(DateTimePicker), ImageSource.FromFile("edit.png"));
    public static readonly BindableProperty EditImageStyleProperty = BindableProperty.Create(nameof(EditImageStyle), typeof(Style), typeof(DateTimePicker), EDITIMAGESTYLE);
    public static readonly BindableProperty OkImageProperty = BindableProperty.Create(nameof(OkImage), typeof(ImageSource), typeof(DateTimePicker), ImageSource.FromFile("ok.png"));
    public static readonly BindableProperty OkImageStyleProperty = BindableProperty.Create(nameof(OkImageStyle), typeof(Style), typeof(DateTimePicker), OKIMAGESTYLE);
    public static readonly BindableProperty CancelImageProperty = BindableProperty.Create(nameof(CancelImage), typeof(ImageSource), typeof(DateTimePicker), ImageSource.FromFile("cancel.png"));
    public static readonly BindableProperty CancelImageStyleProperty = BindableProperty.Create(nameof(CancelImageStyle), typeof(Style), typeof(DateTimePicker), CANCELIMAGESTYLE);

    public DateTime SelectedDateTime { get => (DateTime)GetValue(SelectedDateTimeProperty); set => SetValue(SelectedDateTimeProperty, value); }
    public string DateTimeFormat { get => (string)GetValue(DateTimeFormatProperty); set => SetValue(DateTimeFormatProperty, value); }
    public InputDateFormat InputDateFormat { get => (InputDateFormat)GetValue(InputDateFormatProperty); set => SetValue(InputDateFormatProperty, value); }
    public InputTimeFormat InputTimeFormat { get => (InputTimeFormat)GetValue(InputTimeFormatProperty); set => SetValue(InputTimeFormatProperty, value); }
    public DateTime MinSelectedDate { get => (DateTime)GetValue(MinSelectedDateProperty); set => SetValue(MinSelectedDateProperty, value); }
    public DateTime MaxSelectedDate { get => (DateTime)GetValue(MaxSelectedDateProperty); set => SetValue(MaxSelectedDateProperty, value); }
    public string MonthSpinnerFormat { get => (string)GetValue(MonthSpinnerFormatProperty); set => SetValue(MonthSpinnerFormatProperty, value); }
    public string YearSpinnerFormat { get => (string)GetValue(YearSpinnerFormatProperty); set => SetValue(YearSpinnerFormatProperty, value); }
    public CultureInfo Culture { get => (CultureInfo)GetValue(CultureProperty); set => SetValue(CultureProperty, value); }
    public string DateSeparatorText { get => (string)GetValue(DateSeparatorTextProperty); set => SetValue(DateSeparatorTextProperty, value); }
    public string TimeSeparatorText { get => (string)GetValue(TimeSeparatorTextProperty); set => SetValue(TimeSeparatorTextProperty, value); }

    public bool EditAtTap { get => (bool)GetValue(EditAtTapProperty); set => SetValue(EditAtTapProperty, value); }
    public bool UseAnimation { get => (bool)GetValue(UseAnimationProperty); set => SetValue(UseAnimationProperty, value); }
    public double EditWidth { get => (double)GetValue(EditWidthProperty); set => SetValue(EditWidthProperty, value); }
    public double EditHeight { get => (double)GetValue(EditHeightProperty); set => SetValue(EditHeightProperty, value); }
    public double NoEditWidth { get => (double)GetValue(NoEditWidthProperty); set => SetValue(NoEditWidthProperty, value); }
    public double NoEditHeight { get => (double)GetValue(NoEditHeightProperty); set => SetValue(NoEditHeightProperty, value); }

    public bool EditMode { get => (bool)GetValue(EditModeProperty); set => SetValue(EditModeProperty, value); }
    public bool ShowDaySpinner { get => (bool)GetValue(ShowDaySpinnerProperty); set => SetValue(ShowDaySpinnerProperty, value); }
    public bool ShowMonthSpinner { get => (bool)GetValue(ShowMonthSpinnerProperty); set => SetValue(ShowMonthSpinnerProperty, value); }
    public bool ShowYearSpinner { get => (bool)GetValue(ShowYearSpinnerProperty); set => SetValue(ShowYearSpinnerProperty, value); }
    public bool ShowHourSpinner { get => (bool)GetValue(ShowHourSpinnerProperty); set => SetValue(ShowHourSpinnerProperty, value); }
    public bool ShowMinuteSpinner { get => (bool)GetValue(ShowMinuteSpinnerProperty); set => SetValue(ShowMinuteSpinnerProperty, value); }
    public bool ShowSecondSpinner { get => (bool)GetValue(ShowSecondSpinnerProperty); set => SetValue(ShowSecondSpinnerProperty, value); }
    public bool ShowEditImage { get => (bool)GetValue(ShowEditImageProperty); set => SetValue(ShowEditImageProperty, value); }
    public bool ShowOkImage { get => (bool)GetValue(ShowOkImageProperty); set => SetValue(ShowOkImageProperty, value); }
    public bool ShowCancelImage { get => (bool)GetValue(ShowCancelImageProperty); set => SetValue(ShowCancelImageProperty, value); }

    public DateTimePIckerType PickerType { get => (DateTimePIckerType)GetValue(PickerTypeProperty); set => SetValue(PickerTypeProperty, value); }
    public Style BorderStyle { get => (Style)GetValue(BorderStyleProperty); set => SetValue(BorderStyleProperty, value); }
    public Style SpinnerSelectionBorderStyle { get => (Style)GetValue(SpinnerSelectionBorderStyleProperty); set => SetValue(SpinnerSelectionBorderStyleProperty, value); }
    public Style LabelStyle { get => (Style)GetValue(LabelStyleProperty); set => SetValue(LabelStyleProperty, value); }
    public Style DaySpinnerStyle { get => (Style)GetValue(DaySpinnerStyleProperty); set => SetValue(DaySpinnerStyleProperty, value); }
    public Style MonthSpinnerStyle { get => (Style)GetValue(MonthSpinnerStyleProperty); set => SetValue(MonthSpinnerStyleProperty, value); }
    public Style YearSpinnerStyle { get => (Style)GetValue(YearSpinnerStyleProperty); set => SetValue(YearSpinnerStyleProperty, value); }
    public Style HourSpinnerStyle { get => (Style)GetValue(HourSpinnerStyleProperty); set => SetValue(HourSpinnerStyleProperty, value); }
    public Style MinuteSpinnerStyle { get => (Style)GetValue(MinuteSpinnerStyleProperty); set => SetValue(MinuteSpinnerStyleProperty, value); }
    public Style SecondSpinnerStyle { get => (Style)GetValue(SecondSpinnerStyleProperty); set => SetValue(SecondSpinnerStyleProperty, value); }
    public Style TimeSpinnerStyle { get => (Style)GetValue(TimeSpinnerStyleProperty); set => SetValue(TimeSpinnerStyleProperty, value); }
    public Style SpinnerItemStyle { get => (Style)GetValue(SpinnerItemStyleProperty); set => SetValue(SpinnerItemStyleProperty, value); }
    public Style SeparatorLabelStyle { get => (Style)GetValue(SeparatorLabelStyleProperty); set => SetValue(SeparatorLabelStyleProperty, value); }
    public Style CalendarStyle { get => (Style)GetValue(CalendarStyleProperty); set => SetValue(CalendarStyleProperty, value); }

    public ImageSource EditImage { get => (ImageSource)GetValue(EditImageProperty); set => SetValue(EditImageProperty, value); }
    public Style EditImageStyle { get => (Style)GetValue(EditImageStyleProperty); set => SetValue(EditImageStyleProperty, value); }
    public ImageSource OkImage { get => (ImageSource)GetValue(OkImageProperty); set => SetValue(OkImageProperty, value); }
    public Style OkImageStyle { get => (Style)GetValue(OkImageStyleProperty); set => SetValue(OkImageStyleProperty, value); }
    public ImageSource CancelImage { get => (ImageSource)GetValue(CancelImageProperty); set => SetValue(CancelImageProperty, value); }
    public Style CancelImageStyle { get => (Style)GetValue(CancelImageStyleProperty); set => SetValue(CancelImageStyleProperty, value); }

    /// <summary>
    /// Trigger when user change the selected datetime. The event does not trigger if SelectedDateTime property is set.
    /// </summary>
    public event EventHandler SelectionChanged;
    private ObservableCollection<SpinnerItem> Days { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Months { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Years { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Hours { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Minutes { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Seconds { get; set; } = new ObservableCollection<SpinnerItem>();
    private ObservableCollection<SpinnerItem> Time { get; set; } = new ObservableCollection<SpinnerItem>();
    private int Hour { get; set; }
    private int Minute { get; set; }
    private int Second { get; set; }
    private CultureInfo Checkculture = CultureInfo.CreateSpecificCulture("es-ES");
    private DateTime oldSelected;
    private int displayRow = 0;
    private int controlsRow = 1;
    private bool changingEditMode = false;
    private double spinnersHeight = 75;
    private bool changingTimeMode = false;

    public DateTimePicker()
    {
        DefineDefaultStyles();
        InitializeComponent();

        for (int i = 1; i <= 31; i++)
            Days.Add(new SpinnerItem { Text = i.ToString("D2") });
        MonthsSpn.Items = Months;
        TimeSpn.Items = Time;
        FillMonths();
        FillYears();
        for (int i = 0; i < 24; i++)
            Hours.Add(new SpinnerItem { Text = i.ToString("D2") });
        for (int i = 0; i < 60; i++)
        {
            Minutes.Add(new SpinnerItem { Text = i.ToString("D2") });
            Seconds.Add(new SpinnerItem { Text = i.ToString("D2") });
        }
        HourSpn.Items = Hours;
        MinuteSpn.Items = Minutes;
        SecondSpn.Items = Seconds;
        DaysSpn.Items = Days;

        editImg.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { EditMode = true; }) });
        okImg.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { EditMode = false; }) });
        cancelImg.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { SelectedDateTime = oldSelected; EditMode = false; }) });

        GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { if (EditAtTap) EditMode = !EditMode; }) });
        label.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { if (EditAtTap) EditMode = !EditMode; }) });

        SetDate(SelectedDateTime);
        SetLayout();
        SetEditMode();

        DaysSpn.SelectionChanged += Spn_SelectionChanged;
        MonthsSpn.SelectionChanged += Spn_SelectionChanged;
        YearsSpn.SelectionChanged += Spn_SelectionChanged;
        HourSpn.SelectionChanged += Spn_SelectionChanged;
        MinuteSpn.SelectionChanged += Spn_SelectionChanged;
        SecondSpn.SelectionChanged += Spn_SelectionChanged;
        TimeSpn.SelectionChanged += Spn_SelectionChanged;
        Calendar.SelectionChanged += Spn_SelectionChanged;

        label.SizeChanged += SizesChanged;
        editImg.SizeChanged += SizesChanged;
        okImg.SizeChanged += SizesChanged;
        cancelImg.SizeChanged += SizesChanged;
        DaysSpn.SizeChanged += SizesChanged;
        MonthsSpn.SizeChanged += SizesChanged;
        YearsSpn.SizeChanged += SizesChanged;
        HourSpn.SizeChanged += SizesChanged;
        MinuteSpn.SizeChanged += SizesChanged;
        SecondSpn.SizeChanged += SizesChanged;
        Calendar.SizeChanged += SizesChanged;
    }

    private void SizesChanged(object sender, EventArgs e)
    {
        if (!changingEditMode)
        {
            double height = Math.Max(DaysSpn.ItemHeight, Math.Max(MonthsSpn.ItemHeight, Math.Max(YearsSpn.ItemHeight, Math.Max(HourSpn.ItemHeight, Math.Max(MinuteSpn.ItemHeight, SecondSpn.ItemHeight))))) *
                Math.Max(DaysSpn.NumItemsToShow, Math.Max(MonthsSpn.NumItemsToShow, Math.Max(YearsSpn.NumItemsToShow, Math.Max(HourSpn.NumItemsToShow, Math.Max(MinuteSpn.NumItemsToShow, SecondSpn.NumItemsToShow)))));
            if (height > 0 && height != spinnersHeight) spinnersHeight = height;
            SetVisibility();
        }
    }

    private void Spn_SelectionChanged(object sender, EventArgs e)
    {
        if (!changingTimeMode)
        {
            DateTime date;
            int day, month, year;
            int hour = ShowHourSpinner ? HourSpn.SelectedItemIndex : 0;
            if (InputTimeFormat == InputTimeFormat.TwelveHours)
            {
                hour++;
                if (TimeSpn.SelectedItemIndex == 1)
                {
                    if (hour != 12) hour += 12;
                }
                else
                {
                    if (hour == 12) hour = 0;
                }
            }
            int minute = ShowMinuteSpinner ? MinuteSpn.SelectedItemIndex : 0;
            int second = ShowSecondSpinner ? SecondSpn.SelectedItemIndex : 0;
            if (Calendar.IsVisible && Calendar.SelectedDate != null)
            {
                day = Calendar.SelectedDate.Value.Day;
                month = Calendar.SelectedDate.Value.Month;
                year = Calendar.SelectedDate.Value.Year;
            }
            else
            {
                day = ShowDaySpinner ? DaysSpn.SelectedItemIndex + 1 : 1;
                month = ShowMonthSpinner ? MonthsSpn.SelectedItemIndex + 1 : 1;
                year = ShowYearSpinner ? Convert.ToInt32(Years[YearsSpn.SelectedItemIndex].Text) : 2000;
            }
            while (!DateTime.TryParse(day + "/" + month + "/" + year + " " + hour + ":" + minute + ":" + second, Checkculture, out date))
            {
                day--;
            }
            if (date < MinSelectedDate)
                date = MinSelectedDate;
            else if (date > MaxSelectedDate)
                date = MaxSelectedDate;
            if (SelectedDateTime != date)
            {
                SelectedDateTime = date;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
            else
                SetDate(date);
        }
    }

    private static void SelectedDateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue && newValue is DateTime selectedDate)
        {
            var picker = (DateTimePicker)bindable;
            picker.SetDate(selectedDate);
        }
    }
    private void SetDate(DateTime date)
    {
        if (ShowDaySpinner) DaysSpn.SelectedItemIndex = date.Day - 1;
        if (ShowMonthSpinner) MonthsSpn.SelectedItemIndex = date.Month - 1;
        if (ShowYearSpinner) YearsSpn.SelectedItem = Months.FirstOrDefault(y => y.Text == date.Year.ToString());
        if (ShowHourSpinner)
        {
            if (InputTimeFormat == InputTimeFormat.TwelveHours)
            {
                HourSpn.SelectedItemIndex = Convert.ToInt32(date.ToString("hh")) - 1;
                TimeSpn.SelectedItemIndex = date.ToString("tt", new CultureInfo("en-US")).Contains("AM") ? 0 : 1;
            }
            else
                HourSpn.SelectedItemIndex = date.Hour;
        }
        if (ShowMinuteSpinner) MinuteSpn.SelectedItemIndex = date.Minute;
        if (ShowSecondSpinner) SecondSpn.SelectedItemIndex = date.Second;
        label.Text = date.ToString(DateTimeFormat);
        Calendar.SelectedDate = date;
    }

    private void SetEditMode()
    {

        changingEditMode = true;
        oldSelected = SelectedDateTime;

        if (UseAnimation && EditWidth > 0 && EditHeight > 0 && NoEditWidth > 0 && NoEditHeight > 0)
        {
            editImg.IsVisible = !EditMode && ShowEditImage;
            okImg.IsVisible = EditMode && ShowOkImage;
            cancelImg.IsVisible = EditMode && ShowCancelImage;
            switch (PickerType)
            {
                case DateTimePIckerType.CenterSpinner:
                case DateTimePIckerType.CenterCalendar:
                case DateTimePIckerType.CenterCalendarAndSpinner:
                    LabelStk.Opacity = (EditMode ? 1 : 0);
                    InputsStk.Opacity = (EditMode ? 0 : 1);
                    LabelStk.IsVisible = InputsStk.IsVisible = true;
                    SpinnersSelectionGrid.ColumnDefinitions[1].Width = (okImg.IsVisible ? okImg.WidthRequest + okImg.Margin.Left + okImg.Margin.Right : 0);
                    SpinnersSelectionGrid.ColumnDefinitions[2].Width = (cancelImg.IsVisible ? cancelImg.WidthRequest + cancelImg.Margin.Left + cancelImg.Margin.Right : 0);
                    new Animation
                    {
                        { 0, 1, new Animation (v => border.WidthRequest = v, border.Width, (EditMode?EditWidth:NoEditWidth)) },
                        { 0, 1, new Animation (v => border.HeightRequest = v, border.Height, (EditMode?EditHeight:NoEditHeight)) },
                        { 0, 1, new Animation (v => mainGrid.RowDefinitions[displayRow].Height = v, (EditMode?NoEditHeight:0), (EditMode?0:NoEditHeight)) },
                        { 0, 1, new Animation (v => mainGrid.RowDefinitions[controlsRow].Height = v, (EditMode?0:EditHeight), (EditMode?EditHeight:0)) },
                        { (EditMode?0:0.4), (EditMode?0.6:1), new Animation (v => LabelStk.Opacity = v, (EditMode?1:0), (EditMode?0:1)) },
                        { (EditMode?0.4:0), (EditMode?1:0.6), new Animation (v => InputsStk.Opacity = v, (EditMode?0:1), (EditMode?1:0)) }
                    }.Commit(this, "ChangeEditAnimations", 16, 200, null, (a, b) => { SetVisibility(); changingEditMode = false; });
                    break;
                default:
                    LabelStk.Opacity = 1;
                    InputsStk.Opacity = 1;
                    LabelStk.IsVisible = InputsStk.IsVisible = true;
                    new Animation
                    {
                        { 0, 1, new Animation (v => border.WidthRequest = v, border.Width, (EditMode?EditWidth:NoEditWidth)) },
                        { 0, 1, new Animation (v => border.HeightRequest = v, border.Height, (EditMode?EditHeight:NoEditHeight)) },
                        { 0, 1, new Animation (v => mainGrid.RowDefinitions[controlsRow].Height = v, Math.Max(0, (EditMode?0:EditHeight-NoEditHeight)),  Math.Max(0, (EditMode?EditHeight-NoEditHeight:0))) },
                        { (EditMode?0.4:0), (EditMode?1:0.6), new Animation (v => InputsStk.Opacity = v, (EditMode?0:1), (EditMode?1:0)) }
                    }.Commit(this, "ChangeEditAnimations", 16, 200, null, (a, b) => { SetVisibility(); changingEditMode = false; });
                    break;
            }
        }
        else
        {
            SetVisibility();
            changingEditMode = false;
        }
    }
    private void SetLayout()
    {
        InputsStk.Clear();
        if (SpinnersSelectionGrid.Contains(okImg))
        {
            SpinnersSelectionGrid.Remove(okImg);
            SpinnersSelectionGrid.Remove(cancelImg);
        }
        else
        {
            LabelStk.Remove(okImg);
            LabelStk.Remove(cancelImg);
        }
        if (SpinnersSelectionGrid.Contains(SpinnersGrid))
            SpinnersSelectionGrid.Remove(SpinnersGrid);
        switch (PickerType)
        {
            case DateTimePIckerType.CenterSpinner:
                displayRow = 0;
                controlsRow = 1;
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                SpinnersSelectionGrid.Add(okImg);
                SpinnersSelectionGrid.Add(cancelImg);
                break;
            case DateTimePIckerType.DownSpinner:
                displayRow = 0;
                controlsRow = 1;
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
            case DateTimePIckerType.UpSpinner:
                displayRow = 1;
                controlsRow = 0;
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
            case DateTimePIckerType.CenterCalendar:
                displayRow = 0;
                controlsRow = 1;
                InputsStk.Add(Calendar);
                InputsStk.Add(SpinnersSelectionGrid);
                SpinnersSelectionGrid.Add(okImg);
                SpinnersSelectionGrid.Add(cancelImg);
                break;
            case DateTimePIckerType.DownCalendar:
                displayRow = 0;
                controlsRow = 1;
                InputsStk.Add(Calendar);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
            case DateTimePIckerType.UpCalendar:
                displayRow = 1;
                controlsRow = 0;
                InputsStk.Add(Calendar);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
            case DateTimePIckerType.CenterCalendarAndSpinner:
                displayRow = 0;
                controlsRow = 1;
                InputsStk.Add(Calendar);
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                SpinnersSelectionGrid.Add(okImg);
                SpinnersSelectionGrid.Add(cancelImg);
                break;
            case DateTimePIckerType.DownCalendarAndSpinner:
                displayRow = 0;
                controlsRow = 1;
                InputsStk.Add(Calendar);
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
            case DateTimePIckerType.UpCalendarAndSpinner:
                displayRow = 1;
                controlsRow = 0;
                InputsStk.Add(Calendar);
                SpinnersSelectionGrid.Add(SpinnersGrid);
                InputsStk.Add(SpinnersSelectionGrid);
                LabelStk.Add(okImg);
                LabelStk.Add(cancelImg);
                break;
        }
        Grid.SetRow(LabelStk, displayRow);
        Grid.SetRow(InputsStk, controlsRow);
        SetVisibility();
    }

    private void SetVisibility()
    {
        editImg.IsVisible = !EditMode && ShowEditImage;
        okImg.IsVisible = EditMode && ShowOkImage;
        cancelImg.IsVisible = EditMode && ShowCancelImage;
        LabelStk.Opacity = 1;
        InputsStk.Opacity = 1;
        InputsStk.IsVisible = EditMode;
        if (EditWidth < 0 && NoEditWidth < 0)
            border.WidthRequest = -1;
        if (EditHeight < 0 && NoEditHeight < 0)
            border.HeightRequest = -1;

        double editDisplayHeight = 0;
        double editControlsHeight = 0;
        double editControlsDefinedHeight = EditHeight;
        double okWidth = 0, cancelWidth = 0;
        switch (PickerType)
        {
            case DateTimePIckerType.CenterSpinner:
                Calendar.IsVisible = false;
                editControlsHeight = Math.Max(SpinnersGrid.Height, spinnersHeight + 8d);
                okWidth = (okImg.IsVisible ? okImg.WidthRequest + okImg.Margin.Left + okImg.Margin.Right : 0);
                cancelWidth = (cancelImg.IsVisible ? cancelImg.WidthRequest + cancelImg.Margin.Left + cancelImg.Margin.Right : 0);
                break;
            case DateTimePIckerType.CenterCalendar:
                Calendar.IsVisible = true;
                editControlsHeight = Calendar.HeightRequest;
                if (ShowOkImage)
                    editControlsHeight += okImg.HeightRequest + 8d;
                else if (ShowCancelImage)
                    editControlsHeight += cancelImg.HeightRequest + 8d;
                okWidth = (okImg.IsVisible ? okImg.WidthRequest + okImg.Margin.Left + okImg.Margin.Right : 0);
                cancelWidth = (cancelImg.IsVisible ? cancelImg.WidthRequest + cancelImg.Margin.Left + cancelImg.Margin.Right : 0);
                break;
            case DateTimePIckerType.CenterCalendarAndSpinner:
                Calendar.IsVisible = true;
                editControlsHeight = Calendar.Height;
                if (ShowDaySpinner || ShowMonthSpinner || ShowYearSpinner || ShowHourSpinner || ShowMinuteSpinner || ShowSecondSpinner)
                    editControlsHeight += Math.Max(SpinnersGrid.Height, spinnersHeight + 8d);
                else if (ShowOkImage)
                    editControlsHeight += okImg.Height + 8d;
                else if (ShowCancelImage)
                    editControlsHeight += cancelImg.Height + 8d;
                okWidth = (okImg.IsVisible ? okImg.WidthRequest + okImg.Margin.Left + okImg.Margin.Right : 0);
                cancelWidth = (cancelImg.IsVisible ? cancelImg.WidthRequest + cancelImg.Margin.Left + cancelImg.Margin.Right : 0);
                break;
            case DateTimePIckerType.UpSpinner:
            case DateTimePIckerType.DownSpinner:
                Calendar.IsVisible = false;
                editDisplayHeight = NoEditHeight > 0 ? NoEditHeight : Math.Max(editImg.Height + editImg.Margin.Top + editImg.Margin.Bottom, label.Height + label.Margin.Top + label.Margin.Bottom);
                editControlsDefinedHeight = EditHeight - NoEditHeight;
                SpinnersGrid.HeightRequest = editControlsHeight = Math.Max(SpinnersGrid.Height, spinnersHeight + 8d);
                if (editControlsHeight != SpinnersSelectionGrid.Height) SpinnersSelectionGrid.HeightRequest = editControlsHeight;
                break;
            case DateTimePIckerType.UpCalendar:
            case DateTimePIckerType.DownCalendar:
                Calendar.IsVisible = true;
                editDisplayHeight = NoEditHeight > 0 ? NoEditHeight : Math.Max(editImg.Height + editImg.Margin.Top + editImg.Margin.Bottom, label.Height + label.Margin.Top + label.Margin.Bottom);
                editControlsDefinedHeight = EditHeight - NoEditHeight;
                editControlsHeight = Calendar.Height;
                break;
            default:
                Calendar.IsVisible = true;
                editDisplayHeight = NoEditHeight > 0 ? NoEditHeight : Math.Max(editImg.Height + editImg.Margin.Top + editImg.Margin.Bottom, label.Height + label.Margin.Top + label.Margin.Bottom);
                editControlsDefinedHeight = EditHeight - NoEditHeight;
                SpinnersGrid.HeightRequest = Math.Max(SpinnersGrid.Height, spinnersHeight + 8d);
                editControlsHeight = SpinnersGrid.HeightRequest + Calendar.Height;
                break;
        }
        LabelStk.IsVisible = !EditMode || editDisplayHeight > 0;
        SpinnersSelectionGrid.ColumnDefinitions[1].Width = okWidth;
        SpinnersSelectionGrid.ColumnDefinitions[2].Width = cancelWidth;
        if (EditHeight > 0 && NoEditHeight > 0)
        {
            mainGrid.RowDefinitions[displayRow].Height = EditMode ? editDisplayHeight : NoEditHeight;
            mainGrid.RowDefinitions[controlsRow].Height = EditMode ?  Math.Max(0, editControlsDefinedHeight) : 0;
        }
        else
        {
            //mainGrid.RowDefinitions[displayRow].Height = Math.Max(0, (EditMode ? editDisplayHeight : Math.Max(editImg.Height + editImg.Margin.Top + editImg.Margin.Bottom, label.Height + label.Margin.Top + label.Margin.Bottom)));
            mainGrid.RowDefinitions[displayRow] = EditMode ? new RowDefinition(editDisplayHeight) : new RowDefinition(new GridLength(1, GridUnitType.Star));
            mainGrid.RowDefinitions[controlsRow] = new RowDefinition(Math.Max(0, (EditMode ? editControlsHeight : 0)));
            InputsStk.HeightRequest = -1;
            InputsStk.WidthRequest = -1;
        }
        //border.MinimumHeightRequest = mainGrid.RowDefinitions[displayRow].Height.Value + mainGrid.RowDefinitions[controlsRow].Height.Value;
        InvalidateLayout();
    }
    private void SetDateFormat()
    {
        SpinnersStk.Clear();
        switch(InputDateFormat)
        {
            case InputDateFormat.DMY:
                if (ShowDaySpinner) 
                {
                    SpinnersStk.Add(DaysSpn);
                    if (ShowMonthSpinner || ShowYearSpinner) SpinnersStk.Add(sepLabel1);
                }
                if (ShowMonthSpinner)
                {
                    SpinnersStk.Add(MonthsSpn);
                    if (ShowYearSpinner) SpinnersStk.Add(sepLabel2);
                }
                if (ShowYearSpinner)
                    SpinnersStk.Add(YearsSpn);
                break;
            case InputDateFormat.MDY:
                if (ShowMonthSpinner)
                {
                    SpinnersStk.Add(MonthsSpn);
                    if (ShowDaySpinner || ShowYearSpinner) SpinnersStk.Add(sepLabel1);
                }
                if (ShowDaySpinner)
                {
                    SpinnersStk.Add(DaysSpn);
                    if (ShowYearSpinner) SpinnersStk.Add(sepLabel2);
                }
                if (ShowYearSpinner)
                    SpinnersStk.Add(YearsSpn);
                break;
            case InputDateFormat.YMD:
                if (ShowYearSpinner)
                {
                    SpinnersStk.Add(YearsSpn);
                    if (ShowDaySpinner || ShowMonthSpinner) SpinnersStk.Add(sepLabel1);
                }
                if (ShowMonthSpinner)
                {
                    SpinnersStk.Add(MonthsSpn);
                    if (ShowYearSpinner) SpinnersStk.Add(sepLabel2);
                }
                if (ShowDaySpinner)
                    SpinnersStk.Add(DaysSpn);
                break;
            case InputDateFormat.YDM:
                if (ShowYearSpinner)
                {
                    SpinnersStk.Add(YearsSpn);
                    if (ShowDaySpinner || ShowMonthSpinner) SpinnersStk.Add(sepLabel1);
                }
                if (ShowDaySpinner)
                {
                    SpinnersStk.Add(DaysSpn);
                    if (ShowMonthSpinner) SpinnersStk.Add(sepLabel2);
                }
                if (ShowMonthSpinner)
                    SpinnersStk.Add(MonthsSpn);
                break;
        }
        if ((ShowDaySpinner || ShowMonthSpinner || ShowYearSpinner) && (ShowHourSpinner || ShowMinuteSpinner || ShowSecondSpinner))
            SpinnersStk.Add(sepLabel3);
        if (ShowHourSpinner)
        {
            SpinnersStk.Add(HourSpn);
            if (ShowMinuteSpinner || ShowSecondSpinner) SpinnersStk.Add(sepLabel4);
        }
        if (ShowMinuteSpinner)
        {
            SpinnersStk.Add(MinuteSpn);
            if (ShowSecondSpinner) SpinnersStk.Add(sepLabel5);
        }
        if (ShowSecondSpinner)
            SpinnersStk.Add(SecondSpn);
        SpinnersStk.Add(TimeSpn);
    }

    private void SetTimeFormat()
    {
        changingTimeMode = true;
        Hours.Clear();
        if (InputTimeFormat == InputTimeFormat.TwelveHours)
        {
            for (int i = 1; i <= 12; i++)
                Hours.Add(new SpinnerItem { Text = i.ToString("D2") });
            TimeSpn.IsVisible = true;
            HourSpn.SelectedItemIndex = Convert.ToInt32(SelectedDateTime.ToString("hh")) - 1;
            TimeSpn.SelectedItemIndex = SelectedDateTime.ToString("tt", new CultureInfo("en-US")).Contains("AM") ? 0 : 1;
        }
        else
        {
            for (int i = 0; i < 24; i++)
                Hours.Add(new SpinnerItem { Text = i.ToString("D2") });
            HourSpn.SelectedItemIndex = Convert.ToInt32(SelectedDateTime.ToString("hh"));
            TimeSpn.IsVisible = false;
        }
        changingTimeMode = false;
    }

    private void FillMonths()
    {
        int selIdx = MonthsSpn.SelectedItemIndex;
        Months.Clear();
        DateTime date = new DateTime(2000, 1, 1);
        for (int i = 0; i < 12; i++)
            Months.Add(new SpinnerItem { Text = date.AddMonths(i).ToString(MonthSpinnerFormat, Culture) });
        MonthsSpn.SelectedItemIndex = selIdx;

        selIdx = TimeSpn.SelectedItemIndex;
        Time.Clear();
        Time.Add(new SpinnerItem { Text = string.IsNullOrEmpty(Culture.DateTimeFormat.AMDesignator) ? "AM" : Culture.DateTimeFormat.AMDesignator });
        Time.Add(new SpinnerItem { Text = string.IsNullOrEmpty(Culture.DateTimeFormat.PMDesignator) ? "PM" : Culture.DateTimeFormat.PMDesignator });
        TimeSpn.SelectedItemIndex = selIdx;
    }
    private void FillYears()
    {
        Years.Clear();
        if (MaxSelectedDate >= MinSelectedDate)
        {
            for (int i = 0; i <= MaxSelectedDate.Year - MinSelectedDate.Year; i++)
                Years.Add(new SpinnerItem { Text = MinSelectedDate.AddYears(i).ToString(YearSpinnerFormat) });
            YearsSpn.Items = Years;
        }
    }
    private static void DefineDefaultStyles()
    {
        if (!STYLESINITIATED)
        {
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.Fill);
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(HeightRequestProperty, 35);
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(Border.StrokeProperty, Brush.Gray);
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(BackgroundProperty, (Brush)Color.FromRgba("99999933"));
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(MarginProperty, new Thickness(2));
            SPINNERSELECTIONBORDERSTYLE.Setters.Add(Border.StrokeShapeProperty, new RoundRectangle { CornerRadius = new CornerRadius(10) });

            EDITIMAGESTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.End);
            EDITIMAGESTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            EDITIMAGESTYLE.Setters.Add(MarginProperty, new Thickness(10, 5, 10, 5));
            EDITIMAGESTYLE.Setters.Add(WidthRequestProperty, 32);
            EDITIMAGESTYLE.Setters.Add(HeightRequestProperty, 32);

            OKIMAGESTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.End);
            OKIMAGESTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            OKIMAGESTYLE.Setters.Add(MarginProperty, new Thickness(10, 5, 2, 5));
            OKIMAGESTYLE.Setters.Add(WidthRequestProperty, 32);
            OKIMAGESTYLE.Setters.Add(HeightRequestProperty, 32);
            
            CANCELIMAGESTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.End);
            CANCELIMAGESTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            CANCELIMAGESTYLE.Setters.Add(MarginProperty, new Thickness(2, 5, 10, 5));
            CANCELIMAGESTYLE.Setters.Add(WidthRequestProperty, 32);
            CANCELIMAGESTYLE.Setters.Add(HeightRequestProperty, 32);

            LABELSTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.Start);
            LABELSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            LABELSTYLE.Setters.Add(MarginProperty, new Thickness(5));

            DAYSPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            DAYSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            DAYSPINNERSTYLE.Setters.Add(WidthRequestProperty, 30d);
            MONTHSPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            MONTHSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            MONTHSPINNERSTYLE.Setters.Add(WidthRequestProperty, 40d);
            YEARSPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            YEARSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            YEARSPINNERSTYLE.Setters.Add(WidthRequestProperty, 45d);
            HOURSPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            HOURSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            HOURSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.IsCyclicProperty, true);
            HOURSPINNERSTYLE.Setters.Add(WidthRequestProperty, 30d);
            MINUTESPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            MINUTESPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            MINUTESPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.IsCyclicProperty, true);
            MINUTESPINNERSTYLE.Setters.Add(WidthRequestProperty, 30d);
            SECONDSPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            SECONDSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            SECONDSPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.IsCyclicProperty, true);
            SECONDSPINNERSTYLE.Setters.Add(WidthRequestProperty, 30d);
            TIMESPINNERSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            TIMESPINNERSTYLE.Setters.Add(Spinner.MAUI.Spinner.SelectionBoxIsVisibleProperty, false);
            TIMESPINNERSTYLE.Setters.Add(WidthRequestProperty, 30d);

            SEPARATORLABELSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            SEPARATORLABELSTYLE.Setters.Add(Label.FontAttributesProperty, FontAttributes.Bold);

            CALENDARSTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.Fill);
            CALENDARSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Fill);
            CALENDARSTYLE.Setters.Add(WidthRequestProperty, 220);
            CALENDARSTYLE.Setters.Add(HeightRequestProperty, 220);

            STYLESINITIATED = true;
        }
    }
}