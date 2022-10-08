using System.Text.RegularExpressions;

namespace HardwareLab {

	static class Program {

		public static void Main(string[] args) {

			List<string[]> tokens = Parser.TokenizeSource("../../../data/basic.s");

			//Regex whitespace = new Regex(@",?\s+", RegexOptions.Compiled);
			//Regex comments = new Regex(@"\s*#.+$", RegexOptions.Compiled);

			//string line = "lw      s0, 8(sp)                       # 4-byte Folded Reload";
			//line = comments.Replace(line, "");
			//Console.WriteLine(line);

			//line = whitespace.Replace(line, " ");
			//Console.WriteLine(line);

			for (int i = 0; i < tokens.Count; i++) {
			
				foreach (string token in tokens[i]) {
				
					Console.Write($"{token} ");
				}
				Console.WriteLine();
			}
		}
	}
}