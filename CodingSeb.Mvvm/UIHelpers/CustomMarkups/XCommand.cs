using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// To Bind all kind of UI event or commands to a command or a method call in the viewModel
    /// </summary>
    public class XCommand : MarkupExtension
    {
        [ConstructorArgument("commandOrMethodOrEvaluation")]
        public string CommandOrMethodOrEvaluation { get; set; }

        /// <summary>
        /// if true pass andEventToCommandArgs object with commandparameter event sender and event args, if false just CommandParameter
        /// </summary>
        public bool UseEventToCommandArgs { get; set; }

        public object CommandParameter { get; set; }

        public bool CatchEvaluationExceptions { get; set; }

        internal InternalExpressionEvaluatorWithXamlContext Evaluator { get; set; }

        public XCommand(string commandOrMethodOrEvaluation)
        {
            CommandOrMethodOrEvaluation = commandOrMethodOrEvaluation;
        }

        public XCommand()
        {}

        private void InvokeCommand(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(CommandOrMethodOrEvaluation)
                && sender is FrameworkElement frameworkElement)
            {
                // Find control's ViewModel
                var viewmodel = frameworkElement.DataContext;
                if (viewmodel != null)
                {
                    Type viewModelType = viewmodel.GetType();

                    if (viewModelType.GetProperty(CommandOrMethodOrEvaluation)?.GetValue(viewmodel) is ICommand command)
                    {
                        object objArg = UseEventToCommandArgs ?
                        new XCommandArgs()
                        {
                            Sender = sender,
                            EventArgs = args,
                            CommandParameter = CommandParameter
                        } :
                        CommandParameter;

                        // Execute Command and pass event arguments as parameter
                        if (command.CanExecute(objArg))
                        {
                            command.Execute(objArg);
                        }
                    }
                    else if(viewModelType
                        .GetMethods()
                        .Where(methodInfo => methodInfo.Name.Equals(CommandOrMethodOrEvaluation))
                        .ToArray() is MethodInfo[] methodInfos && methodInfos.Length > 0)
                    {
                        methodInfos.ToList().ForEach(methodInfo =>
                        {
                            ParameterInfo[] parametersInfos = methodInfo.GetParameters();
                            if (parametersInfos.Length == 0)
                            {
                                methodInfo.Invoke(viewmodel, new object[0]);
                            }
                            else if (parametersInfos.Length == 1
                                && (parametersInfos[0].ParameterType == typeof(XCommandArgs) || UseEventToCommandArgs))
                            {
                                methodInfo.Invoke(viewmodel, new object[] {
                                    new XCommandArgs()
                                    {
                                        Sender = sender,
                                        EventArgs = args,
                                        CommandParameter = CommandParameter
                                    }});
                            }
                            else if (parametersInfos.Length == 1
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType())))
                            {
                                methodInfo.Invoke(viewmodel, new object[] { CommandParameter });
                            }
                            else if(parametersInfos.Length == 2
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType()))
                                && (parametersInfos[1].ParameterType == args.GetType() || parametersInfos[1].ParameterType.IsAssignableFrom(args.GetType())))
                            {
                                methodInfo.Invoke(viewmodel, new object[] { sender, args});
                            }
                            else if(parametersInfos.Length == 3
                                && (parametersInfos[0].ParameterType == sender.GetType() || parametersInfos[0].ParameterType.IsAssignableFrom(sender.GetType()))
                                && (parametersInfos[1].ParameterType == args.GetType() || parametersInfos[1].ParameterType.IsAssignableFrom(args.GetType()))
                                && (CommandParameter == null || parametersInfos[2].ParameterType == CommandParameter.GetType() || parametersInfos[2].ParameterType.IsAssignableFrom(CommandParameter.GetType())))
                            {
                                methodInfo.Invoke(viewmodel, new object[] { sender, args, CommandParameter});
                            }
                        });
                    }
                    else
                    {
                        try
                        {
                            Evaluator.Variables["Sender"] = sender;
                            Evaluator.Variables["EventArgs"] = args;
                            Evaluator.Variables["CommandParameter"] = CommandParameter;

                            Evaluator.ScriptEvaluate(CommandOrMethodOrEvaluation);
                        }
                        catch when (CatchEvaluationExceptions)
                        {}
                        finally
                        {
                            Evaluator.Variables.Clear();
                        }
                    }
                }
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
                object dataContext = frameworkElement?.DataContext;

                Evaluator = new InternalExpressionEvaluatorWithXamlContext(dataContext, serviceProvider)
                {
                    TargetObject = targetObject,
                    OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
                };

                if (service.TargetProperty is EventInfo eventInfo)
                {
                    // If the context is an event, simply return the helper method as delegate
                    // (this delegate will be invoked when the event fires)
                    var eventHandlerType = eventInfo.EventHandlerType;
                    return invokeCommand.CreateDelegate(eventHandlerType, this);
                }
                else if (service.TargetProperty is MethodInfo methodInfo)
                {
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
                else if(service.TargetProperty is DependencyProperty dependencyProperty && dependencyProperty.PropertyType == typeof(ICommand))
                {
                    return new RelayCommand(_ => InvokeCommand(targetObject, EventArgs.Empty));
                }
            }
            throw new InvalidOperationException("The EventBinding markup extension is valid only in the context of events or commands.");
        }
    }
}
