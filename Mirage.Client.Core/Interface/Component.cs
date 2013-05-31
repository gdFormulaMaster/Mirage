using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mirage.Client.Core.Interface.Events;
using EventArgs = Mirage.Client.Core.Interface.Events.EventArgs;
using EventHandler = Mirage.Client.Core.Interface.Events.EventHandler;
using Microsoft.Xna.Framework.Graphics;

namespace Mirage.Client.Core.Interface {
    public enum ComponentState {
        Default,
        Hovered,
        Active,
    }

    internal class ComponentZOrderComparer : IComparer<Component> {
        public int Compare(Component a, Component b) {
            return a.ZOrder - b.ZOrder;
        }
    }

    public class Component {
        internal protected static Texture2D pixel = null;
        private static ComponentZOrderComparer ComparerZOrder = new ComponentZOrderComparer();

        #region Component Fields

        private Component parent = null;
        private List<Component> children = new List<Component>();
        private Rectangle bounds = Rectangle.Empty;
        private int zOrder = 0;
        private Color backgroundColor = Color.TransparentBlack;
        private bool visible = true;
        private bool enabled = true;
        private bool focused = false;
        private bool resizable = false;
        private ComponentState state = ComponentState.Default;

        #endregion

        #region Component Properties

        public Component Parent {
            get { return parent; }
            set {
                parent = value;
                TriggerOnParentChanged(this, parent);
            }
        }

        public Rectangle Bounds {
            get { return bounds; }
            set { bounds = value; }
        }

        public Vector2 Position {
            get { return new Vector2(bounds.Left, bounds.Top); }
            set {
                bounds.X = (int)value.X;
                bounds.Y = (int)value.Y;
            }
        }

        public Vector2 Size {
            get { return new Vector2(bounds.Width, bounds.Height); }
            set {
                bounds.Width = (int)value.X;
                bounds.Height = (int)value.Y;
            }
        }

        public int ZOrder {
            get { return zOrder; }
            set {
                zOrder = value;
                UpdateZOrder(false);
                TriggerOnZOrderChanged(this);
            }
        }

