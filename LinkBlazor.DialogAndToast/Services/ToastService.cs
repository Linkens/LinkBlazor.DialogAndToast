using LinkBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LinkBlazor
{
    public class ToastService
    {
        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>The messages.</value>
        public ObservableCollection<ToastMessage> Messages { get; private set; } = new ObservableCollection<ToastMessage>();

        /// <summary>
        /// Notifies the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Toast(ToastMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!Messages.Contains(message))
            {
                Messages.Add(message);
            }
        }
        public void Toast(ToastSeverity severity = ToastSeverity.Info, string summary = "", string detail = "", double duration = 3000, Action<ToastMessage> click = null, bool closeOnClick = false, object payload = null, Action<ToastMessage> close = null)
        {
            var newMessage = new ToastMessage()
            {
                Duration = duration,
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };

            if (!Messages.Contains(newMessage))
            {
                Messages.Add(newMessage);
            }
        }
    }
    public class ToastMessage : IEquatable<ToastMessage>
    {
        public double? Duration { get; set; } = 3000;
        public ToastSeverity Severity { get; set; } = ToastSeverity.Info;
        public string Summary { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public Action<ToastMessage> Click { get; set; } 
        public Action<ToastMessage> Close { get; set; }
        public bool CloseOnClick { get; set; }
        public object Payload { get; set; }


        public bool Equals(ToastMessage other)
        {
            if(other == null) return false;
            
            if(object.ReferenceEquals(this, other)) return true;

            return this.Severity == other.Severity 
                && this.Summary == other.Summary 
                && this.Detail == other.Detail 
                && this.Duration == other.Duration
                && this.Style == other.Style
                && this.Click == other.Click
                && this.Close == other.Close
                && this.CloseOnClick == other.CloseOnClick
                && this.Payload == other.Payload;
        }

        public override bool Equals(object obj) => Equals(obj as ToastMessage);

        public override int GetHashCode() => (Summary, Detail, Duration, Style, Click, Close, CloseOnClick, Payload).GetHashCode();

        public static bool operator ==(ToastMessage message, ToastMessage otherMessage)
        {
            if (message is null)
            {
                if (otherMessage is null)
                {
                    return true;
                }

                return false;
            }

            return message.Equals(otherMessage);
        }
        public static bool operator !=(ToastMessage message, ToastMessage otherMessage)
        {
            return !(message == otherMessage);
        }

    }
}
