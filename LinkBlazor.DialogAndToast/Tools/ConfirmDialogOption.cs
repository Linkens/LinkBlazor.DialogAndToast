using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkBlazor
{
    public class ConfirmDialogOptions
    {
        public string OkText { get; set; } = "Ok";
        public string OkClass { get; set; } = "lb-Ok";
        public string CancelText { get; set; } = "Cancel";
        public string CancelClass { get; set; } = "lb-Cancel";
    }
}
