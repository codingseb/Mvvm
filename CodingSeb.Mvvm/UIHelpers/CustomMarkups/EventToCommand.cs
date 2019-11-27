using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// To Bind all kind of UI event to a command in the viewModel
    /// </summary>
    public class EventToCommand : MarkupExtension
    {
        [ConstructorArgument("command")]
        public string Command { get; set; }

        /// <summary>
        /// if true pass andEventToCommandArgs object with commandparameter event sender and event args, if false just CommandParameter
        /// </summary>
        public bool UseEventToCommandArgs { get; set; } = true;

        public object CommandParameter { get; set; }

        public EventToCommand(string command)
        {
            Command = command;
        }

        public EventToCommand()
        {}

        private void InvokeCommand(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(Command) 
                && sender is FrameworkElement control)
            {
                // Find control's ViewModel
                var viewmodel = control.DataContext;
                if (viewmodel != null)
                {
                    // Command must be declared as public property within ViewModel
                    var commandProperty = viewmodel.GetType().GetProperty(Command);
                    if (commandProperty?.GetValue(viewmodel) is ICommand command)
                    {
                        object objArg = UseEventToCommandArgs ?
                        new EventToCommandArgs()
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
                }
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Retrieve a reference to the InvokeCommand helper method declared below, using reflection
            MethodInfo invokeCommand = GetType().GetMethod(nameof(InvokeCommand), BindingFlags.Instance | BindingFlags.NonPublic);
            if (invokeCommand != null)
            {
                // Check if the current context is an event or a method call with two parameters
                if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
                {
                    if (target.TargetProperty is EventInfo eventInfo)
                    {
                        // If the context is an event, simply return the helper method as delegate
                        // (this delegate will be invoked when the event fires)
                        var eventHandlerType = eventInfo.EventHandlerType;
                        return invokeCommand.CreateDelegate(eventHandlerType, this);
                    }
                    else if (target.TargetProperty is MethodInfo methodInfo)
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
                }
            }
            throw new InvalidOperationException("The EventBinding markup extension is valid only in the context of events.");
        }
    }
}