        public Color BackgroundColor {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        public bool Visible {
            get { return visible; }
            set {
                visible = value;
                TriggerOnVisibilityChanged(this, visible);
            }
        }

        public bool Enabled {
            get { return enabled; }
            set {
                enabled = value;
                TriggerOnEnabledChanged(this, enabled);
            }
        }

        public bool Resizable {
            get { return resizable; }
            set {
                resizable = value;
                TriggerOnResizableChanged(this, resizable);
            }
        }

        public bool Hovered {
            get { return state == ComponentState.Hovered; }
        }

        public bool Active {
            get { return state == ComponentState.Active; }
        }

        public bool Focused {
            get { return focused; }
        }

        #endregion

        #region Constructors/Destructors

        public Component() {
            OnFocusGained += new EventHandler(ProcessOnFocusGained);
            OnFocusLost += new EventHandler(ProcessOnFocusLost);
        }

        #endregion

        public void Focus() {
            parent.FocusComponent(this);
        }

        public void Unfocus() {
            parent.FocusComponent(null);
        }

        protected virtual Component FocusedComponent() {
            return parent.FocusedComponent();
        }

        protected virtual void FocusComponent(Component component) {
            parent.FocusComponent(component);
        }

        private void UpdateZOrder(bool updateThis = true) {
            if (parent != null)
                parent.UpdateZOrder(true);

            if (updateThis)
                children.Sort(ComparerZOrder);
        }

        public bool Contains(int x, int y) {
            return bounds.Contains(x, y);
        }

        public bool Contains(Point point) {
            return bounds.Contains(point);
        }

        public bool Contains(Vector2 point) {
            return bounds.Contains((int)point.X, (int)point.Y);
        }

        public virtual void Update(InterfaceManager manager, GameTime time) {
            InternalUpdate(manager, time);

            foreach (Component c in children)
                c.Update(manager, time);
        }

        internal protected virtual void InternalUpdate(InterfaceManager manager, GameTime time) {
            if (state != ComponentState.Default || parent == null) {
                Component topmost = null;

                foreach (Component c in children) {
                    if (c.Contains(manager.Input.Cursor)) {
                        topmost = c;
                        break;
                    }
                }

                state = ComponentState.Default;
                if (topmost != null) {
                    topmost.state = (manager.Input.MouseDown(MouseButton.Primary) ? ComponentState.Active : ComponentState.Hovered);
                } else if (Contains(manager.Input.Cursor)) {
                    state = (manager.Input.MouseDown(MouseButton.Primary) ? ComponentState.Active : ComponentState.Hovered);
                }
            }
        }

        public virtual void Draw(InterfaceManager manager, GameTime time) {
            InternalDraw(manager, time);

            foreach (Component c in children)
                c.Draw(manager, time);
        }

        internal protected virtual void InternalDraw(InterfaceManager manager, GameTime time) {
            if (pixel == null) {
                pixel = new Texture2D(manager.GraphicsDevice, 1, 1);
                pixel.SetData(new Color[] { Color.White });
            }

            manager.Batch.Draw(pixel, bounds, backgroundColor);//Hovered ? Color.Yellow : (Active ? Color.Red : backgroundColor));
        }

        public void Add(Component component) {
            if (component != null) {
                if (children.Count > 0)
                    component.zOrder = children[0].zOrder + 1;

                children.Add(component);
                TriggerOnChildAdded(this, component);

                component.parent = this;
                component.TriggerOnParentChanged(this, this);

                children.Sort(ComparerZOrder);
                component.TriggerOnZOrderChanged(this);
            }
        }

        public void Remove(Component component) {
            if (component != null) {
                children.Remove(component);
                TriggerOnChildRemoved(this, component);

                component.parent = null;
                TriggerOnParentChanged(this, null);
            }
        }

        public bool Contains(Component component) {
            return children.Contains(component);
        }

        #region Component Events

        public event EventHandler OnAction;
        internal protected virtual void TriggerOnAction(Component sender) {
            if (OnAction != null)
                OnAction.Invoke(new EventArgs(sender));
        }

        public event MouseEventHandler OnMouseMove;
        internal protected virtual void TriggerOnMouseMove(Component sender, int x, int y) {
            if (OnMouseMove != null)
                OnMouseMove.Invoke(new MouseEventArgs(sender, x, y));
        }

        public event MouseButtonEventHandler OnMouseClick;
        internal protected virtual void TriggerOnMouseClick(Component sender, int x, int y, MouseButton button) {
            if (OnMouseClick != null)
                OnMouseClick.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseButtonEventHandler OnMousePressed;
        internal protected virtual void TriggerOnMousePressed(Component sender, int x, int y, MouseButton button) {
            if (OnMousePressed != null)
                OnMousePressed.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseButtonEventHandler OnMouseReleased;
        internal protected virtual void TriggerOnMouseReleased(Component sender, int x, int y, MouseButton button) {
            if (OnMouseReleased != null)
                OnMouseReleased.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseScrollEventHandler OnMouseScrolled;
        internal protected virtual void TriggerOnMouseScrolled(Component sender, int x, int y, int delta) {
            if (OnMouseScrolled != null)
                OnMouseScrolled.Invoke(new MouseScrollEventArgs(sender, x, y, delta));
        }

        public event KeyEventHandler OnKeyPressed;
        internal protected virtual void TriggerOnKeyPressed(Component sender, Keys key) {
            if (OnKeyPressed != null)
                OnKeyPressed.Invoke(new KeyEventArgs(sender, key));
        }

        public event KeyEventHandler OnKeyReleased;
        internal protected virtual void TriggerOnKeyReleased(Component sender, Keys key) {
            if (OnKeyReleased != null)
                OnKeyReleased.Invoke(new KeyEventArgs(sender, key));
        }

        public event MouseButtonEventHandler OnDrag;
        internal protected virtual void TriggerOnDrag(Component sender, int x, int y, MouseButton button) {
            if (OnDrag != null)
                OnDrag.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseButtonEventHandler OnDrop;
        internal protected virtual void TriggerOnDrop(Component sender, int x, int y, MouseButton button) {
            if (OnDrop != null)
                OnDrop.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseButtonEventHandler OnResizeDrag;
        internal protected virtual void TriggerOnResizeDrag(Component sender, int x, int y, MouseButton button) {
            if (OnResizeDrag != null)
                OnResizeDrag.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event MouseButtonEventHandler OnResizeDrop;
        internal protected virtual void TriggerOnResizeDrop(Component sender, int x, int y, MouseButton button) {
            if (OnResizeDrop != null)
                OnResizeDrop.Invoke(new MouseButtonEventArgs(sender, x, y, button));
        }

        public event BoolStateEventHandler OnVisibilityChanged;
        internal protected virtual void TriggerOnVisibilityChanged(Component sender, bool state) {
            if (OnVisibilityChanged != null)
                OnVisibilityChanged.Invoke(new BoolStateEventArgs(sender, state));
        }

        public event BoolStateEventHandler OnEnabledChanged;
        internal protected virtual void TriggerOnEnabledChanged(Component sender, bool state) {
            if (OnEnabledChanged != null)
                OnEnabledChanged.Invoke(new BoolStateEventArgs(sender, state));
        }

        public event BoolStateEventHandler OnResizableChanged;
        internal protected virtual void TriggerOnResizableChanged(Component sender, bool state) {
            if (OnResizableChanged != null)
                OnResizableChanged.Invoke(new BoolStateEventArgs(sender, state));
        }

        public event MouseEventHandler OnMouseEntered;
        internal protected virtual void TriggerOnMouseEntered(Component sender, int x, int y) {
            if (OnMouseEntered != null)
                OnMouseEntered.Invoke(new MouseEventArgs(sender, x, y));
        }

        public event MouseEventHandler OnMouseLeft;
        internal protected virtual void TriggerOnMouseLeft(Component sender, int x, int y) {
            if (OnMouseLeft != null)
                OnMouseLeft.Invoke(new MouseEventArgs(sender, x, y));
        }

        public event EventHandler OnFocusGained;
        internal protected virtual void TriggerOnFocusGained(Component sender) {
            if (OnFocusGained != null)
                OnFocusGained.Invoke(new EventArgs(sender));
        }
        internal void ProcessOnFocusGained(EventArgs args) {
            focused = (FocusedComponent() == this);
        }

        public event EventHandler OnFocusLost;
        internal protected virtual void TriggerOnFocusLost(Component sender) {
            if (OnFocusLost != null)
                OnFocusLost.Invoke(new EventArgs(sender));
        }
        internal void ProcessOnFocusLost(EventArgs args) {
            focused = (FocusedComponent() == this);
        }

        public event EventHandler OnZOrderChanged;
        internal protected virtual void TriggerOnZOrderChanged(Component sender) {
            if (OnZOrderChanged != null)
                OnZOrderChanged.Invoke(new EventArgs(sender));
        }

        public event ComponentEventHandler OnChildAdded;
        internal protected virtual void TriggerOnChildAdded(Component sender, Component component) {
            if (OnChildAdded != null)
                OnChildAdded.Invoke(new ComponentEventArgs(sender, component));
        }

        public event ComponentEventHandler OnChildRemoved;
        internal protected virtual void TriggerOnChildRemoved(Component sender, Component component) {
            if (OnChildRemoved != null)
                OnChildRemoved.Invoke(new ComponentEventArgs(sender, component));
        }

        public event ComponentEventHandler OnParentChanged;
        internal protected virtual void TriggerOnParentChanged(Component sender, Component component) {
            if (OnParentChanged != null)
                OnParentChanged.Invoke(new ComponentEventArgs(sender, component));
        }

        #endregion
    }
}