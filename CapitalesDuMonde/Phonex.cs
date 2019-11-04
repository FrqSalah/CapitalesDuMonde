using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CapitalesDuMonde
{
    public class Phonex
    {


        public static double Encode(string text)
        {
            // Remplacer les caratères prasites : àâäãéèêëìîïòôöõùûüñ => AAAAYYYYIIIOOOOUUUN
            text = Regex.Replace(text, "[à|â|ä|ã]", "a");
            text = Regex.Replace(text, "[é|è|ê|ë]", "y");
            text = Regex.Replace(text, "[ì|î|ï]", "i");
            text = Regex.Replace(text, "[ò|ô|ö|õ]", "o");
            text = Regex.Replace(text, "[ù|û|ü]", "u");
            text = Regex.Replace(text, "[ñ]", "n");

            // Remplacer les y par des i
            text = text.Replace("Y", "I");
            text = text.ToUpper();
            // supprimer les h qui ne sont pas précédées de c ou de s ou de p
            text = Regex.Replace(text, @"([^C|P|S])H", @"$1");

            //Remplacement du ph par f
            text = text.Replace("PH", "F");

            //Remplacer les groupes de lettres gan => 	kan / gam => kam / gain => kain / gaim => kaim,
            text = Regex.Replace(text, @"G(AI?[N|M])", @"K$1");

            //  Remplacer ain => yn / ein =>  yn / aim => yn / eim => yn,  si elles sont suivies par une lettre a, e, i, o, ou u 
            text = Regex.Replace(text, @"[A|E]I[N|M]([A|E|I|O|U])", @"YN$1");

            // Remplacement de groupes de 3 lettres (sons "o", "oua", "ein") :
            text = text.Replace("EAU", "O");
            text = text.Replace("OUA", "2");
            text = text.Replace("EIN", "4");
            text = text.Replace("AIN", "4");
            text = text.Replace("EIM", "4");
            text = text.Replace("AIM", "4");

            // Remplacement du son É:
            text = text.Replace("AI", "Y");
            text = text.Replace("EI", "Y");
            text = text.Replace("ER", "YR");
            text = text.Replace("ESS", "YS");
            text = text.Replace("ET", "YT");
            text = text.Replace("EZ", "YZ");


            // Remplacer les groupes de 2 lettres suivantes (son â..anâ.. et â..inâ..), sauf s"ils sont suivi par une lettre a, e, i o, u ou un son 1 à  4 :
            text = Regex.Replace(text, @"AN([^A|E|I|O|U|1|2|3|4])", @"1$1");
            text = Regex.Replace(text, @"ON([^A|E|I|O|U|1|2|3|4])", @"1$1");
            text = Regex.Replace(text, @"AM([^A|E|I|O|U|1|2|3|4])", @"1$1");
            text = Regex.Replace(text, @"EN([^A|E|I|O|U|1|2|3|4])", @"1$1");
            text = Regex.Replace(text, @"EM([^A|E|I|O|U|1|2|3|4])", @"1$1");
            text = Regex.Replace(text, @"IN([^A|E|I|O|U|1|2|3|4])", @"4$1");


            // Remplacer les s par des z s’ils sont suivi et précédés des lettres a, e, i, o,u ou d’un son 1 à 4
            text = Regex.Replace(text, @"([A|E|I|O|U|Y|1|2|3|4])S([A|E|I|O|U|Y|1|2|3|4])", @"$1Z$2");

            // Remplacer les groupes de 2 lettres suivants :
            text = text.Replace("OE", "E");
            text = text.Replace("EU", "E");
            text = text.Replace("AU", "O");
            text = text.Replace("OI", "2");
            text = text.Replace("OY", "2");
            text = text.Replace("OU", "3");

            text = Regex.Replace(text, @"C([E|I])", "S$1");


            // Remplacer les groupes de lettres suivants
            text = text.Replace("CH", "5");
            text = text.Replace("SCH", "5");
            text = text.Replace("SH", "5");
            text = text.Replace("SS", "S");
            //r = r.Replace("SC", "S");

            // Remplacer les lettres ou groupe de lettres suivants :
            text = text.Replace("C", "K");
            text = text.Replace("Q", "K");
            text = text.Replace("QU", "K");
            text = text.Replace("GU", "K");
            text = text.Replace("GA", "KA");
            text = text.Replace("GO", "KO");
            text = text.Replace("GY", "KY");

            // Remplacer les lettres suivante :
            text = text.Replace("A", "O");
            text = text.Replace("D", "T");
            text = text.Replace("P", "T");
            text = text.Replace("J", "G");
            text = text.Replace("B", "F");
            text = text.Replace("V", "F");
            text = text.Replace("M", "N");


            // Supprimer les lettres dupliquées
            text = Regex.Replace(text, @"[^\w\s]|(.)(?=\1)", "");

            // Supprimer les terminaisons suivantes : t, x
            text = Regex.Replace(text, @"(.*)[T|X]$", @"$1");

            // Affecter à chaque lettre le code numérique correspondant en partant de la dernière lettre
            List<char> num = new List<char> { '1', '2', '3', '4', '5', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'N', 'O', 'R', 'S', 'T', 'U', 'W', 'X', 'Y', 'Z' };
            List<int> l = new List<int>();
            foreach (var c in text)
            {
                l.Add(num.IndexOf(c));
            }

            // Convertissez les codes numériques ainsi obtenu en un nombre de base 22 exprimé en virgule flottante.
            double res = 0;
            int i = 1;
            foreach (var n in l)
            {
                res += n * Math.Pow(22, -i);
                i++;
            }

            return res;
        }

    }
}
