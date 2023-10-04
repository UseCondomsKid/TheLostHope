using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEngine.EngineCode.UI
{
    public abstract class Selectable : UIElement
    {
        public event Action OnSelect;
        public event Action OnDeselect;
        public event Action OnEnter;

        protected Selectable(GraphicsDevice graphicsDevice, UIAnchor anchor, float xPosPercent, float yPosPercent, UIElement parent,
            Action onEnter = null, Action onSelect = null, Action onDeselct = null) : base(graphicsDevice, anchor, xPosPercent, yPosPercent, parent)
        {
            OnEnter = onEnter;
            OnSelect = onSelect;
            OnDeselect = onDeselct;
        }

        public bool IsSelected { get; private set; }

        public virtual void Select()
        {
            OnSelect?.Invoke();
            IsSelected = true;
        }
        public virtual void Deselect()
        {
            OnDeselect?.Invoke();
            IsSelected = false;
        }
        public virtual void Enter()
        {
            OnEnter?.Invoke();
        }
    }
}
