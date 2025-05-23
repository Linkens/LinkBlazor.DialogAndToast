
window.LinkBlazor = {

    OpenDialog: function (options, dialogService, dialog, dialogID) {
        if (LinkBlazor.closeAllPopups) {
            LinkBlazor.closeAllPopups();
        }
        LinkBlazor.dialogService = dialogService;
        if (document.documentElement.scrollHeight > document.documentElement.clientHeight && !options.canDrag) {
            document.body.classList.add('lb-no-scroll');
        }
        if (options.canDrag) {
            LinkBlazor.MakeDragElement(dialogID);
        }
        if (options.multiDialog) {
            var wrapper = document.getElementById(dialogID);
            wrapper.onmousedown = this.BringWrapperToFront;
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

    BringWrapperToFront: function (e) {
        var dialogs = document.querySelectorAll('.lb-dialog');
        let Min = 150000;
        if (dialogs.length == 1) return;
        dialogs.forEach((d) => {
            Min = Math.min(d.style.zIndex, Min)
        });
        dialogs.forEach((d) => {
            if (d.id == e.currentTarget.id) {
                d.style.zIndex = Min + 1;
            }
            else {
                d.style.zIndex = Min;
            }
        });
    },
    GetZindex: function (id) {
        return document.getElementById(id).Zindex;
    },
    SetZindex: function (id, Zindex) {
        document.getElementById(id).SetZindex(Zindex);
    },
    MakeDragElement: function (dialogID) {
        var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
        var head = document.getElementById(dialogID + "-header");
        let elmnt = document.getElementById(dialogID);
        if (head != null)
            head.onmousedown = dragMouseDown;
        else
            elmnt.onmousedown = dragMouseDown;

        function dragMouseDown(e) {
            e = e || window.event;
            if (!e.target.classList.contains("lb-dialog-drag")) return;
            e.preventDefault();
            // get the mouse cursor position at startup:
            pos3 = e.clientX;
            pos4 = e.clientY;
            document.onmouseup = StopDragElement;
            // call a function whenever the cursor moves:
            document.onmousemove = DoDrag;
        }

        function DoDrag(e) {
            e = e || window.event;
            e.preventDefault();
            // calculate the new cursor position:
            pos1 = pos3 - e.clientX;
            pos2 = pos4 - e.clientY;
            pos3 = e.clientX;
            pos4 = e.clientY;
            // set the element's new position:
            elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
            elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
        }

        function StopDragElement() {
            /* stop moving when mouse button is released:*/
            document.onmouseup = null;
            document.onmousemove = null;
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