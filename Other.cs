using System;
using System.Numerics;

namespace CSharpIBAN
{
	class Other
	{
		static Random random = new Random();
		// Konvertiert alle Buchstaben in str in Nummern und gibt eine neue konvertierte String ab
		public static string BuchstabenInNummernUmwandeln(string str)
		{
			var newStr = "";

			foreach (var z in str)
			{
				if (char.IsLetter(z))
				{
					var index = ((int) z % 32) + 9;
					newStr += index;
				}
				else
				{
					newStr += z;
				}
			}

			return newStr;
		}

		// Rechnet die Prüfziffer aus ISOLandCode und BBAN aus
		public static string PrüfzifferAusrechnen(string ISOLändercode, string BBAN)
		{
			string prüfsumme = BuchstabenInNummernUmwandeln(BBAN + ISOLändercode + "00");
			
			BigInteger bigIntPrüfsumme = BigInteger.Parse(prüfsumme);

			// 98 - (Prüfsumme % 97)
			BigInteger bigIntPrüfziffer = 98 - BigInteger.Remainder(bigIntPrüfsumme, 97);

			string prüfziffer = bigIntPrüfziffer.ToString();

			// Eine Null hinzufügen fall das Ergebniss nur z.B. '8' ist
			if (prüfziffer.Length == 1) {
				prüfziffer = "0" + prüfziffer;
			}

			return prüfziffer;
		}

		// Generiert zufällige Strings aus 4 verschiedenen Sets
		public static string ZufälligeString(int länge, string alphabet)
		{
			var AlphabetN = "0123456789";
			var AlphabetA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var AlphabetC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxz0123456789";
			string ausgewähltesAlphabet;
			var generiert = "";

			// Das Set auswählen
			switch (alphabet)
			{
				case "n": ausgewähltesAlphabet = AlphabetN; break;
				case "a": ausgewähltesAlphabet = AlphabetA; break;
				case "c": ausgewähltesAlphabet = AlphabetC; break;
				// e bedeutet leere Stellen, also wird eine String mit 'länge' leeren Stellen ausgegeben
				case "e":
				default:
					while (länge > generiert.Length)
					{
						generiert += " ";
					}
					return generiert;
			}

			// Zufällige Positionen 'länge'-mal generieren, und das Zeichen bei dieser Position zu 'generiert' hinzufügen
			for (int i = 0; i < länge; i++)
			{
				generiert += ausgewähltesAlphabet.Substring(random.Next(ausgewähltesAlphabet.Length), 1);
			}

			return generiert;
		}
	}
}