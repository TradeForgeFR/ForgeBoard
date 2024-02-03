using System.Windows.Controls;
using System.Windows.Media;

namespace ForgeBoard.Core
{
    public abstract class BarExtension 
    {
       
        public BarExtension() 
        {
        }
         
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool CanBePinned { get; protected set; } = false;
        public PathGeometry PathLogo { get; protected set; } = null;         

        /// <summary>
        /// This method is called when the ForgeBoard Bar is initialized. Add your custom logic here for widget(s) creation
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// This method is called when the ForgeBoard Bar is closed
        /// </summary>
        public abstract void DeInit();

        /// <summary>
        /// This method is called when the user click on the favorites/pinned button related of this BarExtension on the right side of the ForgeBoard Bar
        /// </summary>
        /// <param name="targetButton"></param>
        public abstract void PinnedButtonClick(Button targetButton);

        /// <summary>
        /// This method is called when the user click on the menu item related of this BarExtension from the main menu of the ForgeBoard Bar
        /// </summary>
        public abstract void MenuClick();       
    } 
   
}
