// using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace GraveMirror.Items
{
	public class GraveMirror : ModItem
	{
		public override string Texture => $"GraveMirror/Items/GraveMirror";

		public override void SetDefaults() 
		{
			Item.CloneDefaults(ItemID.MagicMirror);
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			// Start creating dust particles when user uses grave mirror
			if (Main.rand.NextBool()) {
				Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, Color.Yellow, 1.1f);
			}

			// if itemTime is 0, start applying itemTime
			if (player.itemTime == 0) {
				player.ApplyItemTime(Item);
			}
			// once we hit the halfway point of itemTime, start logic for teleport
			else if (player.itemTime == player.itemTimeMax / 2) {
				// more particles
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, default, 1.5f);
				}

				// make sure player isn't grappling
				player.grappling[0] = -1;
				player.grapCount = 0;

				// remove player-made projectiles
				for (int p = 0; p < Main.projectile.Length; p++) {
					Projectile projectile = Main.projectile[p];
					if (projectile.active && projectile.owner == player.whoAmI && projectile.aiStyle == 7) {
						projectile.Kill();
					}
				}

				// if user has a death position, teleport there
				if (player.lastDeathPostion != Vector2.Zero) {
					Vector2 vector = player.lastDeathPostion - new Vector2(16f, 24f);
					player.Teleport(vector, 0, 0);
				} else {
					if (player == Main.player[Main.myPlayer])
                    {
						Main.NewText("The mirror shows no reflection...");
					}
				}
				
				// spawn particles one teleported
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
				}
			}
		}

		public override void AddRecipes() 
		{
			int mirrorGroupID = RecipeGroup.RegisterGroup("GraveMirror:Mirrors", new RecipeGroup(() => 
				$"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MagicMirror)} or {Lang.GetItemNameValue(ItemID.IceMirror)}", 
				ItemID.MagicMirror, ItemID.IceMirror));

			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup("GraveMirror:Mirrors");
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddCondition(Condition.InGraveyard);
			recipe.Register();
		}
	}
}