using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Inventories;
using StardewValley.Tools;

namespace AutoConsume
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        bool ShouldEat = false;
        bool ShouldDrink = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            // print button presses to the console window
            // this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void OnOneSecondUpdateTicked(object sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;


            // check Buff
            if (!Game1.player.hasBuff("drink") && Game1.player.canMove && Game1.timeOfDay < 2400) ShouldDrink = true;
            else ShouldDrink = false;

            this.Monitor.Log($"CanMove: {Game1.player.canMove}", LogLevel.Debug);
            this.Monitor.Log($"timeofday: {Game1.timeOfDay}", LogLevel.Debug);


            if (ShouldDrink)
            {
                DrinkTrippleShotEspresso();
            }

        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;

            // check health
            if (Game1.player.health <= Game1.player.maxHealth * 0.3 && Game1.player.canMove) ShouldEat = true;
            else ShouldEat = false;

            if (ShouldEat)
            {
                EatCheese();
            }
        }

        private void OnDayStarted(object sender, EventArgs e)
        {
            // drink Triple Shot Espresso when the player wakes up
            // ShouldDrink = true;
        }

        private void EatCheese()
        {
            // set variables
            const string Cheese_ID = "424";
            Item cheese = new StardewValley.Object(Cheese_ID, 1, false, -1, 2);
            StardewValley.Object cheeseObj = new StardewValley.Object(Cheese_ID, 1, false, -1, 2);
            // find cheese 
            int idx = Game1.player.getIndexOfInventoryItem(cheese);
            // check inventory
            if (idx >= 0)
            {
                Game1.player.eatObject(cheeseObj);
                Game1.player.Items.ReduceId(Cheese_ID, 1);
            }
        }

        private void DrinkTrippleShotEspresso()
        {
            // set variable
            const string TSE_ID = "253"; 
            Item TSE = new StardewValley.Object(TSE_ID, 1);
            StardewValley.Object TSEObj = new StardewValley.Object(TSE_ID, 1);
            // find TSE
            int idx = Game1.player.getIndexOfInventoryItem(TSE);
            // check inventory
            if (idx >= 0)
            {
                Game1.player.eatObject(TSEObj);
                Game1.player.Items.ReduceId(TSE_ID, 1);
            }

            // Previous
            /*
             * if (Game1.player.IsBusyDoingSomething()) return;
             * if (Game1.player.buffs.Speed == 0 && Game1.player.getIndexOfInventoryItem(TSE) >= 0)
             */
        }

    }
}