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

		public static Music gameMusic = new Music("assets/sfx/Where The Light Fades.wav", true);
		public static Music bossMusic = new Music("assets/sfx/To Fight The Sea.wav", true);
	}

	public enum Tags {
		PLAYER,
		ENEMY,
		SOLID,
		PROJECTILE,
		PLAYERATTACK,
		ENEMYATTACK,
		INK,
	}
}
