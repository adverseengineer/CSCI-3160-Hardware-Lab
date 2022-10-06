using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Data.SqlTypes;


namespace HardwareLab {

	public class Program 
	{

		static void Main(string[] args) 
		{
			Instruction i1 = new Instruction("add, x9, x4, x2");

			Instruction it = new Instruction("lw, x4, 20(x7)");

			Console.WriteLine(i1.getDestRegister);
			Console.ReadLine();

			
		}
	}
}