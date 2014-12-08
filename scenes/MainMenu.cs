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

		private int selection = 0;

		public MainMenu() : base(1920, 1080) {
			//
		}

		public override void Update() {
			selection += Convert.ToInt32(Global.PlayerOne.Controller.Right.Pressed) - Convert.ToInt32(Global.PlayerOne.Controller.Left.Pressed);
			selection = (selection + (int)Choices.NUM) % (int)Choices.NUM;

			if (Global.PlayerOne.Controller.Cross.Pressed) {
				switch (selection) {
					case (int)Choices.PLAY:
						Game.AddScene(new Level());
						break;
					case (int)Choices.OPTIONS:
						// TODO:
						break;
					case (int)Choices.QUIT:
						Game.Close();
						break;
				}
			}
		}

	}
}
