using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class BretUtil {

		public static Color LerpColorSet(float amount, params Color[] colors) {
            if (amount <= 0) return colors[0];
            if (amount >= 1) return colors[colors.Length - 1];

            int fromIndex = (int)Util.ScaleClamp(amount, 0, 1, 0, colors.Length - 1);
            int toIndex = fromIndex + 1;

            float length = 1f / (colors.Length - 1);
            float lerp = Util.ScaleClamp(amount % length, 0, length, 0, 1);

            // This is a fix for odd numbered color amounts. When fromIndex was
            // odd, lerp would evaluate to 1 when it should be 0.
            if (lerp >= 0.9999f && fromIndex % 2 == 1) {
                lerp = 0;
            }

            return Util.LerpColor(colors[fromIndex], colors[toIndex], lerp);
        }
	}
}
