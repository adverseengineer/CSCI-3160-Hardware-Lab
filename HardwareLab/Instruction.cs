using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareLab {

	public enum InstructionType {
		DataTransfer = 0,
		Arithmetic = 1,
		Control = 2
	}

	public enum Instruction {
		LW = 0,
		SW = 1,
		
	}
}
