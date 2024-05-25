using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoConsume
{
    public class AutoConsumeMenu : IClickableMenu
    {
        // Fields
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();
        private readonly List<ClickableTextureComponent> CheckBoxes = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> Arrows = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> ItemBoxes = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> HealItemInfoIcons = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> BuffItemInfoIcons = new List<ClickableTextureComponent>();
        private readonly List<ClickableComponent> HealInfoTexts = new List<ClickableComponent>();
        private readonly List<ClickableComponent> BuffInfoTexts = new List<ClickableComponent>();
        private readonly List<ClickableComponent> BuffEndTimeTexts = new List<ClickableComponent>();
        private ClickableTextureComponent ExitButton;
        private ModConfig Config;
        private readonly List<Item> InventoryItems;
        private readonly List<Item> InventoryBuffItems = new List<Item>();
        private Rectangle EmptyCheckBox = new Rectangle(227, 425, 9, 9);
        private Rectangle FullCheckBox = new Rectangle(236, 425, 9, 9);
        private Rectangle RightArrow = new Rectangle(365, 494, 12, 12);
        private Rectangle LeftArrow = new Rectangle(352, 494, 12, 12);
        private Rectangle ItemBox = new Rectangle(293, 360, 24, 24);
        private Rectangle HIcon = new Rectangle(0, 438, 10, 10);
        private Rectangle EIcon = new Rectangle(0, 428, 10, 10);
        private int healItemIdx = 0;
        private int buffItemIdx = 0;
        private const float ScaleFactor = 4f;
        private const float IconScaleFactor = ScaleFactor / 1.5f;

        private static int menuWidth = (int)(Game1.uiViewport.Width/2);
        private static int menuHeight = (int)(Game1.uiViewport.Height/1.2);

        private AutoConsumeOptionsSlider slider;


        // Public Method
        public AutoConsumeMenu(ModConfig Config, List<Item> InventoryItems)
            : base((int)getAppropriateMenuPosition().X, (int)getAppropriateMenuPosition().Y, menuWidth, menuHeight)
        {
            // initialized variables
            this.Config = Config;
            this.InventoryItems = InventoryItems;
            foreach(Item buffItem in InventoryItems)
            {
                if (Game1.objectData.TryGetValue(buffItem.ItemId, out var value) && value.Buffs != null)
                    InventoryBuffItems.Add(buffItem);
            }

            // Get the item ID from the configuration, find the index of the InventoryItem, and then set the item index.
            this.setIndex();

            // setup components position
            this.setUpPositions();

            this.slider = new AutoConsumeOptionsSlider("", this.Config, -1, this.xPositionOnScreen + borderWidth, this.yPositionOnScreen + borderWidth + (9 * (int)ScaleFactor + 20) * 8);
        }

        public static Vector2 getAppropriateMenuPosition()
        {
            Vector2 defaultPosition = new Vector2((Game1.uiViewport.Width - menuWidth)/2 , (Game1.uiViewport.Height - menuHeight)/2);

            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + menuWidth > Game1.uiViewport.Width) defaultPosition.X = 0;
            if (defaultPosition.Y + menuHeight > Game1.uiViewport.Height) defaultPosition.Y = 0;

            return defaultPosition;

        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (int)getAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)getAppropriateMenuPosition().Y;
            this.setUpPositions();

        }

        // private method
        private void setIndex()
        {
            int tmpidx = 0;
            bool finditem = false;
            foreach(Item curitem in InventoryItems)
            {
                if (curitem.ItemId == Config.HealItemID)
                {
                    healItemIdx = tmpidx;
                    finditem = true;
                }
                tmpidx++;
            }

            if (!finditem && InventoryItems.Count != 0)
            {
                healItemIdx = 0;
                Config.HealItemID = InventoryItems[healItemIdx].ItemId; 
                Config.HealItemQuality = InventoryItems[healItemIdx].Quality;
            }

            
            tmpidx = 0;
            finditem = false;
            foreach(Item curitem in InventoryBuffItems)
            {
                if (curitem.ItemId == Config.BuffItemID)
                {
                    buffItemIdx = tmpidx;
                    finditem = true;
                }
                tmpidx++;
            }

            if (!finditem && InventoryBuffItems.Count != 0)
            {
                buffItemIdx = 0;
                Config.BuffItemID = InventoryBuffItems[buffItemIdx].ItemId;
                Config.BuffItemQuality = InventoryBuffItems[buffItemIdx].Quality;
            }
        }


        private void setUpPositions()
        {
            string healLabelText = "Auto Heal";
            string buffLabelText = "Auto Buff";
            string healItemText = "Heal Item";
            string buffItemText = "Buff Item";
            string buffEndTimeText = "Buff End Time: ";
            int paddingSize = 20;
            int spriteSize = 9 * (int)ScaleFactor + paddingSize;
            int itemBoxSize = ItemBox.Width * (int)(ScaleFactor * 0.9f);
            int arrowSize = RightArrow.Width * (int)ScaleFactor;
            int recX = this.xPositionOnScreen + borderWidth;
            int recY = this.yPositionOnScreen + borderWidth;
            Rectangle sliderPos = new Rectangle(recX, recY + spriteSize * 8, 1, 1);

            // clear and initialized
            this.ExitButton = new ClickableTextureComponent("exit-button", new Rectangle(this.xPositionOnScreen + menuWidth, this.yPositionOnScreen, Game1.tileSize, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(337, 493, 13, 13), ScaleFactor);
            this.Labels.Clear();
            this.CheckBoxes.Clear();
            this.Arrows.Clear();
            this.ItemBoxes.Clear();
            // set labels and checkboxes position

            this.CheckBoxes.Add(new ClickableTextureComponent("autoheal-check-box", new Rectangle(recX, recY, (int)(EmptyCheckBox.Width * ScaleFactor), (int)(EmptyCheckBox.Height * ScaleFactor)), "", "", Game1.mouseCursors, EmptyCheckBox, ScaleFactor));
            this.CheckBoxes.Add(new ClickableTextureComponent("autobuff-check-box", new Rectangle(recX, recY + spriteSize, (int)(EmptyCheckBox.Width * ScaleFactor), (int)(EmptyCheckBox.Height * ScaleFactor)), "", "", Game1.mouseCursors, EmptyCheckBox, ScaleFactor));
            this.Labels.Add(new ClickableComponent(new Rectangle(recX + spriteSize, recY, 1, 1), healLabelText));
            this.Labels.Add(new ClickableComponent(new Rectangle(recX + spriteSize, recY + spriteSize, 1, 1), buffLabelText));
            // set arrows and itemboxes position and labels position
            this.Labels.Add(new ClickableComponent(new Rectangle(recX, recY + spriteSize * 2, 1, 1), healItemText));
            this.Labels.Add(new ClickableComponent(new Rectangle(recX, recY + spriteSize * 5, 1, 1), buffItemText));

            this.Arrows.Add(new ClickableTextureComponent("heal-item-left-arrow", new Rectangle(recX, recY + spriteSize * 3 + itemBoxSize/4, (int)(LeftArrow.Height * ScaleFactor),(int)(LeftArrow.Height * ScaleFactor)), "", "", Game1.mouseCursors, LeftArrow, ScaleFactor));
            this.Arrows.Add(new ClickableTextureComponent("heal-item-right-arrow", new Rectangle(recX + arrowSize + paddingSize * 2 + (int)(ItemBox.Width * ScaleFactor * 0.9f), recY + spriteSize * 3 + itemBoxSize / 4, (int)(RightArrow.Width * ScaleFactor), (int)(RightArrow.Height * ScaleFactor)), "", "", Game1.mouseCursors, RightArrow, ScaleFactor));
            this.ItemBoxes.Add(new ClickableTextureComponent("heal-item-box", new Rectangle(recX + arrowSize + paddingSize, recY + spriteSize * 3, (int)(ItemBox.Width * ScaleFactor), (int)(ItemBox.Height * ScaleFactor)), "", "", Game1.mouseCursors, ItemBox, ScaleFactor * 0.9f));

            this.Arrows.Add(new ClickableTextureComponent("buff-item-left-arrow", new Rectangle(recX, recY + spriteSize * 6 + itemBoxSize / 4, (int)(LeftArrow.Height * ScaleFactor), (int)(LeftArrow.Height * ScaleFactor)), "", "", Game1.mouseCursors, LeftArrow, ScaleFactor));
            this.Arrows.Add(new ClickableTextureComponent("buff-item-right-arrow", new Rectangle(recX + arrowSize + paddingSize * 2 + (int)(ItemBox.Width * ScaleFactor * 0.9f), recY + spriteSize * 6 + itemBoxSize / 4, (int)(RightArrow.Width * ScaleFactor), (int)(RightArrow.Height * ScaleFactor)), "", "", Game1.mouseCursors, RightArrow, ScaleFactor));
            this.ItemBoxes.Add(new ClickableTextureComponent("buff-item-box", new Rectangle(recX + arrowSize + paddingSize, recY + spriteSize * 6, (int)(ItemBox.Width * ScaleFactor), (int)(ItemBox.Height * ScaleFactor)), "", "", Game1.mouseCursors, ItemBox, ScaleFactor * 0.9f));
            // set Info box position
            this.HealItemInfoIcons.Add(new ClickableTextureComponent("HP-icon", new Rectangle(recX + 300, recY + spriteSize * 3, HIcon.Width*(int)IconScaleFactor, HIcon.Height*(int)IconScaleFactor), "", "", Game1.mouseCursors, HIcon, IconScaleFactor));
            this.HealItemInfoIcons.Add(new ClickableTextureComponent("Energy-icon", new Rectangle(recX + 300, recY + spriteSize * 3 + EIcon.Width * (int)IconScaleFactor + paddingSize, EIcon.Width * (int)IconScaleFactor, EIcon.Height * (int)IconScaleFactor), "", "", Game1.mouseCursors, EIcon, IconScaleFactor));

            // buff end time text position
            sliderPos.X = this.xPositionOnScreen + borderWidth + paddingSize + 222;
            this.BuffEndTimeTexts.Add(new ClickableComponent(sliderPos, buffEndTimeText));
        }

        private void setInformationIconPos()
        {
            // get heal information
            int healthRecoverdAmount = InventoryItems[healItemIdx].healthRecoveredOnConsumption();
            int EnergyRecoverdAmount = InventoryItems[healItemIdx].staminaRecoveredOnConsumption();
            
        }

        private void handleButtonClick(string name)
        {
            switch (name)
            {
                case "autoheal-check-box":
                    Config.AutoHealKey = !Config.AutoHealKey;
                    break;
                case "autobuff-check-box":
                    Config.AutoBuffKey = !Config.AutoBuffKey;
                    break;
                case "exit-button":
                    this.exitThisMenu();
                    break;
                case "heal-item-left-arrow":
                    healItemIdx--;
                    if (healItemIdx < 0) healItemIdx = 0;
                    break;
                case "heal-item-right-arrow":
                    if (healItemIdx + 1 < InventoryItems.Count) healItemIdx++;
                    break;
                case "buff-item-left-arrow":
                    buffItemIdx--;
                    if (buffItemIdx < 0) buffItemIdx = 0;
                    break;
                case "buff-item-right-arrow":
                    if (buffItemIdx + 1 < InventoryBuffItems.Count) buffItemIdx++;
                    break;

            }

            // set Config data: HealItemId, Quality, BuffItemID etc
            if(InventoryItems.Count != 0)
            {
                Config.HealItemID = InventoryItems[healItemIdx].ItemId;
                Config.HealItemQuality = InventoryItems[healItemIdx].Quality;
            }
            if(InventoryBuffItems.Count != 0)
            {
                Config.BuffItemID = InventoryBuffItems[buffItemIdx].ItemId;
                Config.BuffItemQuality = InventoryBuffItems[buffItemIdx].Quality;
            }

            Game1.playSound("Ostrich");
        }


        // overide
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            foreach(ClickableTextureComponent checkbox in this.CheckBoxes.ToList())
            {
                if (checkbox.containsPoint(x, y))
                {
                    this.handleButtonClick(checkbox.name);
                }
            }

            foreach(ClickableTextureComponent arrow in this.Arrows.ToList())
            {
                if (arrow.containsPoint(x, y))
                {
                    this.handleButtonClick(arrow.name);
                }
            }

            if (ExitButton.containsPoint(x, y))
            {
                this.handleButtonClick(ExitButton.name);
            }

            this.slider.receiveLeftClick(x, y);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            this.slider.leftClickHeld(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            float textSize = 1.3f;
            // draw menu box
            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.Beige);
            // draw exit button
            this.ExitButton.draw(b);
            // draw text
            foreach (ClickableComponent label in this.Labels)
            {
                // draw in a violet color so that the text can be seen when the background is dark
                Color color = Color.Violet;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color, textSize);
                color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color, textSize);
            }
            
            // draw check boxes
            foreach (ClickableTextureComponent checkbox in this.CheckBoxes)
            {
                switch (checkbox.name)
                {
                    case "autoheal-check-box":
                        if (Config.AutoHealKey) checkbox.sourceRect = FullCheckBox;
                        else checkbox.sourceRect = EmptyCheckBox;
                        break;

                    case "autobuff-check-box":
                        if (Config.AutoBuffKey) checkbox.sourceRect = FullCheckBox;
                        else checkbox.sourceRect = EmptyCheckBox;
                        break;

                }
                checkbox.draw(b);     
            }

            BuffInfoTexts.Clear();
            BuffItemInfoIcons.Clear();
            // draw item box and arrows
            foreach (ClickableTextureComponent itembox in this.ItemBoxes)
            {
                itembox.draw(b);
                switch (itembox.name)
                {
                    case "heal-item-box":
                        if (InventoryItems.Count == 0) break;
                        InventoryItems[healItemIdx].drawInMenu(b, new Vector2(itembox.bounds.X + 10, itembox.bounds.Y + 10), 1f);
                        break;
                    case "buff-item-box":
                        if (InventoryBuffItems.Count == 0) break;
                        InventoryBuffItems[buffItemIdx].drawInMenu(b, new Vector2(itembox.bounds.X + 10, itembox.bounds.Y + 10), 1f);
                        // get buff information

                        //int tmpy = 1;
                        foreach (Buff curbuff in InventoryBuffItems[buffItemIdx].GetFoodOrDrinkBuffs())
                        {
                            //BuffItemInfoIcons.Add(new ClickableTextureComponent("buff", new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, 64, 64), "", "", curbuff.iconTexture, Game1.getSourceRectForStandardTileSheet(curbuff.iconTexture, curbuff.iconSheetIndex), IconScaleFactor));
                            //BuffInfoTexts.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen * tmpy, 1, 1), "test" + curbuff.effects.LuckLevel.ToString()));
                            //tmpy++;
                        }


                        break;
                }
            }
            foreach (ClickableTextureComponent arrow in this.Arrows)
            {
                arrow.draw(b);
            }


            // draw infomation
            /*
            foreach (ClickableTextureComponent healIcon in this.HealItemInfoIcons)
            {
                healIcon.draw(b);
            }
            foreach (ClickableTextureComponent buffIcon in this.BuffItemInfoIcons)
            {
                buffIcon.draw(b);
            }
            foreach (ClickableComponent buffInfo in this.BuffInfoTexts)
            {
                Utility.drawTextWithShadow(b, buffInfo.name, Game1.smallFont, new Vector2(buffInfo.bounds.X, buffInfo.bounds.Y), Game1.textColor, 1.5f);
            }
            */

            // draw slider bar
            this.slider.draw(b, 0, 0);

            foreach (ClickableComponent buffendtimetext in this.BuffEndTimeTexts)
            {
                string text = "";
                text += Config.BuffEndTime.ToString();
                string text1 = text.Insert(text.Length - 2, ":");
                Utility.drawTextWithShadow(b, buffendtimetext.name + text1, Game1.smallFont, new Vector2(buffendtimetext.bounds.X, buffendtimetext.bounds.Y), Game1.textColor, textSize / 1.3f);
            }



            // draw cursor
            this.drawMouse(b);
        }

    }
}