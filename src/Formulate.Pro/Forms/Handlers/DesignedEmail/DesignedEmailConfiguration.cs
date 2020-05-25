namespace Formulate.Pro.Forms.Handlers.DesignedEmail
{

    // Namespaces.
    using formulate.app.Forms.Handlers.Email;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The configuration for the designed email handler.
    /// </summary>
    public class DesignedEmailConfiguration : IEmailSenderRecipientConfiguration
    {

        #region Properties

        /// <summary>
        /// The email address of the sender.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// The email addresses of the recipients.
        /// </summary>
        public IEnumerable<string> Recipients { get; set; }

        /// <summary>
        /// The fields that will be used to ascertain the recipient email addresses.
        /// </summary>
        public IEnumerable<Guid> RecipientFields { get; set; }

        /// <summary>
        /// The type of email delivery (to, cc, bcc).
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The subject line.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Optional. The path to the Razor view file that will render the subject line
        /// of the email.
        /// </summary>
        public string SubjectRazorPath { get; set; }

        /// <summary>
        /// Required. The path to the Razor view file that will render the HTML email.
        /// </summary>
        public string HtmlEmailRazorPath { get; set; }

        /// <summary>
        /// Optional. The path to the Razor view file that will render the text email.
        /// </summary>
        public string TextEmailRazorPath { get; set; }

        #endregion

    }

}