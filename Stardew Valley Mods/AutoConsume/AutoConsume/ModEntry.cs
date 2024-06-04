using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace AutoConsume
{
    // create Class for Keybinding
    public sealed class ModConfig
    {
        public KeybindList OpenMenuKey { get; set; } = KeybindList.Parse("O");
        public bool AutoHealKey { get; set; }
        public bool AutoBuffKey { get; set; }
        public string HealItemID { get; set; } = "424"; // cheese item id
        public int HealItemQuality { get; set; }
        public string BuffItemID { get; set; } = "253"; // Triple shot espresso item id
        public int BuffItemQuality { get; set; }
        public int BuffStartTime { get; set; }
        public int BuffEndTime { get; set; }
    }

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        bool ShouldHeal = false;
        bool ShouldBuff = false;
        bool buffMessageOn = false;
        bool healMessageOn = false;
        string warningMessage = "There are no available items in the inventory";

        private ModConfig Config;
        private List<Item> InventoryItems = new List<Item>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        }

        
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        ///
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Game1.player.canMove) return;
            if (!Context.IsWorldReady) return;

            if (Config.OpenMenuKey.JustPressed())
            {
                // Get Inventroy Items
                GetInventoryItems();

                // if Auto Consume Menu is Open then close
                if (Game1.activeClickableMenu is AutoConsumeMenu autoConsumeMenu)
                {
                    autoConsumeMenu.exitThisMenu();
                }
                else Game1.activeClickableMenu = (IClickableMenu)(object)new AutoConsumeMenu(Config, InventoryItems);
            }
        }

        private void GetInventoryItems()
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            // clear list
            InventoryItems.Clear();
            // get inventory items that can consume
            foreach (Item curItem in Game1.player.Items)
            {
                if (curItem == null) continue;
                StardewValley.Object o = new StardewValley.Object(curItem.ItemId, 1);
                if (o.Edibility > 0) InventoryItems.Add(curItem);
            }
        }

        private void OnOneSecondUpdateTicked(object sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            if (!Config.AutoBuffKey) return;
            if (Game1.activeClickableMenu is AutoConsumeMenu autoConsumeMenu) return;
            // check Buff
            if (!Game1.player.hasBuff("drink") && !Game1.player.hasBuff("food") && Game1.player.canMove && !Game1.IsFading() && Game1.timeOfDay >= Config.BuffStartTime && Game1.timeOfDay < Config.BuffEndTime)
            {
                ShouldBuff = true;
            }
            else ShouldBuff = false;

            if (ShouldBuff)
            {
                GoBuff();
            }

        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            if (!Config.AutoHealKey) return;
            if (Game1.player.health <= 0) return;
            if (Game1.activeClickableMenu is AutoConsumeMenu autoConsumeMenu) return;

            // check health
            if (Game1.player.health <= Game1.player.maxHealth * 0.3 && Game1.player.canMove && !Game1.IsFading()) ShouldHeal = true;
            else ShouldHeal = false;

            if (ShouldHeal)
            {
                GoHeal();
            }

        }

        private void GoHeal()
        {
            // set variables
            string HealID = Config.HealItemID;
            int HealQuality = Config.HealItemQuality;
            StardewValley.Object HealObj = new StardewValley.Object(HealID, 1, false, -1, HealQuality);
            Item HealItem = HealObj;
            // find HealItem
            int HealIdx = Game1.player.getIndexOfInventoryItem(HealObj);
            // check inventory
            if (HealIdx >= 0)
            {
                Game1.player.eatObject(HealObj);
                Game1.player.Items.ReduceId(HealID, 1);
            }
            else
            {
                // If there are no available items in the inventory, then open the menu
                Game1.addHUDMessage(new HUDMessage(warningMessage, 3));
                Config.AutoHealKey = false;
                //GetInventoryItems();
                //Game1.activeClickableMenu = (IClickableMenu)(object)new AutoConsumeMenu(Config, InventoryItems);
            }
        }
        
        private void GoBuff()
        {
            // set variable
            string BuffID = Config.BuffItemID;
            int BuffQuality = Config.BuffItemQuality;
            StardewValley.Object BuffObj = new StardewValley.Object(BuffID, 1, false, -1, BuffQuality);
            Item BuffItem = BuffObj;
            // find BuffItem
            int BuffIdx = Game1.player.getIndexOfInventoryItem(BuffObj);
            // check inventory
            if (BuffIdx >= 0)
            {
                Game1.player.eatObject(BuffObj);
                Game1.player.Items.ReduceId(BuffID, 1);
            }
            else
            {
                // If there are no available items in the inventory, then open the menu
                Game1.addHUDMessage(new HUDMessage(warningMessage, 3));
                Config.AutoBuffKey = false;
            }
        }
    }
}