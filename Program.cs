using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Program {

		static Game game;
		static bool fullscreen = false;

		static void Main(string[] args) {
			// Create the game
			game = new Game("LD31 - Team Butts", 1920, 1080, 60, false);
			
			// Screen size
			game.SetWindowScale(0.5f);

			// Set up the session
			Global.PlayerOne = CreateSession("PlayerOne");

			// Create the first scene
#if DEBUG
			//game.FirstScene = new Level();
			game.FirstScene = new MainMenu();
#else
			game.FirstScene = new MainMenu();
#endif

			// Get some updating goodness
			game.OnUpdate += ToggleFullscreen;
			game.OnUpdate += ToggleMusic;
			game.OnUpdate += UpdateMaps;

			// Set up explosion shader
			Global.explosionShader = new Shader(ShaderType.Fragment, "assets/shaders/displacement.frag");
			game.Surface.AddShader(Global.explosionShader);
			Global.shockwave.CenterOrigin();

			game.EnableQuitButton = false;

			// Start the game
			game.Start();
		}

		static Session CreateSession(string name) {
			Session session = game.AddSession(name);

			// Directional controls
			session.Controller.Up.AddKey(Key.Up, Key.W);
			session.Controller.Left.AddKey(Key.Left, Key.A);
			session.Controller.Down.AddKey(Key.Down, Key.S);
			session.Controller.Right.AddKey(Key.Right, Key.D);

			session.Controller.Square.AddKey(Key.Z); // Ball
			session.Controller.Triangle.AddKey(Key.X); // Area attack
			session.Controller.Cross.AddKey(Key.C, Key.Return);

			// Let's give that gamepad some lovin'!
			for (int i = 0; i < 4; ++i) {
				session.Controller.Up.AddAxisButton(AxisButton.PovYMinus, i);
				session.Controller.Left.AddAxisButton(AxisButton.PovXMinus, i);
				session.Controller.Down.AddAxisButton(AxisButton.PovYPlus, i);
				session.Controller.Right.AddAxisButton(AxisButton.PovXPlus, i);

				session.Controller.AxisLeft.AddAxis(JoyAxis.PovX, JoyAxis.PovY, i);
				session.Controller.AxisLeft.AddAxis(JoyAxis.X, JoyAxis.Y, i);

				session.Controller.Square.AddButton(2, i);
				session.Controller.Triangle.AddButton(3, i);
				session.Controller.Cross.AddButton(0, i);
			}
			
			return session;
		}

		static void ToggleFullscreen() {
			if (game.Input.KeyPressed(Key.F)) {
				fullscreen = !fullscreen;

				if (fullscreen) game.SetWindow((int)Global.Resolution.X, (int)Global.Resolution.Y, true, true);
				else game.SetWindow(960, 540, false, false);
			}
		}

		static void ToggleMusic() {
			if (game.Input.KeyPressed(Key.M)) {
				Sound.GlobalVolume = Music.GlobalVolume = Math.Abs(Music.GlobalVolume - 1);
			}
		}

		static void UpdateMaps() {
			Global.explosionShader.SetParameter("displacementMap", Global.shaderSurface.Texture);
			Global.shaderSurface.Fill(new Color(0.5f, 0.5f, 0.5f, 1.0f));
		}

	}
}
