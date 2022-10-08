using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

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

		private Queue<int> fetchQueue = new Queue<int>();
		private Queue<int> decodeQueue = new Queue<int>();
		private Queue<int> executeQueue = new Queue<int>();
		private Queue<int> memAccQueue = new Queue<int>();
		private Queue<int> writeQueue = new Queue<int>();

		private Queue<Instruction> instructionQueue = new Queue<Instruction>();

		public void ExecuteFile(string path) {

			
		}

		#region Dumpers

		/// <summary>
		/// prints out the contents of all registers, except zero
		/// </summary>
		public void RegDump() {

			for(int i = 1; i < reg.Length; i++) {
				String regName = String.Format("{0, 4}", Parser.RegToStr[(Register)i]);
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
				fetchedWord |= (Word)(mem[addr + i] << (i * 8));

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
				mem[addr + i] = (Byte)((value >> (i * 8)) & 0xFF);
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
