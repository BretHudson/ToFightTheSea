using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Screenshaker {

		private static int length;
		private static int amount;
		private static Vector2 camera;

		public static float CameraX {
			get { return camera.X; }
		}

		public static float CameraY {
			get { return camera.X; }
		}

		public static void InitShake(int l, int a) {
			length = l;
			amount += a;
		}

		public static void Shake() {
			camera.X = camera.Y = 0;
			if (length > 0) {
				camera.X = Rand.Int(0, (int)amount) - amount * 0.5f;
				camera.Y = Rand.Int(0, (int)amount) - amount * 0.5f;
				amount -= amount / length;
				--length;
			}
		}

	}
}
