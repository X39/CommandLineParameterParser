using System;
using System.IO;


namespace CommandLineParameterParser
{
    public struct Command
    {
        /// <summary>
        /// Enum representing the different command types that are available;
        /// </summary>
        public enum EKind
        {
            /// <summary>
            /// <para>Simple flag command. Only one occurance is allowed, no arguments passed.</para>
            /// </summary>
            Flag,
            /// <summary>
            /// <para>Valid path to file. Path will be passed as argument.</para>
            /// </summary>
            Path,
            /// <summary>
            /// Property with a value bound to it. Argument value will be passed to this.
            /// </summary>
            Property
        }

        public EKind Kind;
        public string Name;
        public string DefaultValue;
        public string Description;
        public Action<TextWriter, string> Action;

        private Command(EKind Kind, string Name, string Description, Action<TextWriter, string> Action)
        {
            this.Kind = Kind;
            this.Name = Name;
            this.Action = Action;
            this.DefaultValue = string.Empty;
            this.Description = Description;
        }
        private Command(EKind Kind, string Description, Action<TextWriter, string> Action)
        {
            this.Kind = Kind;
            this.Name = string.Empty;
            this.Action = Action;
            this.DefaultValue = string.Empty;
            this.Description = Description;
        }
        private Command(EKind Kind, string Name, string Description, Action<TextWriter, string> Action, string DefaultValue)
        {
            this.Kind = Kind;
            this.Name = Name;
            this.Action = Action;
            this.DefaultValue = DefaultValue;
            this.Description = Description;
        }

        /// <summary>
        /// Creates a new Flag command.
        /// </summary>
        /// <param name="Name">Name of this flag</param>
        /// <param name="Action">Action for this Command. TextWriter is for any output. string will be empty.</param>
        /// <param name="Description">Description displayed when user requests help.</param>
        /// <returns>New Command with of the kind Flag.</returns>
        public static Command CreateFlag(string Name, Action<TextWriter, string> Action, string Description = "")
        {
            return new Command(EKind.Flag, Name, Description, Action);
        }
        /// <summary>
        /// Creates a new Path command.
        /// It is recommended to only have one of theese.
        /// </summary>
        /// <param name="Action">Action for this Command. TextWriter is for any output. string will contain the path provided.</param>
        /// <param name="Description">Description displayed when user requests help.</param>
        /// <returns>New Command with of the kind Path.</returns>
        public static Command CreatePath(Action<TextWriter, string> Action)
        {
            return new Command(EKind.Path, Description, Action);
        }
        /// <summary>
        /// Creates a new Property command. Property will always trigger if DefaultValue got assigned.
        /// </summary>
        /// <param name="Name">Name of this Property</param>
        /// <param name="Action">Action for this Command. TextWriter is for any output. string will be the assigned value or the default value.</param>
        /// <param name="Description">Description displayed when user requests help.</param>
        /// <param name="DefaultValue">Default value for this command in case it was never added to the arrg list. Empty value means this wont trigger then.</param>
        /// <returns>New Command with of the kind Flag.</returns>
        public static Command CreateProperty(string Name, Action<TextWriter, string> Action, string Description = "", string DefaultValue = "")
        {
            return new Command(EKind.Path, Name, Description, Action, DefaultValue);
        }
    }
}