using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ItemBanOnJoin
{
	[ApiVersion(1,16)]
	public class ItemBanOnJoin : TerrariaPlugin
	{
		public override string Author
		{
			get { return "Olink"; }
		}

		public override string Description
		{
			get { return "Kick users with banned items before they join."; }
		}

		public override string Name
		{
			get { return "Item Ban Kick On Join"; }
		}

		public override Version Version
		{
			get { return new Version(1, 0, 0, 0); }
		}

		public ItemBanOnJoin(Main game) : base(game)
		{
			
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);

			base.Dispose(disposing);
		}

		private void OnGreet(GreetPlayerEventArgs args)
		{
			TSPlayer tSPlayer = TShock.Players[args.Who];
			Item[] inventory = tSPlayer.TPlayer.inventory;
			List<Item> bannedItems = new List<Item>();

			if (!tSPlayer.Group.HasPermission("itembanonjoin.bypassitembans"))
			{
				for (int i = 0; i < inventory.Length; i++)
				{
					Item item = inventory[i];
					if (item.active && TShock.Itembans.ItemIsBanned(item.name))
					{
						bannedItems.Add(item);
					}
				}
			}

			if (!tSPlayer.Group.HasPermission("itembanonjoin.bypassarmorbans"))
			{
				Item[] armor = tSPlayer.TPlayer.armor;
				for (int i = 0; i < armor.Length; i++)
				{
					Item item = armor[i];
					if (item.active && TShock.Itembans.ItemIsBanned(item.name))
					{
						bannedItems.Add(item);
					}
				}
			}

			Item[] dye = tSPlayer.TPlayer.dye;
			if (!tSPlayer.Group.HasPermission("itembanonjoin.bypassdyebans"))
			{
				for (int i = 0; i < dye.Length; i++)
				{
					Item item = dye[i];
					if (item.active && TShock.Itembans.ItemIsBanned(item.name))
					{
						bannedItems.Add(item);
					}
				}
			}

			if (bannedItems.Count > 0)
			{
				TShock.Utils.ForceKick(tSPlayer, string.Format("{0} {1} banned.  Please remove {2} before joining.", string.Join(",", bannedItems.Select(i => i.name)), bannedItems.Count > 1 ? "are" : "is", bannedItems.Count > 1 ? "them" : "it"), true, false);
				args.Handled = true;
			}
		}
	}
}
