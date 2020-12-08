namespace System.Windows.Input
{
    public class RelayCommand<T> : ICommand
    {
        private EventHandler internalCanExecuteChanged;
        private bool autoCanExecuteRequery;
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Enable or Disable the automatic CanExecute re-query support using the
        /// WPF CommandManager.
        /// </summary>
        public bool AutoCanExecuteRequery
        {
            get { return autoCanExecuteRequery; }
            set
            {
                if (autoCanExecuteRequery != value)
                {
                    autoCanExecuteRequery = value;

                    if (internalCanExecuteChanged != null)
                    {
                        if (autoCanExecuteRequery)
                        {
                            foreach (EventHandler handler in internalCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested += handler;
                            }
                        }
                        else
                        {
                            foreach (EventHandler handler in internalCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested -= handler;
                            }
                        }
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                internalCanExecuteChanged += value;
                if (autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                internalCanExecuteChanged -= value;
                if (autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WPF to re-query the status of this command directly.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (canExecute != null)
                OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            internalCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}