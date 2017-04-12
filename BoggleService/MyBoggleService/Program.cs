using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Boggle
{
    class Program
    {
        static void Main()
        {
            HttpStatusCode status;
            Name name = new Name { Nickname = "Joe" };
            BoggleService service = new BoggleService();
            UserIDInfo user = service.SaveUserID(name, out status);
            Console.WriteLine(user.UserToken);
            Console.WriteLine(status.ToString());

            // This is our way of preventing the main thread from
            // exiting while the server is in use
            Console.ReadLine();
        }
    }
}
