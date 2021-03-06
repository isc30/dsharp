﻿using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Delegate for handling generic events.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> object that contains the event data.</param>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public delegate void EventHandler(object sender, EventArgs e);

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public delegate void EventHandler<TArgument>(object sender, TArgument e) 
        where TArgument : EventArgs;
}
