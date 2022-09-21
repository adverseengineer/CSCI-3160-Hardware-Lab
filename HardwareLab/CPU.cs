using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareLab {

	public sealed class CPU {

		private ulong elapsedCycles;
		private uint[] registers = new uint[32];

		public uint this[uint index] {
			get {
				if (index != 0) return registers[index];
				else return 0;
			}
			set {
				if (index != 0) registers[index] = value;
			}
		}

		public CPU() {
			
		}
	}
}
