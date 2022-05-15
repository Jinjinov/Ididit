using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components
{
    public partial class CheckBox
    {
        [Inject]
        JsInterop JsInterop { get; set; } = null!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public bool IsTriState { get; set; }

        [Parameter]
        public bool? Checked { get; set; }

        [Parameter]
        public EventCallback<bool?> CheckedChanged { get; set; }

        private bool internalChecked;

        private bool isIndeterminate;

        private ElementReference elementReference;

        private readonly string elementId = Guid.NewGuid().ToString();

        private void SetInternalChecked()
        {
            internalChecked = Checked != false;
        }

        protected override void OnParametersSet()
        {
            SetInternalChecked();
        }

        private async Task SetIndeterminate()
        {
            bool indeterminate = Checked == null;

            if (isIndeterminate != indeterminate)
            {
                isIndeterminate = indeterminate;

                await JsInterop.SetElementProperty(elementReference, "indeterminate", indeterminate);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SetIndeterminate();
        }

        private async Task ChangeChecked()
        {
            if (IsTriState)
            {
                Checked = Checked switch
                {
                    false => true,
                    true => null,
                    null => false,
                };
            }
            else
            {
                Checked = !Checked;
            }

            await CheckedChanged.InvokeAsync(Checked);
        }

        private async Task OnChange(ChangeEventArgs e)
        {
            await ChangeChecked();

            SetInternalChecked();

            await SetIndeterminate();
        }
    }
}
