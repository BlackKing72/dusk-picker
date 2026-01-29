namespace Black.DuskPicker;

public abstract class Layer
{
    public virtual void OnAttach() { }

    public virtual void OnDetach() { }

    /// <summary> Updates from back to front, so from the first added layer to the last </summary>
    public virtual void OnGui() { }

    /// <summary> Updates from back to front, so from the first added layer to the last </summary>
    public virtual void OnRender() { }

    /// <summary> Updates front to back, so from the last added layer to the first. </summary>
    /// <returns> A boolean indicating if the layer manager should block the update from being called on other layers. </returns>
    public virtual bool OnUpdate()
    {
        return false;
    }

    /// <summary> Updates front to back, so from the last added layer to the first. </summary>
    public virtual void OnEvent(Event evt) { }
}

public class LayerManager
{
    private readonly List<Layer> layers = [];
    private readonly List<Layer> overlays = [];

    public void PushLayer(Layer layer) => PushLayer(layers, layer);

    public void PopLayer(Layer layer) => PopLayer(layers, layer);

    public void PopLayer() => PopLayer(layers, layers[^1]);

    public void PushOverlay(Layer layer) => PushLayer(overlays, layer);

    public void PopOverlay(Layer layer) => PopLayer(overlays, layer);

    public void PopOverlay() => PopLayer(overlays, overlays[^1]);

    public void Dispose()
    {
        IterateBackToFront(static layer =>
        {
            layer.OnDetach();
            return false;
        });

        layers.Clear();
    }

    /// <summary> Updates front to back, so from the last added layer to the first. </summary>
    public void OnUpdate()
    {
        IterateFrontToBack(static layer =>
        {
            return layer.OnUpdate();
        });
    }

    /// <summary> Updates from back to front, so from the first added layer to the last </summary>
    public void OnRender()
    {
        IterateBackToFront(static layer =>
        {
            layer.OnRender();
            return false;
        });
    }

    public void OnGui()
    {
        IterateBackToFront(static layer =>
        {
            layer.OnGui();
            return false;
        });
    }

    public void OnEvent(Event currentEvent)
    {
        IterateFrontToBack(layer =>
        {
            layer.OnEvent(currentEvent);
            return currentEvent.IsHandled;
        });
    }

    private void IterateBackToFront(Func<Layer, bool> forEach)
    {
        // create a copy of both lists, making it safe to modify during iteration
        List<Layer> layerStack = [.. layers, .. overlays];
        for (int index = 0; index < layerStack.Count; index++)
        {
            if (forEach(layerStack[index]))
                break;
        }
    }

    private void IterateFrontToBack(Func<Layer, bool> forEach)
    {
        // create a copy of both lists, making it safe to modify during iteration
        List<Layer> layerStack = [.. layers, .. overlays];
        for (int index = layerStack.Count - 1; index >= 0; index--)
        {
            if (forEach(layerStack[index]))
                break;
        }
    }

    private static void PushLayer(List<Layer> list, Layer layer)
    {
        list.Add(layer);
        layer.OnAttach();
    }

    private static void PopLayer(List<Layer> list, Layer layer)
    {
        layer.OnDetach();
        list.Remove(layer);
    }
}
