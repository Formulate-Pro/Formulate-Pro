namespace Formulate.Pro.Forms.Handlers.DesignedEmail
{

    //  Namespaces.
    using formulate.core.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    /// <summary>
    /// Manages data used by an email.
    /// </summary>
    public class EmailData
    {

        #region Private Properties

        /// <summary>
        /// The field values, stored by field alias/name.
        /// </summary>
        private SafeExpandoObject PrivateValues { get; set; }

        /// <summary>
        /// The collection field values (e.g., checkbox list), stored by field alias/name.
        /// </summary>
        private SafeExpandoObject PrivateCollectionValues { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The mail message this email will be sent with.
        /// </summary>
        public MailMessage MailMessage { get; set; }

        /// <summary>
        /// The email address of the email sender.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// The email addresses of the email recipients.
        /// </summary>
        public IEnumerable<string> RecipientEmails { get; set; }

        /// <summary>
        /// The email subject line text (as specified in the back office configuration).
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The email message text (as specified in the back office configuration).
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The field values, stored by field alias/name.
        /// </summary>
        public dynamic Values => PrivateValues;

        /// <summary>
        /// The collection field values, stored by field alias/name. Each
        /// value you access will be a collection.
        /// </summary>
        public dynamic CollectionValues => PrivateCollectionValues;

        /// <summary>
        /// An object that will help with common email operations.
        /// </summary>
        public EmailDataHelper Helper { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="values">
        /// The field values to include in the email data.
        /// </param>
        public EmailData(Dictionary<string, object[]> values)
        {

            // Store the values for later access.
            PrivateCollectionValues = new SafeExpandoObject(new object[0]);
            PrivateValues = new SafeExpandoObject();
            foreach (var pair in values.MakeSafe())
            {
                PrivateCollectionValues.SetValue(pair.Key, pair.Value);
                PrivateValues.SetValue(pair.Key, pair.Value.MakeSafe().FirstOrDefault());
            }

        }

        #endregion

    }

}