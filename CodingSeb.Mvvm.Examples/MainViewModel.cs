using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace CodingSeb.Mvvm.Examples
{
    public sealed class MainViewModel : NotifyPropertyChangedBaseClass
    {
        public int Value1 { get; set; } = 10;

        public int Value2 { get; set; } = 4;

        public ICommand IncrementValue1Command => new RelayCommand(_ => Value1++);

        public void DecrementValue1()
        {
            Value1--;
        }

        public ICommand IncrementValue2Command => new RelayCommand(_ => Value2++);

        public void DecrementValue2()
        {
            Value2--;
        }

        public void ClickTest()
        {
            Debug.WriteLine("ClickTest()");
        }

        public void ClickTest(object arg)
        {
            Debug.WriteLine($"ClickTest(object arg) : {arg}");
        }

        public void ClickTest(XCommandArgs arg)
        {
            Debug.WriteLine($"ClickTest(XCommandArgs arg) : {arg};{arg.Sender};{arg.EventArgs};{arg.CommandParameter}");
        }

        public void ClickTest(object sender, EventArgs arg)
        {
            Debug.WriteLine($"ClickTest(object sender, EventArgs arg) : {sender};{arg}");
        }

        public void ClickTest(object sender, RoutedEventArgs arg)
        {
            Debug.WriteLine($"ClickTest(object sender, RoutedEventArgs arg) : {sender};{arg}");
        }

        public void ClickTest(object sender, RoutedEventArgs arg, object commandParameter)
        {
            Debug.WriteLine($"ClickTest(object sender, RoutedEventArgs arg, object commandParameter) : {sender};{arg};{commandParameter}");
        }

        public void ClickTest(object sender, RoutedEventArgs arg, string commandParameter)
        {
            Debug.WriteLine($"ClickTest(object sender, RoutedEventArgs arg, string commandParameter) : {sender};{arg};{commandParameter}");
        }

        #region Singleton          

        private static MainViewModel instance;

        public static MainViewModel Instance => instance ?? (instance = new MainViewModel());

        private MainViewModel()
        { }

        #endregion
    }
}
