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
		private Word[] mem = new Word[MAX_ADDR / sizeof(Word)];

		public enum Register : uint {
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

		#region Register Getter and Setter

		/// <summary>
		/// gets the value stored in the specified register
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Word GetReg(Register register) {

			//return zero for any attempt to read x0
			if (register != Register.zero) return reg[(uint)register];
			else return 0;
		}

		/// <summary>
		/// sets the value of the specified register
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void SetReg(Register register, Word value) {

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
		public Word GetWord(Addr addr) {

			//we subtract sizeof(Word) here so that a word read near the end will not reach out of bounds
			Debug.Assert(addr < MAX_ADDR - sizeof(Word), "memory read out of range");

			//divide by the size of a word to get the actual index for our memory array
			uint index = addr / sizeof(Word);

			//calculate the number of bytes on the right of the word boundary
			int rBytes = (int)(addr % sizeof(Word));

			//if it is zero, our read is aligned and can simply return the word at that address
			if (rBytes == 0)
				return mem[index];
			//otherwise, we must access the word at addr and the one after it to compose the word
			else {

				//calculate the number of bytes on the left of the word boundary
				int lBytes = sizeof(Word) - rBytes;

				//fetch the two words
				Word lWord = mem[index];
				Word rWord = mem[index + 1];

				Word lMask = Word.MaxValue >> rBytes;
				Word rMask = Word.MaxValue >> lBytes;

				//this bit makes much more sense on paper
				Word w1 = (lWord & lMask) << (rBytes * 8);
				Word w2 = (rWord >> (lBytes * 8)) & rMask;

				return w1 | w2;
			}

		}

		/// <summary>
		/// sets the byte at the specified address in memory
		/// </summary>
		/// <param name="addr"></param>
		/// <param name="value"></param>
		public void SetWord(Addr addr, Word value) {
			
			Debug.Assert(addr < MAX_ADDR - sizeof(Word), "memory write out of range");

			//divide by the size of a word to get the actual index for our memory array
			uint index = addr / sizeof(Word);

			//calculate the number of bytes on the right of the word boundary
			int rBytes = (int)(addr % sizeof(Word));

			//if it is zero, our write is aligned and can simply assign the word at that address
			if (rBytes == 0)
				mem[index] = value;
			//otherwise, we must overwrite parts of the word at addr and the one after it
			else {

				//calculate the number of bytes on the left of the word boundary
				int lBytes = sizeof(Word) - rBytes;

				//calculate the two chunks of our word
				Word lValue = value & (Word.MaxValue >> rBytes);
				Word rValue = value & (Word.MaxValue << lBytes);

				//clear the bits where we are inserting our word
				mem[index] &= (Word.MaxValue << rBytes);
				mem[index + 1] &= (Word.MaxValue >> lBytes);

				//and insert the two chunks of our word into their respective blocks
				mem[index] |= lValue;
				mem[index + 1] |= rValue;
			}
		}

		#endregion

		public void DumpMemory() {

			for (int i = 0; i < mem.Length; i++) {

				Console.WriteLine(mem[i].ToString("X"));
			}
		}

		#region Instructions

		#region Data Transfers

		/// <summary>
		/// rd ← mem[rs1 + offset]
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="offset"></param>
		public void LW(Register rd, Register rs1, int offset) {

			Addr srcAddr = (Addr)(GetReg(rs1) + offset);
			Word fetchedWord = GetWord(srcAddr);
			SetReg(rd, fetchedWord);
		}

		/// <summary>
		/// mem[rs1 + offset] ← rs2
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void SW(Register rs1, Register rs2, int offset) {

			Addr destAddr = (Addr)(GetReg(rs1) + offset);
			Word wordToStore = GetReg(rs2);
			SetWord(destAddr, wordToStore);
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

			Word opA = GetReg(rs1);
			Word opB = GetReg(rs2);
			SetReg(rd, opA + opB);
		}

		/// <summary>
		/// rd ← rs1 + imm
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="imm"></param>
		public void ADDI(Register rd, Register rs1, Word imm) {

			Word opA = GetReg(rs1);
			SetReg(rd, opA + imm);
		}

		/// <summary>
		/// rd ← rs1 - rs2
		/// </summary>
		/// <param name="rd"></param>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		public void SUB(Register rd, Register rs1, Register rs2) {

			Word opA = GetReg(rs1);
			Word opB = GetReg(rs2);
			SetReg(rd, opA - opB);
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

			if (GetReg(rs1) != GetReg(rs2)) pc += offset;
		}

		/// <summary>
		/// if rs1 = rs2 then pc ← pc + offset
		/// </summary>
		/// <param name="rs1"></param>
		/// <param name="rs2"></param>
		/// <param name="offset"></param>
		public void BEQ(Register rs1, Register rs2, int offset) {

			if (GetReg(rs1) == GetReg(rs2)) pc += offset;
		}

		#endregion

		#endregion
	}
}
