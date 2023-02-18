# DateTimePicker.MAUI

A DateTime picker for MAUI that you could custom as you prefer.

You could define styles for all its components:
```C#
    public Style BorderStyle;
    public Style SpinnerSelectionBorderStyle;
    public Style LabelStyle;
    public Style DaySpinnerStyle;
    public Style MonthSpinnerStyle;
    public Style YearSpinnerStyle;
    public Style HourSpinnerStyle;
    public Style MinuteSpinnerStyle;
    public Style SecondSpinnerStyle;
    public Style TimeSpinnerStyle;
    public Style SpinnerItemStyle;
    public Style SeparatorLabelStyle;
    public Style CalendarStyle;
...

You could choose between the diferents picker layouts:

    CenterSpinner
    DownSpinner
    UpSpinner
    CenterCalendar
    DownCalendar
    UpCalendar
    CenterCalendarAndSpinner
    DownCalendarAndSpinner
    UpCalendarAndSpinner

Set the Date and Time formats:
    DMY
    MDY
    YMD
    YDM

    TwentyFourHours
    TwelveHours

And a lot of more properties.

#### Create a DateTimePicker in your code
```C#
using DateTimePicker.MAUI;

...

public DateTimePicker MyDateTime { get; set; } = new DateTimePicker();

...

MyDateTime.SelectionChanged += MyDateTime_SelectionChanged;

...

    private void MyDateTime_SelectionChanged(object sender, EventArgs e)
    {
        Debug.WriteLine($"Selected datetime = {MyDateTime.SelectedDateTime}");
    }

```


#### Add the following xmlns to your page or view
```xaml
xmlns:dtp="clr-namespace:DateTimePicker.MAUI;assembly=DateTimePicker.MAUI"
```

#### Use and custom the DateTimePicker in your xaml
```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:dtp="clr-namespace:DateTimePicker.MAUI;assembly=DateTimePicker.MAUI"
             xmlns:cc="clr-namespace:CalendarView.MAUI;assembly=CalendarView.MAUI"
             x:Class="DatetimePicker.MAUI.Test.MainPage">
    <ContentPage.Resources>
        <Style TargetType="cc:CalendarView" x:Key="CalendarMonthStyle">
            <Setter Property="WidthRequest" Value="220"/>
            <Setter Property="HeightRequest" Value="220"/>
            <Setter Property="FirstDayOfWeekStyle">
                <Setter.Value>
                    <Style TargetType="cc:CalendarDayView">
                        <Setter Property="DayColor" Value="Red"/>
                    </Style>
                </Setter.Value>
            </Setter>
            <Setter Property="BorderStyle">
                <Setter.Value>
                    <Style TargetType="Border">
                        <Setter Property="StrokeThickness" Value="0"/>
                    </Style>
                </Setter.Value>
            </Setter>
        </Style>
    </ContentPage.Resources>
    <Grid HorizontalOptions="Fill" VerticalOptions="Fill">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <dtp:DateTimePicker x:Name="dtpicker1" Grid.Row="0" HorizontalOptions="Center" VerticalOptions="Center"
                            EditHeight="310" NoEditHeight="50" EditWidth="230" NoEditWidth="190" ShowDaySpinner="False" UseAnimation="True" CalendarStyle="{StaticResource Key=CalendarMonthStyle}"
                            ShowMonthSpinner="False" ShowYearSpinner="False" PickerType="CenterCalendarAndSpinner" InputTimeFormat="TwelveHours" DateTimeFormat="MM/dd/yyyy hh:mm:ss"/>
        <dtp:DateTimePicker x:Name="dtpicker2" Grid.Row="1" HorizontalOptions="Start" VerticalOptions="Start" PickerType="UpCalendar" InputDateFormat="MDY" DateTimeFormat="MM/dd/yyyy"/>
    </Grid>

</ContentPage>
```