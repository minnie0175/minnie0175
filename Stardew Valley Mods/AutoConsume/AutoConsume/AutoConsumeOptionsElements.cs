using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley.Menus;

namespace AutoConsume
{
    public class AutoConsumeOptionsElements : OptionsElement
    {
        public AutoConsumeOptionsElements(string label)
             : base(label)
        {
        }

        public AutoConsumeOptionsElements(string label, int x, int y, int width, int height, int whichOption = -1)
            : base(label, x, y, width, height, whichOption)
        {
        }

        public override void receiveLeftClick(int x, int y)
        {
            
        }

        public override void leftClickHeld(int x, int y)
        {
            
        }

        public override void leftClickReleased(int x, int y)
        {

        }

    }
}