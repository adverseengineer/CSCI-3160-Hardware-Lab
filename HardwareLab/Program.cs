namespace HardwareLab {

	internal class Program {

		static void Main(string[] args) {

			CPU cpu = new CPU();

			//set up some initial memory
			cpu.WriteWord(0, 0x9);
			cpu.WriteWord(4, 0x5);

			//load both into registers
			cpu.LW(CPU.Register.t1, CPU.Register.zero, 0);
			cpu.LW(CPU.Register.t2, CPU.Register.zero, 4);

			//add them, and store the result
			cpu.ADD(CPU.Register.t1, CPU.Register.t1, CPU.Register.t2);
			cpu.SW(CPU.Register.zero, CPU.Register.t1, 8);

			//dump to see the results
			cpu.RegDump();
			cpu.HexDump(32);
		}
	}
}