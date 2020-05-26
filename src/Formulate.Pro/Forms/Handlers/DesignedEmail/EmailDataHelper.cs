namespace Formulate.Pro.Forms.Handlers.DesignedEmail
{

    // Namespaces.
    using formulate.core.Types;
    using System.IO;
    using System.Net.Mail;

    /// <summary>
    /// Helps with operations common to email data.
    /// </summary>
    public class EmailDataHelper
    {

        #region Properties

        /// <summary>
        /// The mail message.
        /// </summary>
        private MailMessage MailMessage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="mailMessage">
        /// The mail message this helper operate on.
        /// </param>
        public EmailDataHelper(MailMessage mailMessage)
        {
            MailMessage = mailMessage;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the specified file to the email.
        /// </summary>
        /// <param name="file">
        /// The file to attach.
        /// </param>
        public void AttachFile(FileFieldSubmission file)
        {
            if (file == null || file.FileData == null || file.FileData.Length == 0)
            {
                return;
            }
            var dataStream = new MemoryStream(file.FileData);
            MailMessage.Attachments.Add(new Attachment(dataStream, file.FileName));
        }

        #endregion

    }

}