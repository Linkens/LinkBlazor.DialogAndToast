using System;

namespace LinkBlazor
{
    public class DialogServiceNavigationEventArgs : EventArgs
    {
        public Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs RoutingEvent { protected set; get; }
        public object? ClosingDialog { protected set; get; }
        public bool PreventClose { get; set; }
        public DialogServiceNavigationEventArgs(Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e, object? closingDialog)
        {
            RoutingEvent = e; ClosingDialog = ClosingDialog = closingDialog;
        }
    }
}
