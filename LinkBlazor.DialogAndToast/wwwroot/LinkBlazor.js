
window.LinkBlazor = {
   
    OpenDialog: function (options, dialogService, dialog) {
        if (LinkBlazor.closeAllPopups) {
            LinkBlazor.closeAllPopups();
        }
        LinkBlazor.dialogService = dialogService;
        if (
          document.documentElement.scrollHeight >
          document.documentElement.clientHeight
        ) {
          document.body.classList.add('lb-no-scroll');
        }

        setTimeout(function () {
            var dialogs = document.querySelectorAll('.lb-dialog-content');
            if (dialogs.length == 0) return;
            var lastDialog = dialogs[dialogs.length - 1];

            if (lastDialog) {
                lastDialog.removeEventListener('keydown', LinkBlazor.FocusTrap);
                lastDialog.addEventListener('keydown', LinkBlazor.FocusTrap);
            }
        }, 500);

        document.removeEventListener('keydown', LinkBlazor.KeyCloseDialog);
        if (options.closeDialogOnEsc) {
            document.addEventListener('keydown', LinkBlazor.KeyCloseDialog);
        }
    },
    CloseDialog: function () {
        LinkBlazor.dialogResizer = null;
        document.body.classList.remove('lb-no-scroll');
        var dialogs = document.querySelectorAll('.lb-dialog-content');
        if (dialogs.length <= 1) {
            document.removeEventListener('keydown', LinkBlazor.KeyCloseDialog);
            delete LinkBlazor.dialogService;
        }
    },
    KeyCloseDialog: function (e) {
        e = e || window.event;
        var isEscape = false;
        if ("key" in e) {
            isEscape = (e.key === "Escape" || e.key === "Esc");
        } else {
            isEscape = (e.keyCode === 27);
        }
        if (isEscape && LinkBlazor.dialogService) {
            LinkBlazor.dialogService.invokeMethodAsync('DialogService.Close', null);

            var dialogs = document.querySelectorAll('.lb-dialog-content');
            if (dialogs.length <= 1) {
                document.removeEventListener('keydown', LinkBlazor.KeyCloseDialog);
                delete LinkBlazor.dialogService;
            }
        }
    },
    DisableKeydown: function (e) {
        e = e || window.event;
        e.preventDefault();
    },
    GetFocusableElements: function (element) {
        return [...element.querySelectorAll('a, button, input, textarea, select, details, iframe, embed, object, summary dialog, audio[controls], video[controls], [contenteditable], [tabindex]')]
            .filter(el => el && el.tabIndex > -1 && !el.hasAttribute('disabled') && el.offsetParent !== null);
    },
    FocusTrap: function (e) {
        e = e || window.event;
        var isTab = false;
        if ("key" in e) {
            isTab = (e.key === "Tab");
        } else {
            isTab = (e.keyCode === 9);
        }
        if (isTab) {
            var focusable = LinkBlazor.GetFocusableElements(e.currentTarget);
            var firstFocusable = focusable[0];
            var lastFocusable = focusable[focusable.length - 1];

            if (firstFocusable && e.shiftKey && document.activeElement === firstFocusable) {
                e.preventDefault();
                firstFocusable.focus();
            } else if (lastFocusable && !e.shiftKey && document.activeElement === lastFocusable) {
                e.preventDefault();
                lastFocusable.focus();
            }
        }
    }

}