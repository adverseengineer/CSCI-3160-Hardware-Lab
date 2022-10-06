using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Data.SqlTypes;

namespace HardwareLab
{
    //enum for all of the instruction types
    enum InstructionType
    {
        lw,
        sw,
        add,
        addi,
        sub,
        bne,
        beq
        
    }
    /*
    This is the Instuction class where we hold how the instructions are held 
    */
    public class Instruction
    {
        //array to hold the pieces of the instruction
        private string[] struction;

        public Instruction(string instruct)
        {
            //seperators to separate the the instructions
            char[] seperator = {',', ' '};

            //will split the instructions into their seperate pieces
            struction = instruct.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

        }

        
        /*
        //Method to split up the intstruction string in a way where we can use all of its pieces
        public string[] tokenize(string instruct)
        {
            //place the string into a character array (the string will have this format ("lw, x9, 0(x3)") or ("add, x9, x5, x2"))
            char[] instructionChars = instruct.ToCharArray();

            int start = 0;
            int end = instructionChars.Length();

            //while loop to keep up with where we are at in the gathering of the components of the instruction
            while(start != end)
            {
                //if we are at the start, that means that we are getting the type of instruction first
                if(start == 0)
                {
                    string type = "";
                    while(!instructionChars[start].Equals(","))
                    {
                        type+= instructionChars[start];
                    }
                }
            }
        }
        */

        //Method to get the type of instruction from our array list
        public string getInstructionType()
        {
            //the first element in the array will always be the instruction
            return struction[0];
        }

        //get destination register
        public string getDestRegister()
        {
            //the second element in the array will always be the destination register
            return struction[1];

        }

        
        
    }


}