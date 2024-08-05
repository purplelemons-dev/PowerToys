﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Awake.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Awake.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checked.
        /// </summary>
        internal static string AWAKE_CHECKED {
            get {
                return ResourceManager.GetString("AWAKE_CHECKED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies whether Awake will be using the PowerToys configuration file for managing the state..
        /// </summary>
        internal static string AWAKE_CMD_HELP_CONFIG_OPTION {
            get {
                return ResourceManager.GetString("AWAKE_CMD_HELP_CONFIG_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Determines whether the display should be kept awake..
        /// </summary>
        internal static string AWAKE_CMD_HELP_DISPLAY_OPTION {
            get {
                return ResourceManager.GetString("AWAKE_CMD_HELP_DISPLAY_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Determines the end date and time when Awake will back off and let the system manage the current sleep and display state..
        /// </summary>
        internal static string AWAKE_CMD_HELP_EXPIRE_AT_OPTION {
            get {
                return ResourceManager.GetString("AWAKE_CMD_HELP_EXPIRE_AT_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bind the execution of Awake to another process. When the process ends, the system will resume managing the current sleep and display state..
        /// </summary>
        internal static string AWAKE_CMD_HELP_PID_OPTION {
            get {
                return ResourceManager.GetString("AWAKE_CMD_HELP_PID_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Determines the interval (in seconds) during which the computer is kept awake..
        /// </summary>
        internal static string AWAKE_CMD_HELP_TIME_OPTION {
            get {
                return ResourceManager.GetString("AWAKE_CMD_HELP_TIME_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exit.
        /// </summary>
        internal static string AWAKE_EXIT {
            get {
                return ResourceManager.GetString("AWAKE_EXIT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Terminating from process binding hook..
        /// </summary>
        internal static string AWAKE_EXIT_BINDING_HOOK_MESSAGE {
            get {
                return ResourceManager.GetString("AWAKE_EXIT_BINDING_HOOK_MESSAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exiting from the internal termination handler..
        /// </summary>
        internal static string AWAKE_EXIT_MESSAGE {
            get {
                return ResourceManager.GetString("AWAKE_EXIT_MESSAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Received a signal to end the process. Making sure we quit....
        /// </summary>
        internal static string AWAKE_EXIT_SIGNAL_MESSAGE {
            get {
                return ResourceManager.GetString("AWAKE_EXIT_SIGNAL_MESSAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} hours.
        /// </summary>
        internal static string AWAKE_HOURS {
            get {
                return ResourceManager.GetString("AWAKE_HOURS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keep awake indefinitely.
        /// </summary>
        internal static string AWAKE_KEEP_INDEFINITELY {
            get {
                return ResourceManager.GetString("AWAKE_KEEP_INDEFINITELY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keep awake on interval.
        /// </summary>
        internal static string AWAKE_KEEP_ON_INTERVAL {
            get {
                return ResourceManager.GetString("AWAKE_KEEP_ON_INTERVAL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keep screen on.
        /// </summary>
        internal static string AWAKE_KEEP_SCREEN_ON {
            get {
                return ResourceManager.GetString("AWAKE_KEEP_SCREEN_ON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keep awake until expiration date and time.
        /// </summary>
        internal static string AWAKE_KEEP_UNTIL_EXPIRATION {
            get {
                return ResourceManager.GetString("AWAKE_KEEP_UNTIL_EXPIRATION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to d.
        /// </summary>
        internal static string AWAKE_LABEL_DAYS {
            get {
                return ResourceManager.GetString("AWAKE_LABEL_DAYS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h.
        /// </summary>
        internal static string AWAKE_LABEL_HOURS {
            get {
                return ResourceManager.GetString("AWAKE_LABEL_HOURS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to m.
        /// </summary>
        internal static string AWAKE_LABEL_MINUTES {
            get {
                return ResourceManager.GetString("AWAKE_LABEL_MINUTES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to s.
        /// </summary>
        internal static string AWAKE_LABEL_SECONDS {
            get {
                return ResourceManager.GetString("AWAKE_LABEL_SECONDS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} minutes.
        /// </summary>
        internal static string AWAKE_MINUTES {
            get {
                return ResourceManager.GetString("AWAKE_MINUTES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Off (keep using the selected power plan).
        /// </summary>
        internal static string AWAKE_OFF {
            get {
                return ResourceManager.GetString("AWAKE_OFF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expiring.
        /// </summary>
        internal static string AWAKE_TRAY_TEXT_EXPIRATION {
            get {
                return ResourceManager.GetString("AWAKE_TRAY_TEXT_EXPIRATION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Indefinite.
        /// </summary>
        internal static string AWAKE_TRAY_TEXT_INDEFINITE {
            get {
                return ResourceManager.GetString("AWAKE_TRAY_TEXT_INDEFINITE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Passive.
        /// </summary>
        internal static string AWAKE_TRAY_TEXT_OFF {
            get {
                return ResourceManager.GetString("AWAKE_TRAY_TEXT_OFF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Interval.
        /// </summary>
        internal static string AWAKE_TRAY_TEXT_TIMED {
            get {
                return ResourceManager.GetString("AWAKE_TRAY_TEXT_TIMED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unchecked.
        /// </summary>
        internal static string AWAKE_UNCHECKED {
            get {
                return ResourceManager.GetString("AWAKE_UNCHECKED", resourceCulture);
            }
        }
    }
}
