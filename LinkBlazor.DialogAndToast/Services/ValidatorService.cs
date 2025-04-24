using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LinkBlazor
{
    public class ValidatorService {
        private readonly ToastService _toast;
        private readonly ValidatorOptions _options;

        internal static Action<ValidatorOptions>? _optionsModificator;

        public ValidatorService(ToastService toast) {
            _toast = toast;
            _options = new ValidatorOptions();
            _optionsModificator?.Invoke(_options);
        }

        public bool IsModelValid<T>(T model) {
            if (model is null) return false;
            var context = new ValidationContext(model!, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model!, context, validationResults, true);
            if (validationResults.Count > 0) {
                _toast.Toast(ToastSeverity.Error, _options.Localizer?[_options.ErrorMessage] ?? _options.ErrorMessage, string.Join("\r\n", validationResults.Select(v => _options.Localizer?[v.ErrorMessage!] ?? v.ErrorMessage! )), duration:_options.ErrorDuration);
                _options.OnError?.Invoke();
                return false;
            }
            return true;
        }
    }
    public class ValidatorOptions {
        public string ErrorMessage { get; set; } = "Error";
        public int ErrorDuration { get; set; } = 10000;
        public IStringLocalizer? Localizer { get; set; }
        public Action? OnError;
    }
}
