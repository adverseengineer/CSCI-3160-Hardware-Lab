using System;

public class Display {
	string[] CtoF = new string[100];
	int[] TtoF = new int[100];
	string FtoD = String.Empty;
	string DtoE = String.Empty;
	string EtoM = String.Empty;
	string MtoW = String.Empty;
	string WtoC = String.Empty;
	int CycleCount = 0;
	static void Main(string[] args) {
		Display test = new Display();
		test.Token("add x1, x2; add x2, x3;  add x3, x4; add x4, x5; add x5, x6; add x6, x7; add x7, x8; sub x8, x9; add x9, x10; sub x10, x11;");
		test.Cycle();
		Console.ReadLine();
	}
	internal enum Instruction : int {
		Lw = 1,
		Sw = 2,
		Add = 3,
		Sub = 4,
		Addi = 5,
		Nop = 0,
		Beq = 6,
		Bne = 7,

	}
	public void Token(string input) {
		string[] ins = input.Split(";");
		for (int i = 0; i < ins.Length; i++) {
			if (ins[i] != null) { this.CtoF[i] = ins[i].Trim(); }
			else { this.CtoF[i] = null; }


		}
		for (int i = 0; i < CtoF.Length; i++) {
			Instruction instruction;
			if (this.CtoF[i] != null) {
				TtoF[i] = 0;
			}
			else {
				Enum.IsDefined(typeof(Instruction), CtoF[i].Split(" "));
			}

		}
	}

	public void Cycle() {

		string instcOutput;
		for (int i = 0; i < 100; i++) {

			instcOutput = WtoC;
			Console.WriteLine("Cycle " + CycleCount + ": " + instcOutput);
			MyWriteBack();
			MyMemoryAccess();
			MyExecute();
			MyDecode();
			MyFetch();
			CycleCount++;
		}



	}
	internal void MyWriteBack() {
		//check if buffer is null
		//execute
		this.WtoC = this.MtoW;
		MtoW = string.Empty;

	}
	internal void MyMemoryAccess() {
		//check if buffers are null
		//execute flag
		this.MtoW = this.EtoM;
		EtoM = string.Empty;
	}
	internal void MyExecute() {
		//check if buffer is null
		//execute
		this.EtoM = this.DtoE;
		DtoE = string.Empty;
	}
	internal void MyDecode() {
		//check if buffer is null
		//execute
		this.DtoE = this.FtoD;
		FtoD = string.Empty;
	}
	internal void MyFetch() {
		//check if buffer is null
		//execute
		this.FtoD = this.CtoF[CycleCount];
		if (CycleCount < CtoF.Length) {
			this.CtoF[CycleCount] = string.Empty;
		}

	}





	//WriteBack();1
	//memory();3
	//execute();1
	//decode();1
	//fetch();1
	//sw==read
	//struc haz mem only
	//data haz WB only
	//crtl haz fetch decode


	//readlist();
	//writelist();


	//scan instructions read stdin 
	//break into tokens
	//ID and count
	//classify
	/*

	 buffers
fetch: 1
memory: 1
ints: 1
fp adds: 0
fp muls: 0
latencies
memory: 3
ints: 1
fp_add: 2
fp_mul: 5
fp_div: 10
	 work backwards for hazards
	read from stdin









	 */
}