using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    class BoggleClientApplicationContext : System.Windows.Forms.ApplicationContext
    {
        // Number of open forms
        private int windowCount = 0;

        // Singleton ApplicationContext
        private static BoggleClientApplicationContext context;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private BoggleClientApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static BoggleClientApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new BoggleClientApplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        public void RunNew()
        {
            // Create the window and the controller
            BoggleClientWindow window = new BoggleClientWindow();
            new BoggleClientController(window);

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }
    }
}
