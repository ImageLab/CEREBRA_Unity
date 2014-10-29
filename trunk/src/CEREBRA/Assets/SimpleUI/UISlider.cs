using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleUI;
using UnityEngine;

namespace SimpleUI
{
    class UISlider : UIElementBase
    {
        public int hSliderValue = 0;
        public int maxValue = 0;
        private int prevVal;

        public UISlider()
        {

        }

        public UISlider(int maxValue)
        {
            this.maxValue = maxValue;
        }

        public delegate void OnChangeHandler(IUIElement sender, EventArgs e);
        public event OnChangeHandler onChange;

        public override void OnGUI() {
            if (sizeGiven && positionGiven)
            {
                prevVal = hSliderValue;
                hSliderValue = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y), hSliderValue, 0, maxValue));
                if (prevVal != hSliderValue)
                {
                    if (onChange != null)
                    {
                        onChange(this, null);
                    }
                }
            }
        }

    }
}
