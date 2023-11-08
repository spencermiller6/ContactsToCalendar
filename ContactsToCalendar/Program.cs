using System.Text;

namespace ContactsToCalendar
{
    class ContactsToCalendar
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program started");

            const string filepathIn = "C:\\Users\\spenc\\Downloads\\ContactsIn.vcf";
            const string filepathOut = "C:\\Users\\spenc\\Downloads\\CalendarOut.ics";

            StreamReader reader = new StreamReader(filepathIn);
            StreamWriter writer = new StreamWriter(filepathOut);

            Console.WriteLine("Files opened");

            List<Contact> contacts = Contact.ParseContactList(filepathIn);

            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("METHOD:PUBLISH");
            writer.WriteLine("PRODID:ContactsToCalendar v0.1");
            writer.WriteLine("VERSION:2.0");

            foreach (Contact contact in contacts)
            {
                string exportedContact = contact.Export();
                writer.Write(exportedContact);
            }

            writer.WriteLine("END:VCALENDAR");
            writer.Close();
        }
    }

    public class Contact
    {
        private string? _birthday;
        public string? Name;
        public string? Birthday
        {
            get => _birthday;
            set
            {
                string minDate = "2000";

                if (Int32.Parse(value.Substring(0, 4)) < Int32.Parse(minDate))
                {
                    StringBuilder sb = new StringBuilder(value);

                    for (int i = 0; i < 4; i++)
                    {
                        sb[i] = minDate[i];
                    }

                    _birthday = sb.ToString();
                }
                else
                {
                    _birthday = value;
                }
            }
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
            finally
            {
            }

            return contact;
        }

        public string Export()
        {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("Summary:" + Name + "'s Birthday");
            sb.AppendLine("UID:" + Guid.NewGuid().ToString());
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("TRANSP:TRANSPARENT");
            sb.AppendLine("RRULE:FREQ=YEARLY");
            sb.AppendLine("DTSTART;VALUE=DATE:" + Birthday.Replace("-", ""));
            sb.AppendLine("DTEND;VALUE=DATE:" + Birthday.Replace("-", ""));
            sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMdd") + "T000000");
            sb.AppendLine("END:VEVENT");

            return sb.ToString();
        }
    }
}