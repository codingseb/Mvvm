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

        #region Singleton          

        private static MainViewModel instance;

        public static MainViewModel Instance => instance ?? (instance = new MainViewModel());

        private MainViewModel()
        { }

        #endregion
    }
}
