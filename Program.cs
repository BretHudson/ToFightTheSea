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
			game.FirstScene = new Level();

			// Get some updating goodness
			game.OnUpdate += ToggleFullscreen;

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

			session.Controller.X.AddKey(Key.C);
			session.Controller.Square.AddKey(Key.X); // Ball
			session.Controller.Triangle.AddKey(Key.Z); // Area attack
			
			return session;
		}

		static void ToggleFullscreen() {
			if (game.Input.KeyPressed(Key.F)) {
				fullscreen = !fullscreen;

				if (fullscreen) game.SetWindow(1920, 1080, true, true);
				else game.SetWindow(960, 540, false, false);
			}
		}

	}
}
