# Blazor Dialog and Toast

### Installation

#### In header css

```html
<link rel="stylesheet" href="_content/LinkBlazor.DialogAndToast/LinkBlazor.css" />
```

#### And js for dialog 

```html
<script src="_content/LinkBlazor.DialogAndToast/LinkBlazor.js"></script>
```

#### Add services in the startup of the server

```csharp
builder.Services.AddLinkBlazorServices();
```
#### And add the component in the layout that you want to use

```html
<LinkBlazorDialog />
<LinkBlazorToast />
```

#### Good to go !

### Usage

#### Inject services

```csharp
[Inject] public DialogService Dialog { get; set; }
[Inject] public ToastService Toast { get; set; }
```
#### Dialog

```csharp
await Dialog.OpenAsync<TestWindow>(options: new DialogOptions { Title = "Test", CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true });
@code {
	[Parameter] public int Value { get; set; }
	protected async Task RecursiveChaos()
	{
		await Dialog.OpenAsync<TestWindow>(new() { { "Value", Value + 1 } });
	}
}
```
#### Toast

```csharp
protected async Task ShowToast(ToastSeverity Severity)
{
    Toast.Toast(Severity, $"{Severity.ToString()} Lorem Ipsum", $"{Index++} Lorem Ipsum.", duration: 15000);
}
```