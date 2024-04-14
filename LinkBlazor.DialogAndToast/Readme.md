# Blazor Dialog and Toast

### Installation

#### In header css

	<link rel="stylesheet" href="_content/LinkBlazor.DialogAndToast/LinkBlazor.css" />

#### And js for dialog 

	<script src="_content/LinkBlazor.DialogAndToast/LinkBlazor.js"></script>

#### Add services in the startup of the server

	builder.Services.AddLinkBlazorServices();

#### And add the component in the layout that you want to use

	<LinkBlazorDialog />
	<LinkBlazorToast />

#### Good to go !

### Usage

#### Inject services

	[Inject] public DialogService Dialog { get; set; }
    [Inject] public ToastService Toast { get; set; }

#### Dialog

	await Dialog.OpenAsync<TestWindow>(options: new DialogOptions { Title = "Test", CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true });
	@code {
		[Parameter] public int Value { get; set; }
		protected async Task RecursiveChaos()
		{
			await Dialog.OpenAsync<TestWindow>(new() { { "Value", Value + 1 } });
		}
	}

#### Toast

    protected async Task ShowToast(ToastSeverity Severity)
    {
        Toast.Toast(Severity, $"{Severity.ToString()} Lorem Ipsum", $"{Index++} Lorem Ipsum.", duration: 15000);
    }