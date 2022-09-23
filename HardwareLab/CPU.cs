using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Data.SqlTypes;

namespace HardwareLab {

	//by defining a word alias, we can change the word size on our virtual CPU with one edit
	using Addr = System.UInt32;
	using Word = System.UInt32;

	public sealed class CPU {

		private int pc;
		private long elapsedCycles;

		//we have 2KB of memory to play with
		private const Addr MAX_ADDR = 2048;

		private Word[] reg = new Word[32];
		//we define our memory as an array of words, with enough words to add up to MAX_ADDR bytes
		private Byte[] mem = new Byte[MAX_ADDR / sizeof(Byte)];

		#region Register Look-up Tables

		/// <summary>
		/// an enum to make register accesses safer
		/// </summary>
		public enum Register : int {
			zero = 0,
			ra = 1,
			sp = 2,
			gp = 3,
			tp = 4,
			t0 = 5,
			t1 = 6,
			t2 = 7,
			fp = 8, s0 = 8,
			s1 = 9,
			a0 =10,
			a1 = 11,
			a2 = 12,
			a3 = 13,
			a4 = 14,
			a5 = 15,
			a6 = 16,
			a7 = 17,
			s2 = 18,
			s3 = 19,
			s4 = 20,
			s5 = 21,
			s6 = 22,
			s7 = 23,
			s8 = 24,
			s9 = 25,
			s10 = 26,
			s11 = 27,
			t3 = 28,
			t4 = 29,
			t5 = 30,
			t6 = 31,
		}

		/// <summary>
		/// a dictionary used for name lookups of registers
		/// </summary>
		private static Dictionary<Register, string> registerNames = new Dictionary<Register, string> {
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

		#endregion

		#region Dumpers

		/// <summary>
		/// prints out the contents of all registers, except zero
		/// </summary>
		public void RegDump() {

			for(int i = 1; i < reg.Length; i++) {
				String regName = String.Format("{0, 4}", registerNames[(Register)i]);
				String regVal = reg[i].ToString("X8");
				Console.WriteLine($"{regName}: 0x{regVal}");
			}
		}

		/// <summary>
		/// prints out the contents of memory, with 16 bytes per line
		/// </summary>
		/// <param name="bytesPerLine"></param>
		public void HexDump(int bytesPerLine = 16) {

			Debug.Assert(0 < bytesPerLine, "invalid number of bytes per line");

			for (int i = 0; i < mem.Length; i++) {

				string b = String.Format("{0,2:X2} ", mem[i]);
				Console.Write(b);

				if ((i % bytesPerLine) == (bytesPerLine - 1)) {
					Console.WriteLine();
				}
			}
		}

		#endregion

		#region Register Getter and Setter

		/// <summary>
		/// gets the value stored in the specified register
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Word ReadReg(Register register) {

			//return zero for any attempt to read x0
			if (register != Register.zero) return reg[(uint)register];
			else return 0;
		}

		/// <summary>
		/// sets the value of the specified register
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void WriteReg(Register register, Word value) {

			//do nothing for any attempt to write x0
			if (register != Register.zero) reg[(uint)register] = value;
		}

		#endregion

		#region Memory Getter and Setter
		
		/// <summary>
		/// gets the word at the specified address in memory
		/// </summary>
		/// <param name="addr"></param>
		/// <returns></returns>
		public Word ReadWord(Addr addr) {

			//check if memory access would reach beyond bounds
			Debug.Assert(addr < MAX_ADDR - sizeof(Word) + 1, "memory read out of range");

			Word fetchedWord = 0;
			for(int i = 0; i < sizeof(Word); i++)
				fetchedWord |= (Word)(mem[addr + (sizeof(Word) - i) - 1] << (i * 8));

			return fetchedWord;
		}

		/// <summary>
		/// sets the byte at the specified address in memory
		/// </summary>
		/// <param name="addr"></param>
		/// <param name="value"></param>
		public void WriteWord(Addr addr, Word value) {
			
			//check if memory access would reach beyond bounds
			Debug.Assert(addr < MAX_ADDR - sizeof(Word) + 1, "memory write out of range");

			for (int i = 0; i < sizeof(Word); i++)
				mem[addr + (sizeof(Word) - i) - 1] = (Byte)((value >> (i * 8)) & 0xFF);
		}

		#endregion

		#region Instructions

		#region Data Transfers

		/// <summary>
		/// rd ← mem[rs1 + offset]
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="offset"></param>
		public void LW(Register rd, Register rs1, int offset) {

			Addr srcAddr = (Addr)(ReadReg(rs1) + offset);
			Word fetchedWord = ReadWord(srcAddr);
			WriteReg(rd, fetchedWord);
		}

		/// <summary>
		/// mem[rs1 + offset] ← rs2
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void SW(Register rs1, Register rs2, int offset) {

			Addr destAddr = (Addr)(ReadReg(rs1) + offset);
			Word wordToStore = ReadReg(rs2);
			WriteWord(destAddr, wordToStore);
		}

		#endregion

		#region Arithmetic

		/// <summary>
		/// rd ← rs1 + rs2
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		public void ADD(Register rd, Register rs1, Register rs2) {	

			Word opA = ReadReg(rs1);
			Word opB = ReadReg(rs2);
			WriteReg(rd, opA + opB);
		}

		/// <summary>
		/// rd ← rs1 + imm
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="imm"></param>
		public void ADDI(Register rd, Register rs1, Word imm) {

			Word opA = ReadReg(rs1);
			WriteReg(rd, opA + imm);
		}

		/// <summary>
		/// rd ← rs1 - rs2
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		public void SUB(Register rd, Register rs1, Register rs2) {

			Word opA = ReadReg(rs1);
			Word opB = ReadReg(rs2);
			WriteReg(rd, opA - opB);
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
		public void BNE(Register rs1, Register rs2, int offset) {

			if (ReadReg(rs1) != ReadReg(rs2)) pc += offset;
		}

		/// <summary>
		/// if rs1 = rs2 then pc ← pc + offset
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void BEQ(Register rs1, Register rs2, int offset) {

			if (ReadReg(rs1) == ReadReg(rs2)) pc += offset;
		}

		#endregion

		#endregion
	}
}
