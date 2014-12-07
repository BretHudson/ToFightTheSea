using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Level : Scene {

		private Image background = new Image("assets/gfx/BG.png");
		private Image corners = new Image("assets/gfx/Corners.png");

		public Level() {
			// Set up background
			AddGraphic(background);
			background.Scroll = 0;
			AddGraphic(corners);
		}

		public override void Begin() {
			base.Begin();

			// Add the player
			Add(new Player(1920 >> 1, 1080 >> 1, Global.PlayerOne));

			// Create the four corners
			Add(new Corner(0, 0));
			Add(new Corner(0, Game.Height - (int)Corner.size.Y));
			Add(new Corner(Game.Width - (int)Corner.size.X, Game.Height - (int)Corner.size.Y));
			Add(new Corner(Game.Width - (int)Corner.size.X, 0));
		}

		public override void Update() {
			Screenshaker.Shake();
			CameraX = Screenshaker.CameraX;
			CameraY = Screenshaker.CameraY;
		}

	}
}
