﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace LinkBlazor {
    public class DialogService : IDisposable {
        private DotNetObjectReference<DialogService>? reference;
        internal DotNetObjectReference<DialogService> Reference
        {
            get {
                reference ??= DotNetObjectReference.Create(this);
                return reference;
            }
        }
        public event EventHandler<DialogServiceNavigationEventArgs> OnNavigate = default!;
        NavigationManager UriHelper { get; set; } = null!;
        IJSRuntime JSRuntime { get; set; }
        public DialogService(NavigationManager uriHelper, IJSRuntime jsRuntime)
        {
            UriHelper = uriHelper;
            JSRuntime = jsRuntime;

            if (UriHelper != null) {
                UriHelper.LocationChanged += UriHelper_OnLocationChanged;
            }
        }

        private void UriHelper_OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            bool Cancel = false;
            while (dialogs.Any() && !Cancel) {
                var dialog = dialogs.LastOrDefault();
                var Args = new DialogServiceNavigationEventArgs(e, dialog);
                OnNavigate?.Invoke(this, Args);
                if (Args.PreventClose) break;
                Close();
            }

        }
        public event Action<dynamic?, DialogOptions?>? OnClose;
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
        public virtual Task<dynamic?> OpenAsync<T>(Dictionary<string, object>? parameters = null, DialogOptions? options = null) where T : ComponentBase
        {
            if (OnOpen == null) throw new ArgumentNullException(nameof(OpenAsync), "<LinkBlazorDialog /> needs to be added to the main layout of your application/site.");
            var Task = new TaskCompletionSource<dynamic?>();
            if (options != null) options.TaskHash = Task.GetHashCode();
            else options = new DialogOptions { TaskHash = Task.GetHashCode() };
            Tasks.Add(Task);

            OpenDialog<T>(parameters, options);

            return Task.Task;
        }
        protected List<object> dialogs = new List<object>();

        private void OpenDialog<T>(Dictionary<string, object>? parameters, DialogOptions? options)
        {
            var NewDialog = new object();
            dialogs.Add(NewDialog);
            OnOpen?.Invoke(typeof(T), parameters, new DialogOptions() {
                Width = options != null && !string.IsNullOrEmpty(options.Width) ? options.Width : "600px",
                Left = options != null && !string.IsNullOrEmpty(options.Left) ? options.Left : "",
                Top = options != null && !string.IsNullOrEmpty(options.Top) ? options.Top : "",
                Bottom = options != null && !string.IsNullOrEmpty(options.Bottom) ? options.Bottom : "",
                Height = options != null && !string.IsNullOrEmpty(options.Height) ? options.Height : "",
                ShowTitle = options != null ? options.ShowTitle || !string.IsNullOrEmpty(options.Title) : true,
                Title = options != null ? options.Title : string.Empty,
                ShowClose = options != null ? options.ShowClose : true,
                CanDrag = options != null ? options.CanDrag : false,
                MultiDialog = options != null ? options.MultiDialog : true,
                Style = options != null ? options.Style : "",
                CloseDialogOnOverlayClick = options != null ? options.CloseDialogOnOverlayClick : false,
                CloseDialogOnEsc = options != null ? options.CloseDialogOnEsc : true,
                CssClass = options != null ? options.CssClass : "",
                WrapperCssClass = options != null ? options.WrapperCssClass : "",
                CloseTabIndex = options != null ? options.CloseTabIndex : 0,
                TaskHash = options != null ? options.TaskHash : 0,
                Resizable = options != null ? options.Resizable : false,
                ZIndex = options != null ? options.ZIndex : null,
                Identifier = options != null ? options.Identifier : null,
                ObjectHash = NewDialog.GetHashCode(),
            });
        }

        /// <summary>
        /// Closes the last opened dialog with optional result.
        /// </summary>
        /// <param name="result">The result.</param>
        [JSInvokable("DialogService.Close")]
        public virtual void Close(dynamic? result = null, DialogOptions? Option = null)
        {
            var dialog = Option == null ? dialogs.LastOrDefault() : dialogs.First(d => d.GetHashCode() == Option.ObjectHash);

            if (dialog != null) {
                OnClose?.Invoke(result, Option);
                dialogs.Remove(dialog);
            }

            var Task = Option == null ? Tasks.LastOrDefault() : Tasks.First(t => t.GetHashCode() == Option.TaskHash);
            if (Task != null && Task.Task != null && !Task.Task.IsCompleted) {
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
    public abstract class DialogOptionsBase {
        public bool ShowTitle { get; set; } = true;
        public bool ShowClose { get; set; } = true;
        public bool CanDrag { get; set; } = false;
        public bool MultiDialog { get; set; } = false;
        public bool Resizable { get; set; } = false;
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? Style { get; set; }
        public bool CloseDialogOnOverlayClick { get; set; } = false;
        public string? CssClass { get; set; }
        public string? WrapperCssClass { get; set; }
        public int CloseTabIndex { get; set; } = 0;
    }
    public class DialogOptions : DialogOptionsBase {
        public string? Left { get; set; }
        public string? Top { get; set; }
        public string? Bottom { get; set; }
        public string? Title { get; set; }
        public int? ZIndex { get; set; }
        public Guid? Identifier { get; set; }
        public bool CloseDialogOnEsc { get; set; } = true;
        internal int TaskHash { get; set; }
        internal int ObjectHash { get; set; }
    }
    public class Dialog {
        public Type Type { get; set; } = default!;
        public Dictionary<string, object>? Parameters { get; set; }
        public DialogOptions Options { get; set; } = null!;
        public int Index { get; set; }
        public int ZIndex { get; set; } = 1001;
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
