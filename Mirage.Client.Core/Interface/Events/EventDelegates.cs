using System;

namespace Mirage.Client.Core.Interface.Events {
    [Serializable]
    public delegate void EventHandler(EventArgs args);

    [Serializable]
    public delegate void MouseEventHandler(MouseEventArgs args);

    [Serializable]
    public delegate void MouseButtonEventHandler(MouseButtonEventArgs args);

    [Serializable]
    public delegate void MouseScrollEventHandler(MouseScrollEventArgs args);

    [Serializable]
    public delegate void KeyEventHandler(KeyEventArgs args);

    [Serializable]
    public delegate void ComponentEventHandler(ComponentEventArgs args);

    [Serializable]
    public delegate void BoolStateEventHandler(BoolStateEventArgs args);
}