using System;
using System.Collections.Generic;

namespace ThinnestTuring
{
	public class TuringMachine
	{
		private List<TuringBand> bands = new List<>();
		private Delegate transitionFunction;

		public TuringMachine(uint amountOfBands, String baseWord, char emptySlot) {
			if (amountOfBands == 0) {
				throw new ArgumentOutOfRangeException ("amountOfBands",
					"The amount of bands may not be zero.");
			}
			bands.Add(new TuringBand(baseWord, emptySlot));
			while (amountOfBands > 1) {
				bands.Add (new TuringBand(String.Empty, emptySlot));
				amountOfBands--;
			}
		}
		public TuringMachine(String baseWord) {
			return new TuringMachine(1, baseWord);
		}

		public void Step() {

		}

		public override string ToString()
		{
			var strOut = String.Empty;
			int bandN;
			foreach (TuringBand band in bands) {
				strOut &= String.Format("Band {1}:[{2}]\n", bandN, band.ToString);
			}
			return strOut;
		}
	}
}

