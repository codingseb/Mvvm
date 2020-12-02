﻿using System;
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
        public bool UseEventToCommandArgs { get; set; } = true;

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
                        methodInfos[0].Invoke(viewmodel, new object[0]);
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
