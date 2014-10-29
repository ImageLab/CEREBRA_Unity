using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
    class UIImageButton : UIElementBase
    {
        Texture2D image;
        public UIImageButton(string path)
        {
            Debug.Log("hi " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            //image = Resources.Load(path) as Texture2D;

            image = (Texture2D)Resources.LoadAssetAtPath(path, typeof(Texture2D));
            
            if (image == null) Debug.Log("fuck");
        }

        public delegate void onClickHandler(IUIElement sender, EventArgs e);
        public event onClickHandler onClick;

        public override void OnGUI()
        {
            if(GUILayout.Button(image))
            {
                if(onClick != null) onClick(this, null);
            }
        }
    }
}
