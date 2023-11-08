using System.Reflection.PortableExecutable;
using System.Text;

namespace ContactsToCalendar
{
    class ContactsToCalendar
    {
        static void Main(string[] args)
        {
            string filepathIn;
            string filepathOut;

            GetFilepath(out filepathIn, out filepathOut);

            List<Contact> contacts = Contact.ParseContactList(filepathIn);
            Contact.ExportContacts(contacts, filepathOut);
        }

        static void GetFilepath(out string filepathIn, out string filepathOut)
        {
            string? input;
            StreamReader reader;

            while (true)
            {
                Console.WriteLine("Enter the full path of the vcf contacts file you'd like to import:");
                input = Console.ReadLine() ?? "";

                try
                {
                    reader = new StreamReader(input);
                    reader.Close();

                    filepathIn = input;
                    filepathOut = Path.GetDirectoryName(filepathIn) + "\\Calendar.ics";

                    return;
                }
                catch
                {
                    Console.Write("Invalid filepath. ");
                }
            }
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Birthday { get; set; }

        public Contact()
        {
            Name = "";
            Birthday = "";
        }

        public static List<Contact> ParseContactList(string filepathIn)
        {
            List<Contact> contacts = new List<Contact>();
            StreamReader reader = new StreamReader(filepathIn);
            string? lineIn;

            try
            {
                lineIn = reader.ReadLine();

                while (lineIn != null)
                {
                    string property;
                    string[] values;

                    ParseLine(lineIn, out property, out values);

                    if (property == "BEGIN")
                    {
                        Contact contact = ParseContact(ref reader);

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
            }

            return contacts;
        }

        public static void ParseLine(string line, out string property, out string[] values)
        {
            string[] substrings = line.Split(':', ';');

            property = substrings[0];
            values = new string[substrings.Length - 1];

            for (int i = 1; i < substrings.Length; i++)
            {
                values[i - 1] = substrings[i];
            }
        }

        public static Contact ParseContact(ref StreamReader reader)
        {
            Contact contact = new Contact();
            string? lineIn = reader.ReadLine();

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
            finally { }

            return contact;
        }

        public static void ExportContacts(List<Contact> contacts, string filepathOut)
        {
            StreamWriter writer = new StreamWriter(filepathOut);

            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("METHOD:PUBLISH");
            writer.WriteLine("PRODID:ContactsToCalendar v1.0");
            writer.WriteLine("VERSION:2.0");

            foreach (Contact contact in contacts)
            {
                contact.RoundBirthday();
                writer.Write(contact.Export());
            }

            writer.WriteLine("END:VCALENDAR");
            writer.Close();
        }

        public string Export()
        {
            StringBuilder sb = new StringBuilder();
            DateTime timestamp = DateTime.UtcNow;

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("Summary:" + Name + "'s Birthday");
            sb.AppendLine("UID:" + Guid.NewGuid().ToString());
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("TRANSP:TRANSPARENT");
            sb.AppendLine("RRULE:FREQ=YEARLY");
            sb.AppendLine("DTSTART;VALUE=DATE:" + Birthday.Replace("-", ""));
            sb.AppendLine("DTEND;VALUE=DATE:" + Birthday.Replace("-", ""));
            sb.AppendLine("DTSTAMP:" + timestamp.ToString("yyyyMMdd") + "T" + timestamp.ToString("hhMMss"));
            sb.AppendLine("END:VEVENT");

            return sb.ToString();
        }

        // Some calendars have maximum event age, this caps all birthdays to the year 2000
        public void RoundBirthday()
        {
            const string minDate = "2000";

            if (Int32.Parse(Birthday.Substring(0, 4)) < Int32.Parse(minDate))
            {
                StringBuilder sb = new StringBuilder(Birthday);

                for (int i = 0; i < 4; i++)
                {
                    sb[i] = minDate[i];
                }

                Birthday = sb.ToString();
            }
        }
    }
}