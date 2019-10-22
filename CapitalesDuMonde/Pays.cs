using System;
using System.Collections.Generic;
using System.Text;

namespace CapitalesDuMonde
{


    public class Monde
    {

        public List<Continent> ListContinents { get; set; }

    }


    public class Continent
    {

        public string NomContienent { get; set; }

        private List<Pays> ListePays { get; set; }


    }


    public partial class Pays
    {

        /// <remarks/>
        public string Nom { get; set; }

        /// <remarks/>
        public string Capitale { get; set; }

        /// <remarks/>
        public string Monnaie { get; set; }
    }
}


