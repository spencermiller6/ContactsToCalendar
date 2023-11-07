using System.Reflection.Metadata;
using System.Text;

Console.WriteLine("Program started");

const string filepathIn = "C:\\Users\\spenc\\Downloads\\ContactsIn.vcf";
const string filepathOut = "C:\\Users\\spenc\\Downloads\\CalendarOut.vcf";

StreamReader reader = new StreamReader(filepathIn);
//StreamWriter writer = new StreamWriter(filepathOut);

Console.WriteLine("Files opened");

string? lineIn;
StringBuilder sb = new StringBuilder();
List<Contact> contacts = new List<Contact>();

try
{
    lineIn = reader.ReadLine();

    while(lineIn != null)
    {
        string property;
        string[] values;

        ParseLine(lineIn, out property, out values);

        if (property == "BEGIN")
        {
            lineIn = reader.ReadLine();

            Contact contact = ParseContact();

            if (!String.IsNullOrEmpty(contact.Birthday))
            {
                contacts.Add(contact);
            }
        }

        lineIn = reader.ReadLine();
    }
}
finally
{ 
    reader.Close();
    //writer.Close();
}

//foreach (Contact contact in contacts)
//{
//    string exportedContact = contact.Export();
//    writer.Write(exportedContact);
//}

Contact ParseContact()
{
    Contact contact = new Contact();

    try
    {
        while (lineIn != null)
        {
            string property;
            string[] values;

            ParseLine(lineIn, out property, out values);

            switch (property)
            {
                case "FN":
                    contact.Name = values[0];
                    break;
                case "BDAY":
                    contact.Birthday = values[1];
                    break;
                case "END": return contact;
                default: break;
            }

            lineIn = reader.ReadLine();
        }
    }
    finally
    {
    }

    return contact;
}

void ParseLine(string line, out string property, out string[] values)
{
    string[] substrings = line.Split(':', ';');

    property = substrings[0];
    values = new string[substrings.Length - 1];

    for (int i = 1; i <  substrings.Length; i++)
    {
        values[i - 1] = substrings[i];
    }
}

public class Contact
{
    public string Name;
    public string Birthday;

    //public string Export()
    //{
    //    StringBuilder sb = new StringBuilder();



    //    string[] substrings = lineIn.Split(':', ';');

    //    while (true)
    //    {
    //        string property;
    //        string[] values;
    //        ParseLine()
    //    }

    //    foreach (string substring in substrings)
    //    {
    //        if (substring == "BEGIN")
    //        {
    //            sb.Append("BEGIN:VCALENDAR\n");
    //            break;
    //        }

    //        if (substring == "END")
    //        {
    //            sb.Append("END:VCALENDAR\n");

    //        }
    //    }



    //    return sb.ToString();
    //}
}