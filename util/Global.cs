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

		public static MusicManager musicManager = new MusicManager();

		public static float backgroundTimer = 0.0f;

		public static int fadeOutFrames = 60;
	}

	public enum Tags {
		PLAYER,
		ENEMY,
		SOLID,
		PROJECTILE,
		PLAYERATTACK,
		ENEMYATTACK,
		INK,
		SEALINK,
		FATFISH,
		REPEL,
	}
}
