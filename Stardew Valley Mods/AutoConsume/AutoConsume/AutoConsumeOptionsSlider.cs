using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace AutoConsume
{
    public class AutoConsumeOptionsSlider : AutoConsumeOptionsElements
    {
        // Fields
        public static Rectangle sliderBGSource = new Rectangle(403, 383, 6, 6);
        public static Rectangle sliderButtonRect = new Rectangle(420, 441, 10, 6);

        public const int pixelsWide = 48;
        public const int pixelsHigh = 6;
        public const int sliderButtonWidth = 10;
        public const int sliderMaxValue = 100;
        public int value;

        ModConfig Config;

        public AutoConsumeOptionsSlider(string label, ModConfig Config)
            : base(label)
        {
            this.Config = Config;
        }

        public AutoConsumeOptionsSlider(string label, ModConfig Config, int whichOption, int x = -1, int y = -1, int width = 192, int height = 24)
            : base(label, x, y, width, height, whichOption)
        {
            this.Config = Config;
            setValue();
        }

        public void setValue()
        {
            int hour = Config.BuffEndTime / 100;
            int minute = Config.BuffEndTime % 100;
            value = (hour * 60 + minute - 360) / 10;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!greyedOut)
            {
                base.leftClickHeld(x, y);
                if (y < bounds.Y - 8 || y > bounds.Y + 32) return;
                if (x < bounds.X)
                {
                    value = 0;
                }
                else if (x > bounds.Right - 40)
                {
                    value = 120;
                }
                else
                {
                    value = (int)((float)(x - bounds.X) / (float)(bounds.Width - 40) * 120f);
                }
                // Config buff end time changed
                changedBuffEndTime(value);
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!greyedOut)
            {
                base.receiveLeftClick(x, y);
                leftClickHeld(x, y);
            }
        }

        private void changedBuffEndTime(int percentage)
        {
            percentage *= 10;
            int hour = (percentage + 360) / 60 * 100;
            int minute = (percentage + 360) % 60;
            Config.BuffEndTime = hour + minute;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            base.draw(b, slotX, slotY, context);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, sliderBGSource, slotX + bounds.X, slotY + bounds.Y, bounds.Width + 30, bounds.Height, Color.White, 4f, drawShadow: false);
            b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + bounds.X) + (float)(bounds.Width - 40) * ((float)value / 100f), slotY + bounds.Y), sliderButtonRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
        }
    }
}