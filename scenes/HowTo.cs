using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class HowTo : Scene {

		private Surface ambientLighting;
		private Surface lightSurface;

		private Image lightTexture = new Image("assets/gfx/menuLight.png");
		public static List<Light> lights = new List<Light>();

		private Image background = new Image("assets/gfx/menubg.png");
		private Image corners = new Image("assets/gfx/Corners.png");

		private Image controls = new Image("assets/gfx/zx.png");
		private Image wasd = new Image("assets/gfx/wasd.png");
		private Image fullscreen = new Image("assets/gfx/fullscreen.png");

		public HowTo() : base(1920, 1080) {
			AddGraphic(background);
			background.Scroll = 0;
			AddGraphic(corners);

			controls.SetPosition(1536, 209);
			controls.CenterOrigin();
			AddGraphic(controls);

			wasd.SetPosition(1540, 580);
			wasd.CenterOrigin();
			AddGraphic(wasd);

			fullscreen.SetPosition(1540, 900);
			fullscreen.CenterOrigin();
			AddGraphic(fullscreen);

			background.Shader = new Shader(ShaderType.Fragment, "assets/shaders/water_wave.frag");

			ambientLighting = new Surface(1920, 1080);
			lightSurface = new Surface(1920, 1080);

			Add(new Logo(421, 542, "assets/gfx/logo.png"));
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
				Game.SwitchScene(new MainMenu());
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
