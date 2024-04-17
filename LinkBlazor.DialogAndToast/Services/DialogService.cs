using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkBlazor
{
    public class DialogService : IDisposable
    {
        private DotNetObjectReference<DialogService>? reference;
        internal DotNetObjectReference<DialogService> Reference
        {
            get
            {
                reference ??= DotNetObjectReference.Create(this);
                return reference;
            }
        }
        NavigationManager UriHelper { get; set; } = null!;
        IJSRuntime JSRuntime { get; set; }
        public DialogService(NavigationManager uriHelper, IJSRuntime jsRuntime)
        {
            UriHelper = uriHelper;
            JSRuntime = jsRuntime;

            if (UriHelper != null)
            {
                UriHelper.LocationChanged += UriHelper_OnLocationChanged;
            }
        }

        private void UriHelper_OnLocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {

            while (dialogs.Any())
            {
                Close();
            }

        }
        public event Action<dynamic?>? OnClose;
        public event Action? OnRefresh;

        public event Action<Type, Dictionary<string, object>?, DialogOptions>? OnOpen;
        public void Refresh()
        {
            OnRefresh?.Invoke();
        }
        protected List<TaskCompletionSource<dynamic?>> Tasks = new List<TaskCompletionSource<dynamic?>>();

        /// <summary>
        /// Opens a dialog with the specified arguments.
        /// </summary>
        /// <typeparam name="T">The type of the Blazor component which will be displayed in a dialog.</typeparam>
        /// <param name="title">The text displayed in the title bar of the dialog.</param>
        /// <param name="parameters">The dialog parameters. Passed as property values of <typeparamref name="T" />.</param>
        /// <param name="options">The dialog options.</param>
        /// <returns>The value passed as argument to <see cref="Close" />.</returns>
        public virtual Task<dynamic?> OpenAsync<T>(Dictionary<string, object> parameters = null, DialogOptions options = null) where T : ComponentBase
        {
            if (OnOpen == null) throw new ArgumentNullException(nameof(OpenAsync), "<LinkBlazorDialog /> needs to be added to the main layout of your application/site.");
            var Task = new TaskCompletionSource<dynamic?>();

            Tasks.Add(Task);

            OpenDialog<T>(parameters, options);

            return Task.Task;
        }
        protected List<object> dialogs = new List<object>();

        private void OpenDialog<T>(Dictionary<string, object>? parameters, DialogOptions? options)
        {
            dialogs.Add(new object());
            OnOpen?.Invoke(typeof(T), parameters, new DialogOptions()
            {
                Width = options != null && !string.IsNullOrEmpty(options.Width) ? options.Width : "600px",
                Left = options != null && !string.IsNullOrEmpty(options.Left) ? options.Left : "",
                Top = options != null && !string.IsNullOrEmpty(options.Top) ? options.Top : "",
                Bottom = options != null && !string.IsNullOrEmpty(options.Bottom) ? options.Bottom : "",
                Height = options != null && !string.IsNullOrEmpty(options.Height) ? options.Height : "",
                ShowTitle = options != null ? options.ShowTitle || !string.IsNullOrEmpty(options.Title) : true,
                Title = options != null ? options.Title : string.Empty,
                ShowClose = options != null ? options.ShowClose : true,

                Style = options != null ? options.Style : "",
                CloseDialogOnOverlayClick = options != null ? options.CloseDialogOnOverlayClick : false,
                CloseDialogOnEsc = options != null ? options.CloseDialogOnEsc : true,
                CssClass = options != null ? options.CssClass : "",
                WrapperCssClass = options != null ? options.WrapperCssClass : "",
                CloseTabIndex = options != null ? options.CloseTabIndex : 0,
            });
        }

        /// <summary>
        /// Closes the last opened dialog with optional result.
        /// </summary>
        /// <param name="result">The result.</param>
        [JSInvokable("DialogService.Close")]
        public virtual void Close(dynamic? result = null)
        {
            var dialog = dialogs.LastOrDefault();

            if (dialog != null)
            {
                OnClose?.Invoke(result);
                dialogs.Remove(dialog);
            }

            var Task = Tasks.LastOrDefault();
            if (Task != null && Task.Task != null && !Task.Task.IsCompleted)
            {
                Tasks.Remove(Task);
                Task.SetResult(result);
            }
        }
        public void Dispose()
        {
            reference?.Dispose();
            reference = null;

            UriHelper.LocationChanged -= UriHelper_OnLocationChanged;
        }
    }
    public abstract class DialogOptionsBase
    {
        public bool ShowTitle { get; set; } = true;
        public bool ShowClose { get; set; } = true;
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? Style { get; set; }
        public bool CloseDialogOnOverlayClick { get; set; } = false;
        public string? CssClass { get; set; }
        public string? WrapperCssClass { get; set; }
        public int CloseTabIndex { get; set; } = 0;
    }
    public class DialogOptions : DialogOptionsBase
    {
        public string? Left { get; set; }
        public string? Top { get; set; }
        public string? Bottom { get; set; }
        public string? Title { get; set; }
        public bool CloseDialogOnEsc { get; set; } = true;
    }
    public class Dialog
    {
        public Type Type { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public DialogOptions Options { get; set; } = null!;
    }
}
