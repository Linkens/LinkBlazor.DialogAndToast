using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LinkBlazor
{
    public class ValidatorService(ToastService Toast) {

        public bool IsModelValid<T>(T model, string ErrorMessage = "Error", int Duration = 10000, Action? OnError = null) {
            if (model is null) return false;
            var context = new ValidationContext(model!, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model!, context, validationResults, true);
            if (validationResults.Count > 0) {
                Toast.Toast(ToastSeverity.Error, ErrorMessage, string.Join("\r\n", validationResults.Select(v => v.ErrorMessage! )), duration: Duration);
                OnError?.Invoke();
                return false;
            }
            return true;
        }
    }
    public class ValidatorService<M>(ToastService Toast, IStringLocalizer<M> Localizer) {

        public bool IsModelValid<T>(T model, string ErrorMessage = "Error", int Duration = 10000, Action? OnError = null) {
            if (model is null) return false;
            var context = new ValidationContext(model!, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model!, context, validationResults, true);
            if (validationResults.Count > 0) {
                Toast.Toast(ToastSeverity.Error, Localizer[ErrorMessage], string.Join("\r\n", validationResults.Select(v => Localizer[v.ErrorMessage!])), duration: Duration);
                OnError?.Invoke();
                return false;
            }
            return true;
        }
    }
}
