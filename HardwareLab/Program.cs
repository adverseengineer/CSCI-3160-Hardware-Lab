namespace HardwareLab {

	internal class Program {

		static void Main(string[] args) {

			CPU cpu = new CPU();

			cpu.SetWord(0, 0xDEADBEEF);
			cpu.SetWord(4, 0x00C0FFEE);
			cpu.DumpMemory();

			Console.WriteLine($"0x{cpu.GetWord(0).ToString("X")}");

		}
	}
}