using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class MainMenu : Scene {

		enum Choices {
			PLAY, OPTIONS, QUIT, NUM
		};

		private Surface ambientLighting;
		private Surface lightSurface;

		private Image lightTexture = new Image("assets/gfx/menuLight.png");
		public static List<Light> lights = new List<Light>();

		private Image background = new Image("assets/gfx/menubg.png");
		private Image corners = new Image("assets/gfx/Corners.png");

		private int selection = 0;

		private Entity playButton, howToButton, quitButton;

		private Light menuLight;

		private Image credits = new Image("assets/gfx/credits.png");

		public MainMenu() : base(1920, 1080) {
			AddGraphic(background);
			background.Scroll = 0;
			AddGraphic(corners);

			/*credits.X = 290;
			credits.Y = 800;
			credits.CenterOrigin();*/
			AddGraphic(credits);

			background.Shader = new Shader(ShaderType.Fragment, "assets/shaders/water_wave.frag");

			ambientLighting = new Surface(1920, 1080);
			lightSurface = new Surface(1920, 1080);

			Add(new Logo(421, 542, "assets/gfx/logo.png"));

			// Play/How To/Quit buttons
			playButton = Add(new Logo(1486, 440, "assets/gfx/play.png", 14.0f));
			howToButton = Add(new Logo(1544, 540, "assets/gfx/howto.png", 14.0f));
			quitButton = Add(new Logo(1480, 640, "assets/gfx/quit.png", 14.0f));

			menuLight = new Light(0, 0.1f);
			menuLight.entity = playButton;
			menuLight.SetAlpha(1.0f, 0.7f);
			menuLight.SetAlphaSpan(1.1f);
			menuLight.SetOffset(175, 100);
			lights.Add(menuLight);
		}

		public override void Begin() {
			Game.AddSurface(ambientLighting);
			Game.AddSurface(lightSurface);
		}

		public override void End() {
			Game.RemoveSurface(ambientLighting);
			Game.RemoveSurface(lightSurface);
		}

		public override void Update() {
			if (Input.KeyPressed(Key.Escape)) {
				Game.Close();
			}

			foreach (Light light in lights) {
				light.Update(Game.RealDeltaTime * 0.001f);
			}

			selection -= Convert.ToInt32(Global.PlayerOne.Controller.Up.Pressed) - Convert.ToInt32(Global.PlayerOne.Controller.Down.Pressed);
			selection = (selection + (int)Choices.NUM) % (int)Choices.NUM;

			switch (selection) {
				case (int)Choices.PLAY:
					menuLight.entity = playButton;
					menuLight.SetOffset(175, 100);
					break;
				case (int)Choices.OPTIONS:
					menuLight.entity = howToButton;
					menuLight.SetOffset(240, 100);
					break;
				case (int)Choices.QUIT:
					menuLight.entity = quitButton;
					menuLight.SetOffset(175, 100);
					break;
			}

			if (Global.PlayerOne.Controller.Cross.Pressed) {
				switch (selection) {
					case (int)Choices.PLAY:
						Game.SwitchScene(new Level());
						break;
					case (int)Choices.OPTIONS:
						Game.SwitchScene(new HowTo());
						break;
					case (int)Choices.QUIT:
						Game.Close();
						break;
				}
			}

			Global.backgroundTimer += Game.DeltaTime;
			background.Shader.SetParameter("timer", Global.backgroundTimer);
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
