﻿using System;
using System.Net.Mail;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySQL_Backup {
    public static class UtilityFunctions {       
        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <param name="mbType">
        /// What kind of messagebox? error or information?
        /// </param>
        /// <param name="displayCancel">
        /// Should the MessageBox display a Cancel button as well as an OK button? 
        /// </param>
        /// <param name="caption">
        /// The MessageBox caption
        /// </param>
        /// <param name="message">
        /// Error message to display
        /// </param>
        /// <returns>
        /// Returns bool true if OK is clicked, otherwise it returns false.
        /// </returns> 
        public static bool DisplayMessage(string mbType, string message, string caption, bool displayCancel) {
            MessageBoxIcon icon;// = new MessageBoxIcon();
            switch (mbType) {
                case "error":
                    icon = MessageBoxIcon.Error;
                    break;
                case "information":
                    icon = MessageBoxIcon.Information;
                    break;
                case "question":
                    icon = MessageBoxIcon.Question;
                    break;
                default:
                    icon = MessageBoxIcon.Error;
                    break;
            }
            if (displayCancel) {
                return MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, icon) == DialogResult.OK;
            }
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);                
            return true;
        }
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <returns>
        /// Returns string of Sent email. or the exception message if there is a failure.
        /// </returns>
        public static string SendEmail(string[] parameters) {
            /*
        utilityFunctions.SendEmail(new string[]
        {
            tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup completed for the selected database(s) on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text
        });
        */
            /* var message = new MimeMessage();
        message.From.Add (new MailboxAddress (parameters[5]));
        message.To.Add (new MailboxAddress (parameters[4]));
        message.Subject = parameters[6];
        message.Body = new TextPart ("plain") {
            Text = @parameters[7]
        };
        using (var client = new SmtpClient ()) {
            // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
            client.ServerCertificateValidationCallback = (s,c,h,e) => true;
            client.Connect (parameters[0], Convert.ToInt16(parameters[1]), false);
            // Note: only needed if the SMTP server requires authentication
            if (parameters[2] != "" && parameters[3] != "") {
                client.Authenticate(parameters[2],parameters[3]);
            }
            client.Send (message);
            client.Disconnect (true);
        }*/
            // Create SMTP client
            var client = new SmtpClient(parameters[0]);
            if (parameters[1] != "") {
                client.Port = Convert.ToInt16(parameters[1]);
            }
            // Specify the message content.
            var message = new MailMessage(new MailAddress(parameters[5]), new MailAddress(parameters[4]))
            {
                Body = parameters[7],
                Subject = parameters[6]
            };
            if (parameters[2] != "" && parameters[3] != "") {
                client.Credentials = new System.Net.NetworkCredential(parameters[2],parameters[3]);
            }           
            // Try to send the message
            try {
                client.Send(message);
            }
            catch (SmtpException ex){
                message.Dispose();
                return ex.Message;
            }            
            // Clean up.
            message.Dispose();
            return "OK";
        }      
        /// <summary>
        /// Validates an email address
        /// </summary>
        /// <returns>
        /// Returns a boolean
        /// </returns>      
        public static bool ValidEmailAddress(string txtEmailId, out string errorMsg) {
            // Confirm that the e-mail address string is not empty. 
            if (txtEmailId.Length == 0) {
                errorMsg = "e-mail address is required to send email.";
                return true;
            }
            // Confirm that there is an "@" and a "." in the e-mail address, and in the correct order.
            if (txtEmailId.IndexOf("@", StringComparison.Ordinal) > -1) {
                if (txtEmailId.IndexOf(".", txtEmailId.IndexOf("@", StringComparison.Ordinal), StringComparison.Ordinal) > txtEmailId.IndexOf("@", StringComparison.Ordinal)) {
                    errorMsg = "";
                    return true;
                }
            }
            errorMsg = "e-mail address must be valid e-mail address format.\n" +
                       "For example 'someone@example.com' ";
            return false;
        }       
        /// <summary>
        /// Creates a connection to the MySQL database
        /// </summary>
        /// <returns>
        /// Returns a MySqlConnection object
        /// </returns>       
        public static MySqlConnection DbConnect(string connectionString) {         
            // MySQL Connections
            MySqlConnection connection = null;
            // Connect to the databases.
            try {
                // Try to connect to the MySQL server
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (MySqlException ex) {
                // Catch any exceptions.
                DisplayMessage("error",ex.Message,"Error",false);
            }            
            return connection;
        }
        /// <summary>
        /// Closes a connection to the database server
        /// </summary>
        /// <param name="connectionObject">
        /// The database conenction to be closed
        /// </param>
        /// <returns>
        /// A boolean if the database conenction was closed or not.
        /// </returns>
        public static bool DbClose(MySqlConnection connectionObject) {
            try {
                connectionObject.Close();
                return true;
            }
            catch (MySqlException ex) {
                DisplayMessage("error",ex.Message,"Error",false);
                return false;
            }
        }
        /// <summary>
        /// Updates specific text controls on a form to have a background color when they have focus.
        /// </summary>
        /// <param name="thisform">
        /// The form object that needs to be checked
        /// </param>
        /// <returns>
        /// none
        /// </returns>       
        public static void Textupdate(Form thisform) {
            var lastColorSaved = System.Drawing.Color.Empty;
            foreach (Control child in thisform.Controls) {
                if (child is GroupBox) {
                    foreach (Control tb in child.Controls) {
                        if (!(tb is TextBox) && !(tb is ComboBox) && !(child is MaskedTextBox)) continue;
                        tb.Enter += (s, e) =>
                        {
                            var control = (Control)s;
                            lastColorSaved = control.BackColor;
                            control.BackColor = System.Drawing.Color.LightYellow;
                        };
                        tb.Leave += (s, e) =>
                        {
                            ((Control)s).BackColor = lastColorSaved;
                        };
                    }
                }

                if (!(child is TextBox) && !(child is ComboBox) && !(child is MaskedTextBox)) continue;
                {
                    child.Enter += (s, e) =>
                    {
                        var control = (Control)s;
                        lastColorSaved = control.BackColor;
                        control.BackColor = System.Drawing.Color.LightYellow;
                    };
                    child.Leave += (s, e) =>
                    {
                        ((Control)s).BackColor = lastColorSaved;
                    };
                }
            }
        }
        /// <summary>
        /// Scheduler error and result codes
        /// </summary>
        /// <param name="error">
        /// Error code
        /// </param>
        /// <returns>
        /// String type message
        /// </returns>
     
        public static string SchedulerErrors(string error) {
            string message;
            switch (error) {
                case "0":
                    message = "Operation completed successfully";
                    break;
                case "267008":
                    message = "Task is ready";
                    break;
                case "267009":
                    message = "Task is running";
                    break;
                case "267010":
                    message = "Task is disabled";
                    break;
                case "267011":
                    message = "Task has never run";
                    break;
                case "267012":
                    message = "Task has no more runs";
                    break;
                case "267013":
                    message = "Task is not scheduled";
                    break ;
                case "267014":
                    message = "Task terminated by user";
                    break;
                case "267015":
                    message = "Task has no valid triggers";
                    break;
                case "267016":
                    message = "No trigger run times";
                    break;
                case "2147750665":
                    message = "Trigger not found";
                    break;
                case "2147750666":
                    message = "Task not ready";
                    break;
                case "2147750667":
                    message = "Task not running";
                    break;
                case "2147750668":
                    message = "Scheduler service not installed";
                    break;
                case "2147750669":
                    message = "Cannot open task";
                    break;
                case "2147750670":
                    message = "Invalid task";
                    break;
                case "2147750671":
                    message = "Account information not set";
                    break;
                case "2147750672":
                    message = "Account name not found";
                    break;
                case "2147750673":
                    message = "Account database corrupted";
                    break;
                case "2147750674":
                    message = "No security services available";
                    break;
                case "2147750675":
                    message = "Unknown object version";
                    break;
                case "2147750676":
                    message = "Unsupported account options";
                    break;
                case "2147750677":
                    message = "Scheduler service not running";
                    break;
                case "2147750678":
                    message = "Malformed task XML";
                    break;
                case "2147750679":
                    message = "Task XML unexpected namespace";
                    break;
                case "2147750680":
                    message = "Invalid value in task XML";
                    break;
                case "2147750681":
                    message = "Missing node in task XML";
                    break;
                case "2147750682":
                    message = "Malformed task XML";
                    break;
                case "267035":
                    message = "Some triggers failed";
                    break;
                case "267036":
                    message = "Batch logon problems.";
                    break;
                case "2147750685":
                    message = "Task XML contains too many nodes";
                    break;
                case "2147750686":
                    message = "Schedule past end boundary";
                    break;
                case "2147750687":
                    message = "Task already running";
                    break;
                case "2147750688":
                    message = "User not logged in";
                    break;
                case "2147750689":
                    message = "Task image is corrupt";
                    break;
                case "2147750690":
                    message = "Scheduler service not available";
                    break;
                case "2147750691":
                    message = "Scheduler service too busy";
                    break;
                case "2147750692":
                    message = "Task was attempted but failed";
                    break;
                case "267045":
                    message = "Task has been queued";
                    break;
                case "2147750694":
                    message = "Task is disabled";
                    break;
                case "2147750695":
                    message = "Task not V1 compatible";
                    break;
                case "2147750696":
                    message = "Task cannot start on demand";
                    break;
                default :
                    message = "Unknown error occured";
                    break;
            }
            return message;
        }
    }
}