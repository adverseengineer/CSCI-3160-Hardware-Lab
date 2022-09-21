using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace HardwareLab {

	public sealed class CPU {

		private int pc;
		private long elapsedCycles;

		private int[] reg = new int[32];
		private int[] mem = new int[512]; //512 ints = 512 * 4 bytes = 2048 bytes 

		private static Dictionary<string, int> abi = new Dictionary<string, int> {
			{"zero", 0},
			{"ra", 1},
			{"sp", 2},
			{"gp", 3},
			{"tp", 4},
			{"t0", 5},
			{"t1", 6},
			{"t2", 7},
			{"fp", 8}, {"s0", 8},
			{"s1", 9},
			{"a0", 10},
			{"a1", 11},
			{"a2", 12},
			{"a3", 13},
			{"a4", 14},
			{"a5", 15},
			{"a6", 16},
			{"a7", 17},
			{"s2", 18},
			{"s3", 19},
			{"s4", 20},
			{"s5", 21},
			{"s6", 22},
			{"s7", 23},
			{"s8", 24},
			{"s9", 25},
			{"s10", 26},
			{"s11", 27},
			{"t3", 28},
			{"t4", 29},
			{"t5", 30},
			{"t6", 31},
		};

		public CPU() {
			//TODO: this		
		}

		#region Instructions

		#region Data Transfers

		/// <summary>
		/// rd ← s32[rs1 + offset]
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="offset"></param>
		/// <param name="rs1"></param>
		public void LW(int rd, int offset, int rs1) {

			Debug.Assert((0 <= rd) && (rd < 32), "rd index out of range");
			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");

			int srcAddr = reg[rs1] + offset;
			reg[rd] = mem[srcAddr];
		}

		/// <summary>
		/// u32[rs1 + offset] ← rs2
		/// </summary>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		/// <param name="rs1"></param>
		public void SW(int rs2, int offset, int rs1) {

			Debug.Assert((0 <= rs2) && (rs2 < 32), "rs2 index out of range");
			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");

			int destAddr = reg[rs1] + offset;
			mem[destAddr] = reg[rs2];
		}

		#endregion

		#region Arithmetic

		/// <summary>
		/// rd ← sx(rs1) + sx(rs2)
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		public void ADD(int rd, int rs1, int rs2) {

			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");
			Debug.Assert((0 <= rs2) && (rs2 < 32), "rs2 index out of range");

			int opA = mem[reg[rs1]];
			int opB = mem[reg[rs2]];
			mem[reg[rd]] = opA + opB;
		}

		/// <summary>
		/// rd ← rs1 + sx(imm)
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="imm"></param>
		public void ADDI(int rd, int rs1, int imm) {

			Debug.Assert((0 <= rd) && (rd < 32), "rd index out of range");
			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");

			int opA = mem[reg[rs1]];
			mem[reg[rd]] = opA + imm;
		}

		/// <summary>
		/// rd ← sx(rs1) - sx(rs2)
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		public void SUB(int rd, int rs1, int rs2) {

			Debug.Assert((0 <= rd) && (rd < 32), "rd index out of range");
			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");
			Debug.Assert((0 <= rs2) && (rs2 < 32), "rs2 index out of range");

			int opA = mem[reg[rs1]];
			int opB = mem[reg[rs2]];
			mem[reg[rd]] = opA - opB;
		}

		#endregion

		#region Control

		/// <summary>
		/// does nothing, pseudo instruction
		/// </summary>
		public void NOP() {

			ADDI(0, 0, 0);
		}

		/// <summary>
		/// if rs1 ≠ rs2 then pc ← pc + offset
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void BNE(int rs1, int rs2, int offset) {

			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");
			Debug.Assert((0 <= rs2) && (rs2 < 32), "rs2 index out of range");

			if (reg[rs1] != reg[rs2])
				pc += offset;
		}

		/// <summary>
		/// if rs1 = rs2 then pc ← pc + offset
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void BEQ(int rs1, int rs2, int offset) {

			Debug.Assert((0 <= rs1) && (rs1 < 32), "rs1 index out of range");
			Debug.Assert((0 <= rs2) && (rs2 < 32), "rs2 index out of range");

			if (reg[rs1] == reg[rs2])
				pc += offset;
		}

		#endregion

		#endregion
	}
}
