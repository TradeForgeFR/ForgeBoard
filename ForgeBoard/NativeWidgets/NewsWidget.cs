﻿using ForgeBoard.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ForgeBoard.NativeWidgets
{
    internal class NewsWidget : BarExtension
    {
        public NewsWidget()
        {

            Name = "News Widget";
            CanBePinned = true;
            PathLogo = PathGeometry.CreateFromGeometry(Geometry.Parse("M4 7V19H19V21H4C2 21 2 19 2 19V7H4M21 5V15H8V5H21M21.3 3H7.7C6.76 3 6 3.7 6 4.55V15.45C6 16.31 6.76 17 7.7 17H21.3C22.24 17 23 16.31 23 15.45V4.55C23 3.7 22.24 3 21.3 3M9 6H12V11H9V6M20 14H9V12H20V14M20 8H14V6H20V8M20 11H14V9H20V11Z"));
            Description = "Displays economical news";
        }

        public override void DeInit()
        {
           // throw new System.NotImplementedException();
        }

        public override void Init()
        {
          //  throw new System.NotImplementedException();
        }

        public override void MenuClick()
        {
            MessageBox.Show("Ce widget doit encore être codé");
        }

        public override void PinnedButtonClick(Button targetButton)
        {
            //throw new System.NotImplementedException();
        }
    }
}
