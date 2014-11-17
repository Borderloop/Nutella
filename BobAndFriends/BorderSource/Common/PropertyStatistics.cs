using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class PropertyStatistics : IStatistics
    {
        public int maxPropertyLength = 0;
        public double averagePropertyLength = 0;
        public int wrongPropertyCount = 0;
        public int[] occurences = new int[300];

        private string _Description = "Statistics for measuring the length of the properties of a Product object.";
        public string Description { get { return _Description; } }


        public void Add(string input)
        {
            if (input == null) input = "";

            if (input.Length >= occurences.Length) occurences[occurences.Length-1]++;
            else occurences[input.Length]++;

            wrongPropertyCount++;
            if (input.Length > maxPropertyLength) maxPropertyLength = input.Length;

            if (averagePropertyLength == 0) 
                averagePropertyLength = input.Length;
            else
            {
                averagePropertyLength = (double)(averagePropertyLength*(wrongPropertyCount-1))/(double)wrongPropertyCount + (double)input.Length/(double)wrongPropertyCount;
            }            
        }
    }
}
