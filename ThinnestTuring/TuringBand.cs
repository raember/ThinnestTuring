using System;
using System.Collections.Generic;

namespace ThinnestTuring
{
	public sealed class TuringBand
	{
		public const uint BANDOVERHEAD = 5;
		private List<char> band;
		private int headPosition;
		private char emptySlot;

		public TuringBand(String baseWord = String.Empty, char emptySlot = '␣')
		{
			band = new List<char>(baseWord.ToCharArray());
			this.emptySlot = emptySlot;
		}

		public void WriteLeft(char write) {
			WriteStay(write);
			headPosition--;
		}
		public void WriteRight(char write) {
			WriteStay(write);
			headPosition++;
		}
		public void WriteStay(char write) {
			if (headPosition < 0) {
				for (int amountPrefix = Math.Abs(headPosition) - 1; amountPrefix > 0; amountPrefix--) {
					band.Insert(0, emptySlot);
				}
				band.Insert(0, write);
				headPosition = 0;
			} else if (headPosition >= band.Count) {
				for (int amountSuffix = headPosition - band.Count; amountSuffix > 0; amountSuffix--) {
					band.Add(emptySlot);
				}
				band.Add(write);
			} else {
				band[headPosition] = write;
			}
		}

		public char Read() {
			if (headPosition < 0 || headPosition >= band.Count) {
				return emptySlot;
			}
			return band[headPosition];
		}

		public override string ToString ()
		{
			var strOut = new String (emptySlot, BANDOVERHEAD);
			var minIndex = Math.Min(headPosition, band.Count - 1);
			var maxIndex = Math.Max(headPosition, band.Count - 1);
			for (int index = minIndex; index < maxIndex; index++) {
				if (index == headPosition) {
					strOut &= '私';
				}
				if (index >= 0 && index < band.Count) {
					strOut &= band[index];
				} else {
					strOut &= emptySlot;
				}
			}
			strOut &= new String (emptySlot, BANDOVERHEAD);
			return strOut;
		}
	}
}