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

		private float backgroundTimer = 0.0f;

		private Surface ambientLighting;
		private Surface lightSurface;

		private Image lightTexture = new Image("assets/gfx/lightTexture.png");

		public static List<Light> lights = new List<Light>();

		public Level() {
			// Set up background
			AddGraphic(background);
			background.Scroll = 0;
			AddGraphic(corners);

			background.Shader = new Shader(ShaderType.Fragment, "assets/shaders/water_wave.frag");

			ambientLighting = new Surface(1920, 1080);
			lightSurface = new Surface(1920, 1080);

			lightTexture.CenterOrigin();
		}

		public override void Begin() {
			base.Begin();

			// Add the player
			Add(new Player(1920 >> 1, 1080 >> 1, Global.PlayerOne));

			Add(new Squid(1100, 480));

			// Create the four corners
			CreateCorners();

			// Add dat surface
			Game.AddSurface(ambientLighting);
			Game.AddSurface(lightSurface);
		}

		private void CreateCorners() {
			// Entities
			var corner1 = Add(new Corner(0, 0));
			var corner2 = Add(new Corner(0, Game.Height - (int)Corner.size.Y));
			var corner3 = Add(new Corner(Game.Width - (int)Corner.size.X, Game.Height - (int)Corner.size.Y));
			var corner4 = Add(new Corner(Game.Width - (int)Corner.size.X, 0));

			// Lights
			var light1 = new Light();
			light1.entity = corner1;
			light1.SetColor(new Color("008080"));
			light1.SetRadius(300.0f);
			light1.SetAlpha(0.5f);
			lights.Add(light1);
		}

		public override void Update() {
			Screenshaker.Shake();
			CameraX = Screenshaker.CameraX;
			CameraY = Screenshaker.CameraY;

			backgroundTimer += Game.DeltaTime;
			background.Shader.SetParameter("timer", backgroundTimer);

			foreach (Light light in lights) {
				light.Update(Game.RealDeltaTime * 0.001f);
			}
		}

		public override void Render() {
			base.Render();

			var texture = Game.Surface.GetTexture();

			ambientLighting.Fill(new Color("BCBCBC"));
			ambientLighting.Blend = BlendMode.Multiply;

			foreach (Light light in lights) {
				lightTexture.Color = light.Color;
				lightTexture.Alpha = light.Alpha;
				lightTexture.Scale = light.Scale;
				lightSurface.Draw(lightTexture, light.X, light.Y);
			}

			lightSurface.Blend = BlendMode.Add;


		}

	}
}
