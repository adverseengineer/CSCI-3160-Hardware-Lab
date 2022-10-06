namespace HardwareLab {

	static class Program {

		public static void Main(string[] args) {

			CPU cpu = new CPU();

			//
			cpu.WriteWord(0, 0x9);
			cpu.WriteWord(4, 0x5);
			
			//load both into registers
			cpu.LW(Register.t1, Register.zero, 0);
			cpu.LW(Register.t2, Register.zero, 4);

			//add them, and store the result
			cpu.ADD(Register.t1, Register.t1, Register.t2);
			cpu.SW(Register.zero, Register.t1, 8);

			//dump to see the results
			cpu.RegDump();
			cpu.HexDump(32);
		}
	}
}