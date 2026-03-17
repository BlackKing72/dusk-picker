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

        layerManager.PushLayer(inputHandlerLayer);
        layerManager.PushLayer(backgroundLayer);
        layerManager.PushLayer(magnifierLayer);

        stateProvider.OnColorDismissed += () => layerManager.PopLayer(colorLayer);
        stateProvider.OnColorSelected += () => layerManager.PushLayer(colorLayer);
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
