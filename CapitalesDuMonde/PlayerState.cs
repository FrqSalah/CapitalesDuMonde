using System;
using System.Collections.Generic;
using System.Text;

namespace CapitalesDuMonde
{
    public class PlayerState
    {
        public int Score { get; set; }
        public bool Progress { get; set; }

        public Pays Pays{get; set;}
        public List<string> Questions { get; set; }
    }
}
