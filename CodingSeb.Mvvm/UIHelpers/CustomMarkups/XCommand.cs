using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// To Bind all kind of UI event or commands to a command or a method call in the viewModel
    /// </summary>
    public class XCommand : MarkupExtension
    {
        private WeakDictionary<INotifyPropertyChanged, List<string>> PropertiesToBindDict { get; } = new WeakDictionary<INotifyPropertyChanged, List<string>>();
        private List<DependencyPropertyListener> DependencyPropertyListeners { get; } = new List<DependencyPropertyListener>();
        private WeakReference<IRelayCommand> relayCommandReference;
        private WeakReference<FrameworkElement> targetObjectReference;

        [ConstructorArgument("commandOrMethodOrEvaluation")]
        public string CommandOrMethodOrEvaluation { get; set; }

        [ConstructorArgument("canExecuteForMethodOrEvaluation")]
        public string CanExecuteForMethodOrEvaluation { get; set; }

        /// <summary>
        /// if true pass andEventToCommandArgs object with commandparameter event sender and event args, if false just CommandParameter
        /// </summary>
        public bool UseEventToCommandArgs { get; set; }

        private DependencyPropertyListener commandParameterListener;

        public object CommandParameter { get; set; }
        public BindingBase CommandParameterBinding { get; set; }

        public bool CatchEvaluationExceptions { get; set; }

        internal InternalExpressionEvaluatorWithXamlContext Evaluator { get; set; }

        public XCommand(string commandOrMethodOrEvaluation)
        {
            CommandOrMethodOrEvaluation = commandOrMethodOrEvaluation;
        }

        public XCommand(string commandOrMethodOrEvaluation, string canExecuteForMethodOrEvaluation)
        {
            CommandOrMethodOrEvaluation = commandOrMethodOrEvaluation;
            CanExecuteForMethodOrEvaluation = canExecuteForMethodOrEvaluation;
        }

        public XCommand()
        { }

        private void InvokeCommand(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(CommandOrMethodOrEvaluation)
                && sender is FrameworkElement frameworkElement)
            {
                // Find control's ViewModel
                var viewModel = frameworkElement.DataContext;
                if (viewModel != null)
                {
                    Type viewModelType = viewModel.GetType();

                    object parameter = commandParameterListener?.Value ?? CommandParameter;

                    if (viewModelType.GetProperty(CommandOrMethodOrEvaluation)?.GetValue(viewModel) is ICommand command)
                    {
                        object objArg = UseEventToCommandArgs ?
                        new XCommandArgs()
                        {
                            Sender = sender,
                            EventArgs = args,
                            CommandParameter = parameter
                        } :
                        parameter;

                        // Execute Command and pass event arguments as parameter
                        if (command.CanExecute(objArg))
                        {
                            command.Execute(objArg);
                        }
                    }
                    else if (viewModelType
                        .GetMethods()
                        .Where(methodInfo => methodInfo.Name.Equals(CommandOrMethodOrEvaluation))
                        .ToArray() is MethodInfo[] methodInfos && methodInfos.Length > 0)
                    {
                        methodInfos.ToList().ForEach(methodInfo =>
                        {
                            ParameterInfo[] parametersInfos = methodInfo.GetParameters();
                            if (parametersInfos.Length == 0)
                            {
                                methodInfo.Invoke(viewModel, new object[0]);
                            }
                            else if (parametersInfos.Length == 1
                                && (parametersInfos[0].ParameterType == typeof(XCommandArgs) || UseEventToCommandArgs))
                            {
                                methodInfo.Invoke(viewModel, new object[] {
                                    new XCommandArgs()
                                    {
                                        Sender = sender,
                                        EventArgs = args,
                                        CommandParameter = parameter
                                    }});
                            }
                            else if (parametersInfos.Length == 1
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType())))
                            {
                                methodInfo.Invoke(viewModel, new object[] { parameter });
                            }
                            else if (parametersInfos.Length == 2
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType()))
                                && (parametersInfos[1].ParameterType == args.GetType() || parametersInfos[1].ParameterType.IsAssignableFrom(args.GetType())))
                            {
                                methodInfo.Invoke(viewModel, new object[] { sender, args });
                            }
                            else if (parametersInfos.Length == 3
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType()))
                                && (parametersInfos[1].ParameterType == args.GetType() || parametersInfos[1].ParameterType.IsAssignableFrom(args.GetType()))
                                && (parameter == null || parametersInfos[2].ParameterType == parameter.GetType() || parametersInfos[2].ParameterType.IsAssignableFrom(parameter.GetType())))
                            {
                                methodInfo.Invoke(viewModel, new object[] { sender, args, parameter });
                            }
                        });
                    }
                    else if(CanExecute(sender))
                    {
                        try
                        {
                            Evaluator.Variables["Sender"] = sender;
                            Evaluator.Variables["EventArgs"] = args;
                            Evaluator.Variables["CommandParameter"] = parameter;

                            Evaluator.ScriptEvaluate(CommandOrMethodOrEvaluation);
                        }
                        catch when (CatchEvaluationExceptions)
                        { }
                        finally
                        {
                            Evaluator.Variables.Clear();
                        }
                    }
                }
            }
        }

        private bool CanExecute(object sender)
        {
            if (!string.IsNullOrEmpty(CommandOrMethodOrEvaluation)
                && sender is FrameworkElement frameworkElement)
            {                // Find control's ViewModel
                var viewModel = frameworkElement.DataContext;
                object parameter = commandParameterListener?.Value ?? CommandParameter;
                if (viewModel != null)
                {
                    Type viewModelType = viewModel.GetType();

                    if (viewModelType.GetProperty(CommandOrMethodOrEvaluation)?.GetValue(viewModel) is ICommand command)
                    {
                        object objArg = UseEventToCommandArgs ?
                        new XCommandArgs()
                        {
                            Sender = sender,
                            EventArgs = null,
                            CommandParameter = parameter
                        } :
                        parameter;

                        // Execute Command and pass event arguments as parameter
                        return command.CanExecute(objArg);
                    }
                    else if (CanExecuteForMethodOrEvaluation != null && viewModelType.GetProperty(CanExecuteForMethodOrEvaluation)?.GetValue(viewModel) is bool canExecute)
                    {
                        return canExecute;
                    }
                }

                if (CanExecuteForMethodOrEvaluation != null)
                {
                    try
                    {
                        if (!frameworkElement.IsLoaded)
                        {
                            WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler(frameworkElement, nameof(FrameworkElement.Loaded), FrameworkElement_Loaded);
                            return false;
                        }

                        PropertiesToBindDict.Keys.ToList()
                            .ForEach(notifyPropertyChanged => WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged));
                        DependencyPropertyListeners.ForEach(listener => listener.Dispose());
                        PropertiesToBindDict.Clear();
                        DependencyPropertyListeners.Clear();

                        Evaluator.Variables["Sender"] = sender;
                        Evaluator.Variables["CommandParameter"] = parameter;

                        Evaluator.PreEvaluateVariable += Evaluator_PreEvaluateVariable;

                        return (bool)Evaluator.ScriptEvaluate(CanExecuteForMethodOrEvaluation);
                    }
                    catch
                    { }
                    finally
                    {
                        Evaluator.Variables.Clear();
                        Evaluator.PreEvaluateVariable -= Evaluator_PreEvaluateVariable;
                    }
                }
            }

            return true;
        }

        private void Evaluator_PreEvaluateVariable(object sender, ExpressionEvaluator.VariablePreEvaluationEventArg args)
        {
            if (args.This is INotifyPropertyChanged notifyPropertyChanged)
            {
                WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged);
                if (!PropertiesToBindDict.ContainsKey(notifyPropertyChanged))
                    PropertiesToBindDict[notifyPropertyChanged] = new List<string>();

                PropertiesToBindDict[notifyPropertyChanged].Add(args.Name);
            }
            else if (args.This is DependencyObject dependencyObject)
            {
                var dependencyPropertyListener = new DependencyPropertyListener(dependencyObject, new PropertyPath(args.Name));
                dependencyPropertyListener.Changed += DependencyPropertyListener_Changed;
                DependencyPropertyListeners.Add(dependencyPropertyListener);
            }
        }

        private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement frameworkElement)
                WeakEventManager<FrameworkElement, RoutedEventArgs>.RemoveHandler(frameworkElement, nameof(FrameworkElement.Loaded), FrameworkElement_Loaded);

            RefreshCanExecute();
        }

        private void DependencyPropertyListener_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            RefreshCanExecute();
        }

        private void RelayCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            RefreshCanExecute();
        }

        private void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is INotifyPropertyChanged notifyPropertyChanged
                && PropertiesToBindDict.ContainsKey(notifyPropertyChanged)
                && PropertiesToBindDict[notifyPropertyChanged].Contains(e.PropertyName))
            {
                RefreshCanExecute();
            }
        }

        private void RefreshCanExecute()
        {
            IRelayCommand relayCommand = null;
            FrameworkElement frameworkElement = null;

            if (relayCommandReference?.TryGetTarget(out relayCommand) ?? false)
            {
                relayCommand.RaiseCanExecuteChanged();
            }
            else if(targetObjectReference?.TryGetTarget(out frameworkElement) ?? false)
            {
                frameworkElement.IsEnabled = CanExecute(frameworkElement);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
                || !(service.TargetObject is DependencyObject targetObject)
                || service.TargetProperty == null)
            {
                return this;
            }

            // Retrieve a reference to the InvokeCommand helper method declared below, using reflection
            MethodInfo invokeCommand = GetType().GetMethod(nameof(InvokeCommand), BindingFlags.Instance | BindingFlags.NonPublic);
            if (invokeCommand != null)
            {
                FrameworkElement frameworkElement = targetObject as FrameworkElement;
                targetObjectReference = new WeakReference<FrameworkElement>(frameworkElement);
                object viewModel = frameworkElement?.DataContext;

                Evaluator = new InternalExpressionEvaluatorWithXamlContext(viewModel, serviceProvider)
                {
                    TargetObject = targetObject,
                    OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
                };

                if (CommandParameterBinding != null)
                {
                    commandParameterListener = new DependencyPropertyListener(CommandParameterBinding, targetObject);
                }

                if (viewModel.GetType().GetProperty(CommandOrMethodOrEvaluation)?.GetValue(viewModel) is ICommand command)
                {
                    WeakEventManager<ICommand, EventArgs>.AddHandler(command, nameof(ICommand.CanExecuteChanged), RelayCommand_CanExecuteChanged);
                    if(viewModel is INotifyPropertyChanged commandNotifyPropertyChanged)
                    {
                        WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(commandNotifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged);
                        if (!PropertiesToBindDict.ContainsKey(commandNotifyPropertyChanged))
                            PropertiesToBindDict[commandNotifyPropertyChanged] = new List<string>();

                        PropertiesToBindDict[commandNotifyPropertyChanged].Add(CommandOrMethodOrEvaluation);
                    }

                }

                if (CanExecuteForMethodOrEvaluation != null
                    && viewModel is INotifyPropertyChanged notifyPropertyChanged
                    && (!PropertiesToBindDict.ContainsKey(notifyPropertyChanged)
                        || !PropertiesToBindDict[notifyPropertyChanged].Contains(CanExecuteForMethodOrEvaluation)))
                {
                    WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged);
                    if (!PropertiesToBindDict.ContainsKey(notifyPropertyChanged))
                        PropertiesToBindDict[notifyPropertyChanged] = new List<string>();

                    PropertiesToBindDict[notifyPropertyChanged].Add(CanExecuteForMethodOrEvaluation);
                }

                if (service.TargetProperty is EventInfo eventInfo)
                {
                    frameworkElement.IsEnabled = CanExecute(frameworkElement);
                    // If the context is an event, simply return the helper method as delegate
                    // (this delegate will be invoked when the event fires)
                    var eventHandlerType = eventInfo.EventHandlerType;
                    return invokeCommand.CreateDelegate(eventHandlerType, this);
                }
                else if (service.TargetProperty is MethodInfo methodInfo)
                {
                    frameworkElement.IsEnabled = CanExecute(frameworkElement);
                    // Some events are represented as method calls with 2 parameters:
                    // The first parameter is the control that acts as the event's sender,
                    // the second parameter is the actual event handler
                    var methodParameters = methodInfo.GetParameters();
                    if (methodParameters.Length == 2)
                    {
                        var eventHandlerType = methodParameters[1].ParameterType;
                        return invokeCommand.CreateDelegate(eventHandlerType, this);
                    }
                }
                else if (service.TargetProperty is DependencyProperty dependencyProperty && dependencyProperty.PropertyType == typeof(ICommand))
                {
                    IRelayCommand relayCommand;

                    if (CanExecuteForMethodOrEvaluation != null)
                    {
                        relayCommand = new RelayCommand(_ => InvokeCommand(targetObject, EventArgs.Empty), _ => CanExecute(targetObject));
                        relayCommandReference = new WeakReference<IRelayCommand>(relayCommand);
                        return relayCommand;
                    }
                    else
                    {
                        return new RelayCommand(_ => InvokeCommand(targetObject, EventArgs.Empty));
                    }
                }
            }
            throw new InvalidOperationException("The EventBinding markup extension is valid only in the context of events or commands.");
        }
    }
}
