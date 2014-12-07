using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Global {
		public static Session PlayerOne;

		public static Vector2 Resolution = new Vector2(1920, 1080);

		public static Shader explosionShader;
		public static Surface shaderSurface = new Surface(1920, 1080);

		public static Image shockwave = new Image("assets/gfx/shockwave.png");

		public static Image displacementMap = new Image("assets/gfx/displacementMap.png");
		public static Image gradientMap = new Image("assets/gfx/gradientMap.png");
		public static Image paletteMap = new Image("assets/gfx/paletteMap.jpg");

		public static Music gameMusic = new Music("assets/sfx/music.wav", true);
	}

	public enum Tags {
		PLAYER,
		SOLID,
		PROJECTILE,
		PLAYERATTACK,
		ENEMYATTACK,
	}
}
