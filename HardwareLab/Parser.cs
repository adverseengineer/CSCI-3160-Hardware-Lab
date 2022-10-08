using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HardwareLab {

	static class Parser {

		public static List<string[]> TokenizeSource(string path) {

			string[] lines = File.ReadAllLines(path, Encoding.ASCII);
			List<string[]> tokens = new List<string[]>();

			Regex comments = new Regex(@"\s*#.+$", RegexOptions.Compiled);
			Regex whitespace = new Regex(@",?\s+", RegexOptions.Compiled);

			for (int i = 0; i < lines.Length; i++) {

				string currentLine = lines[i];

				currentLine = comments.Replace(currentLine, "");
				currentLine = whitespace.Replace(currentLine, " ");

				//if that leaves us with any line left, tokenize it
				if(currentLine.Length > 0)
					tokens.Add(currentLine.Split(" "));
			}

			return tokens;
		}

		/// <summary>
		/// converts register enum values to the proper string representation
		/// </summary>
		public static readonly Dictionary<Register, string> RegToStr = new Dictionary<Register, string> {
			{ Register.zero, "zero" },
			{ Register.ra, "ra" },
			{ Register.sp, "sp" },
			{ Register.gp, "gp" },
			{ Register.tp, "tp" },
			{ Register.t0, "t0" },
			{ Register.t1, "t1" },
			{ Register.t2, "t2" },
			{ Register.fp, "fp" }, //fp is also s0
			{ Register.s1, "s1" },
			{ Register.a0, "a0" },
			{ Register.a1, "a1" },
			{ Register.a2, "a2" },
			{ Register.a3, "a3" },
			{ Register.a4, "a4" },
			{ Register.a5, "a5" },
			{ Register.a6, "a6" },
			{ Register.a7, "a7" },
			{ Register.s2, "s2" },
			{ Register.s3, "s3" },
			{ Register.s4, "s4" },
			{ Register.s5, "s5" },
			{ Register.s6, "s6" },
			{ Register.s7, "s7" },
			{ Register.s8, "s8" },
			{ Register.s9, "s9" },
			{ Register.s10, "s10" },
			{ Register.s11, "s11" },
			{ Register.t3, "t3" },
			{ Register.t4, "t4" },
			{ Register.t5, "t5" },
			{ Register.t6, "t6" },
		};

		/// <summary>
		/// converts ascii register names to the proper register enum value
		/// </summary>
		public static readonly Dictionary<string, Register> StrToReg = new Dictionary<string, Register> {
			{ "zero", Register.zero },
			{ "ra", Register.ra },
			{ "sp", Register.sp },
			{ "gp", Register.gp },
			{ "tp", Register.tp },
			{ "t0", Register.t0 },
			{ "t1", Register.t1 },
			{ "t2", Register.t2 },
			{ "fp", Register.fp }, { "s0", Register.s0},
			{ "s1", Register.s1 },
			{ "a0", Register.a0 },
			{ "a1", Register.a1 },
			{ "a2", Register.a2 },
			{ "a3", Register.a3 },
			{ "a4", Register.a4 },
			{ "a5", Register.a5 },
			{ "a6", Register.a6 },
			{ "a7", Register.a7 },
			{ "s2", Register.s2 },
			{ "s3", Register.s3 },
			{ "s4", Register.s4 },
			{ "s5", Register.s5 },
			{ "s6", Register.s6 },
			{ "s7", Register.s7 },
			{ "s8", Register.s8 },
			{ "s9", Register.s9 },
			{ "s10", Register.s10 },
			{ "s11", Register.s11 },
			{ "t3", Register.t3 },
			{ "t4", Register.t4 },
			{ "t5", Register.t5 },
			{ "t6", Register.t6 },
		};
	}
}
