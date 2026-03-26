namespace Black.DuskPicker;

public class Picker : Application
{
    private readonly ScreenshotProvider screenshotProvider;
    private readonly StateProvider stateProvider;

    public Picker(PickerOptions? options = null)
        : base()
    {
        PickerOptions pickerOptions = options ?? new PickerOptions();

        screenshotProvider = new();
        stateProvider = new(pickerOptions.Palette);

        InputHandlerLayer inputHandlerLayer = new(stateProvider);
        BackgroundLayer backgroundLayer = new(screenshotProvider);
        MagnifierLayer magnifierLayer = new(stateProvider, screenshotProvider, pickerOptions);
        ColorInfoLayer colorLayer = new(stateProvider);
        ColorPreviewLayer quickViewLayer = new(stateProvider, screenshotProvider);

        layerManager.PushLayer(inputHandlerLayer);
        layerManager.PushLayer(backgroundLayer);
        layerManager.PushLayer(magnifierLayer);
        layerManager.PushLayer(quickViewLayer);

        stateProvider.OnColorDismissed += () =>
        {
            layerManager.PopOverlay(colorLayer);
            layerManager.PushLayer(quickViewLayer);
        };

        stateProvider.OnColorSelected += () =>
        {
            layerManager.PopLayer(quickViewLayer);
            layerManager.PushOverlay(colorLayer);
        };

        stateProvider.OnQuitRequested += this.HideWindow;

        Cursor.Lock();
    }

    protected override void OnEvent(Event evt)
    {
        screenshotProvider.OnEvent(evt);
        stateProvider.OnEvent(evt);
    }

    protected override void OnUpdate()
    {
        screenshotProvider.OnUpdate();
    }
}
