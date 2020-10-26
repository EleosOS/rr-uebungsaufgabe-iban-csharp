using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpIBAN
{
	class Program
	{
		static Dictionary<string, Land> Länder = new Dictionary<string, Land>();
		static Random random = new Random();

		// Alle Formate für die Länder
		static string[] formatArr =
		{
			"AD2!n4!n4!n12!c",
			"HU2!n3!n4!n1!n15!n1!n",
			"CH2!n5!n12!c",
			"DE2!n8!n10!n"
		};

		static void Main(string[] args)
		{
			string methode = args[0];
			string eingabe1 = args[1];
			string eingabe2 = args[2];

			switch (methode.ToLower())
			{
				case "ibankorrekt":
					return IBANKorrekt(eingabe1)
				default:
			}
		}

		static void LänderRegistrieren()
		{
			// Durch alle Formate durchgehen und alle Länder registrieren
			foreach (string format in formatArr)
			{
				string ISOLändercode = format.Substring(0, 2);

				Länder.Add(ISOLändercode, new Land(format));
			}
		}

		static bool IBANKorrekt(string iban)
		{
			string ländererkennung, prüfziffer, bban, prüfsumme;
			BigInteger bigIntPrüfsumme;
			BigInteger zusammenrechnung;

			// IBAN können nie länger als 34 Stellen sein
			if (iban.Length > 34)
			{
				return false;
			}

			// Ländererkennung (ISO-Ländercode) rausschneiden
			ländererkennung = iban.Substring(0, 2);

			// Prüfziffer rausschneiden
			prüfziffer = iban.Substring(2, 2);
			// BBAN rausschneiden
			bban = iban.Substring(4);

			// Prüfsumme zusammensetzen, alle Buchstaben mit Position im Alphabet ersetzen
			prüfsumme = Other.BuchstabenInNummernUmwandeln(bban + ländererkennung + prüfziffer);

			// Die Prüfsumme ist viel zu groß für long, also muss in BigInt umgewandelt werden
			bigIntPrüfsumme = BigInteger.Parse(prüfsumme);

			// Mit Modulo 97 verrechnen
			zusammenrechnung = BigInteger.Remainder(bigIntPrüfsumme, 97);

			return zusammenrechnung.IsOne;
		}

		static string IBANErmitteln(string ISOLändercode, string blz, string kontoNr)
		{
			string IBAN;
			string prüfziffer;
			Land land;

			// Eingaben prüfen
			if (ISOLändercode.Length != 2)
			{
				throw new ArgumentException("Ländercode muss 2 Zeichen lang sein!");
			}
			else if (blz.Length == 0)
			{
				throw new ArgumentException("Die Bankleitzahl fehlt!");
			}
			else if (kontoNr.Length == 0)
			{
				throw new ArgumentException("Die Kontonummer fehlt!");
			}
			else
			{
				LänderRegistrieren();

				// Land von Länder holen
				if (Länder.TryGetValue(ISOLändercode, out Land value))
				{
					land = value;
				}
				else
				{
					throw new ArgumentException("Dieses Land wird für die Ermittlung der IBAN derzeit nicht unterstützt.");
				}
			}

			// blz und kontoNr zusammen dürfen nicht die max. Läge überschreiten
			if (land.Länge < (blz.Length + kontoNr.Length))
			{
				throw new ArgumentException("Die Eingaben sind zu groß!");
			}

			// Kontonummer mit Nullen füllen bis die BBAN die korrekte länge hat
			// Korrekte Länge = max. Länge - Bankleitzahl
			while (kontoNr.Length < (land.Länge - blz.Length))
			{
				kontoNr = "0" + kontoNr;
			}

			prüfziffer = Other.PrüfzifferAusrechnen(ISOLändercode, blz + kontoNr);

			IBAN = ISOLändercode + prüfziffer + blz + kontoNr;

			return IBAN;
		}

		static GroupCollection RelevanteFelderErmitteln(string iban)
		{
			// Die Funktion soll nur korrekte IBAN akzeptieren
			if (!IBANKorrekt(iban))
			{
				throw new ArgumentException("Die angegebene IBAN ist falsch!");
			}

			LänderRegistrieren();

			// Land von Länder holen
			Land land;

			if (Länder.TryGetValue(iban.Substring(0, 2), out Land value))
			{
				land = value;
			}
			else
			{
				throw new ArgumentException("Dieses Land wird für die Ermittlung der Kontonummer und Bankleitzahl derzeit nicht unterstützt.");
			}

			// Da die Funktion nach der IBAN Prüfung gerufen wird kann man davon ausgehen das sie korrekt ist.
			// Deswegen wird hier sorglos die IBAN nach dem Format aufgeteilt.
			MatchCollection matches = land.regExp.Matches(iban);

			if (matches.Count == 0)
			{
				throw new ArgumentException("Das Format der IBAN ist falsch!");
			}
			else
			{
				return matches[0].Groups;
			}
		}

		static string ZufälligeIBAN()
		{
			LänderRegistrieren();

			// Zufälliges Land auswählen und holen
			List<string> keyList = new List<string>(Länder.Keys);
			Land land = Länder[keyList[random.Next(keyList.Count)]];

			return land.Random();
		}
	}
}
