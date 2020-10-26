using System;
using System.Text.RegularExpressions;

namespace CSharpIBAN
{
	class Land
	{
		public string ibanFormat, bbanFormat, ISOLändercode;
		MatchCollection bbanRegeln = null;
		int bbanLänge = 0;
		Regex feldRegExp = null;

		// Constructor
		public Land(string ibanFormat)
		{
			this.ibanFormat = ibanFormat;
			bbanFormat = ibanFormat.Substring(5);
			ISOLändercode = ibanFormat.Substring(0, 2);
		}

		public int Länge
		{
			get
			{
				// Falls die Länge schon ein mal berechnet wurde, ist die Länge größer als 0
				// Also muss man es nicht nochmal berechnen
				if (bbanLänge > 0)
				{
					return bbanLänge;
				}
				
				// Sucht Nummern im String, z.B. "1", "15"...
				Regex nummerRegExp = new Regex("([0-9]{1,2})");


				// Jedes Feld/Regel hat eine bestimmte Länge, z.b. "4!n" (4 Nummern)
				// Hier wird jede Regel rausgeschnitten
				MatchCollection feldLängen = nummerRegExp.Matches(bbanFormat);

				// Jede einzelne Länge wird zu einem Integer konvertiert und auf bbanLänge draufgerechnet
				foreach (Match feldLänge in feldLängen)
				{
					bbanLänge += Convert.ToInt32(feldLänge.Value);
				}

				return bbanLänge;
			}
		}

		MatchCollection Regeln
		{
			get
			{
				if (bbanRegeln != null)
				{
					return bbanRegeln;
				}

				// Teilt Format in Regeln auf, z.B. "1!n", "10!a"...
				Regex regelRegExp = new Regex("(\\d{1,2}\\!(?:n|a|c|e))");


            	bbanRegeln = regelRegExp.Matches(ibanFormat);
				return bbanRegeln;
			}
		}

		// Generiert ein nutzbares RegExp das alle relevanten Felder aus einer IBAN rausschneiden kann
		// Der einzige Nachteil ist das diese Felder nicht benannt werden können
		public Regex regExp
		{
			get
			{
				// Das erste Ergebniss bei einer schon generierten RegExp müsste der ISOLändercode sein
				// Wenn nicht, wurde es noch nicht generiert
				if (feldRegExp != null)
				{
					return feldRegExp;
				}
				
				// Zwei Regeln für ISOLändercode und Prüfziffer sind schon vorhanden, diese ändern sich nie
				string regExpString = "([A-Z]{2})";

				// Durch alle Regeln durchgehen und für jede Regel einen funktionierenden RegExp Teil generieren
				foreach (Match reg in Regeln)
				{
					// Aufteilen in Länge und mögliche Zeichen
					// z.B. r = ['4', 'n']
					string[] r = reg.Value.Split("!");
					string zeichen;

					switch (r[1])
					{
						case "n": zeichen = "0-9"; break;
						case "a": zeichen = "A-Z"; break;
						case "e": zeichen = " "; break;
						case "c":
						default:
							zeichen = "A-Za-z0-9";
							break;
					}

					regExpString += $"([{zeichen}]{{{r[0]}}})";
				}

				feldRegExp = new Regex(regExpString);
				return feldRegExp;
			}
		}

		public string Random()
		{
			string BBAN = "";
			string prüfziffer = "";

			foreach (Match reg in Regeln)
			{
				string[] r = reg.Value.Split("!");
				BBAN += Other.ZufälligeString(Convert.ToInt32(r[0]), r[1]);
			}

			prüfziffer = Other.PrüfzifferAusrechnen(ISOLändercode, BBAN);

			return ISOLändercode + prüfziffer + BBAN;
		}
	}
}