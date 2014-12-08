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
		private Surface darknessSurface;

		private Image lightTexture1 = new Image("assets/gfx/lightTexture.png");
		private static List<Image> lightTextures = new List<Image>();

		public static List<Light> lights = new List<Light>();
		public static List<Light> darkness = new List<Light>();

		public static Player player;

		public Level() {
			// Set up background
			AddGraphic(background);
			background.Scroll = 0;
			AddGraphic(corners);

			background.Shader = new Shader(ShaderType.Fragment, "assets/shaders/water_wave.frag");

			ambientLighting = new Surface(1920, 1080);
			lightSurface = new Surface(1920, 1080);
			darknessSurface = new Surface(1920, 1080);

			lightTexture1.CenterOrigin();

			lightTextures.Add(lightTexture1);
		}

		public override void Begin() {
			base.Begin();

			// Add the player
			player = Add(new Player(1920 >> 1, 1080 >> 1, Global.PlayerOne));

			var explosion = Add(new Explosion(1920 >> 1, 1080 >> 1));
			explosion.SetAlpha(2.0f, 1.0f, 0.0f);
			explosion.SetRadius(2.0f, 100.0f, 580.0f, 560.0f, 480.0f);

			var squid = Add(new Squid(1300, 480));
			squid.target = player;

			// Create the four corners
			CreateCorners();

			// Add dat surface
			Game.AddSurface(ambientLighting);
			Game.AddSurface(lightSurface);
			Game.AddSurface(darknessSurface);

			// Start the music
			Global.gameMusic.Loop = true;
			Global.gameMusic.Volume = 0;
			Global.gameMusic.Play();
		}

		private void CreateCorners() {
			// Entities
			var corner1 = Add(new Corner(0, 0));
			var corner2 = Add(new Corner(0, Game.Height - (int)Corner.size.Y));
			var corner3 = Add(new Corner(Game.Width - (int)Corner.size.X, Game.Height - (int)Corner.size.Y));
			var corner4 = Add(new Corner(Game.Width - (int)Corner.size.X, 0));

			// Lights
			#region Lights
			/*var light1 = new Light();
			light1.entity = corner1;
			light1.SetColor(new Color("008080"));
			light1.SetRadius(300.0f);
			light1.SetAlpha(0.5f);
			light1.SetOffset(30, 30);
			lights.Add(light1);

			var light2 = new Light();
			light2.entity = corner2;
			light2.SetColor(new Color("008080"));
			light2.SetRadius(300.0f);
			light2.SetAlpha(0.5f);
			light2.SetOffset(30, -30);
			lights.Add(light2);

			var light3 = new Light();
			light3.entity = corner3;
			light3.SetColor(new Color("008080"));
			light3.SetRadius(300.0f);
			light3.SetAlpha(0.5f);
			light3.SetOffset(-30, -30);
			lights.Add(light3);

			var light4 = new Light();
			light4.entity = corner4;
			light4.SetColor(new Color("008080"));
			light4.SetRadius(300.0f);
			light4.SetAlpha(0.5f);
			light4.SetOffset(30, -30);
			lights.Add(light4);*/
			#endregion
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

			foreach (Light light in darkness) {
				light.Update(Game.RealDeltaTime * 0.001f);
			}
		}

		public override void Render() {
			base.Render();

			var texture = Game.Surface.GetTexture();

			ambientLighting.Fill(new Color("BCBCBC"));
			ambientLighting.Blend = BlendMode.Multiply;

			darknessSurface.Fill(new Color("FFFFFF"));
			darknessSurface.Blend = BlendMode.Multiply;

			foreach (Light light in lights) {
				var i = light.Image;
				lightTextures[i].Color = light.Color;
				lightTextures[i].Alpha = light.Alpha;
				lightTextures[i].Scale = light.Scale;
				lightSurface.Draw(lightTextures[i], light.X, light.Y);
			}

			foreach (Light light in darkness) {
				var i = light.Image;
				lightTextures[i].Color = light.Color;
				lightTextures[i].Alpha = light.Alpha;
				lightTextures[i].Scale = light.Scale;
				darknessSurface.Draw(lightTextures[i], light.X, light.Y);
			}

			lightSurface.Blend = BlendMode.Add;
		}

	}
}
