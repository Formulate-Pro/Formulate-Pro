namespace Formulate.Pro.Forms.Handlers.DesignedEmail
{

    //  Namespaces.
    using formulate.app.Forms;
    using formulate.app.Forms.Handlers.Email;
    using formulate.app.Helpers;
    using formulate.app.Managers;
    using formulate.core.Extensions;
    using Newtonsoft.Json.Linq;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Hosting;
    using Usage;

    /// <summary>
    /// Facilitates sending designed emails using Razor syntax.
    /// </summary>
    public class DesignedEmailHandler : EmailHandler
    {

        #region Private Properties

        /// <summary>
        /// The RazorEngine service used to convert Razor to text.
        /// </summary>
        private static IRazorEngineService RazorService { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The AngularJS directive that allows the handler configuration to be edited
        /// in the back office.
        /// </summary>
        public override string Directive => "formulate-designed-email-handler";

        /// <summary>
        /// The GUID that uniquely identifies this handler.
        /// </summary>
        public override Guid TypeId => new Guid("15BE7A7D179D40A59AD034B71DAC52D4");

        /// <summary>
        /// The name shown when selecting this handler in the back office.
        /// </summary>
        public override string TypeLabel => "Designed Email";

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static DesignedEmailHandler()
        {

            // Configure RazorEngine.
            var razorConfig = new TemplateServiceConfiguration()
            {
                Debug = Debugger.IsAttached
            };
            RazorService = RazorEngineService.Create(razorConfig);

        }

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="configurationManager">
        /// The configuration manager.
        /// </param>
        public DesignedEmailHandler(IConfigurationManager configurationManager) : base(configurationManager)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepares to handle the form submission.
        /// </summary>
        /// <param name="context">
        /// The form submission context.
        /// </param>
        /// <param name="configuration">
        /// The handler configuration.
        /// </param>
        public override void PrepareHandleForm(FormSubmissionContext context, object configuration)
        {

            // Track usage of this feature.
            UsageTracker.TrackDesignedEmail();

            // Boilerplate.
            base.PrepareHandleForm(context, configuration);

        }

        /// <summary>
        /// Handles a form submission (sends an email).
        /// </summary>
        /// <param name="context">
        /// The form submission context.
        /// </param>
        /// <param name="configuration">
        /// The handler configuration.
        /// </param>
        public override void HandleForm(FormSubmissionContext context, object configuration)
        {

            // Prepare the message.
            var message = PrepareEmailMessage(context, configuration as IEmailSenderRecipientConfiguration);
            if (message == null)
            {
                return;
            }

            // Variables.
            var config = configuration as DesignedEmailConfiguration;
            var form = context.Form;
            var data = context.Data;
            var dataForMessage = data;
            var files = context.Files;
            var filesForMessage = files;
            var payload = context.Payload;
            var extraContext = context.ExtraContext;
            var plainTextBody = default(string);
            var htmlBody = default(string);

            // Combine all the email recipients into a single array.
            var emailDataRecipients = message.To.ToArray()
                .Concat(message.CC.ToArray())
                .Concat(message.Bcc.ToArray())
                .Select(x => x.Address).Distinct()
                .OrderBy(x => x).ToArray();

            // Create the email data to be passed to the Razor view.
            var emailData = new EmailData(GetFormValues(context))
            {
                MailMessage = message,
                SenderEmail = config.SenderEmail,
                RecipientEmails = emailDataRecipients,
                Subject = config.Subject,
                Message = config.Message,
                Helper = new EmailDataHelper(message)
            };

            // Generate the email subject.
            var path = GetViewPath(config.SubjectRazorPath);
            var contents = path == null
                ? RazorViewIncorrectMarkup(config.HtmlEmailRazorPath)
                : File.ReadAllText(path);
            var templateSource = path == null
                ? null
                : new LoadedTemplateSource(contents, path);
            var key = GetViewKey(path, contents);
            message.Subject = path == null
                ? config.Subject
                : (RazorService.RunCompile(templateSource, key, emailData.GetType(), emailData) ?? "")
                    .Trim()
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

            // Generate the HTML for the message.
            path = GetViewPath(config.HtmlEmailRazorPath);
            contents = path == null
                ? RazorViewIncorrectMarkup(config.HtmlEmailRazorPath)
                : File.ReadAllText(path);
            templateSource = new LoadedTemplateSource(contents, path);
            key = GetViewKey(path, contents);
            htmlBody = RazorService.RunCompile(templateSource, key, emailData.GetType(), emailData);

            // Generate the text for the message.
            path = GetViewPath(config.TextEmailRazorPath);
            if (path != null)
            {
                contents = File.ReadAllText(path);
                templateSource = new LoadedTemplateSource(contents, path);
                key = GetViewKey(path, contents);
                plainTextBody = RazorService.RunCompile(templateSource, key, emailData.GetType(), emailData);
            }

            // Add plain text alternate view.
            var mimeType = new ContentType(MediaTypeNames.Text.Plain);
            var emailView = AlternateView.CreateAlternateViewFromString(plainTextBody ?? "", mimeType);
            message.AlternateViews.Add(emailView);

            // Add HTML alternate view.
            mimeType = new ContentType(MediaTypeNames.Text.Html);
            emailView = AlternateView.CreateAlternateViewFromString(htmlBody ?? "", mimeType);
            message.AlternateViews.Add(emailView);

            // Send email.
            using (var client = new SmtpClient())
            {
                client.Send(message);
            }

        }

        /// <summary>
        /// Deserializes the configuration for an email handler.
        /// </summary>
        /// <param name="configuration">
        /// The serialized configuration.
        /// </param>
        /// <returns>
        /// The deserialized configuration.
        /// </returns>
        public override object DeserializeConfiguration(string configuration)
        {

            // Variables.
            var recipients = new List<string>();
            var recipientFields = new List<Guid>();
            var fieldsToInclude = new List<Guid>();
            var config = new DesignedEmailConfiguration()
            {
                Recipients = recipients,
                RecipientFields = recipientFields
            };
            var configData = JsonHelper.Deserialize<JObject>(configuration);
            var dynamicConfig = configData as dynamic;
            var properties = configData.Properties().Select(x => x.Name);
            var propertySet = new HashSet<string>(properties);

            // Get recipients.
            if (propertySet.Contains("recipients"))
            {
                foreach (var recipient in dynamicConfig.recipients)
                {
                    recipients.Add(recipient.email.Value as string);
                }
            }

            // Get email recipient fields.
            if (propertySet.Contains("recipientFields"))
            {
                foreach (var recipient in dynamicConfig.recipientFields)
                {
                    recipientFields.Add(GuidHelper.GetGuid(recipient.id.Value as string));
                }
            }

            // Get simple properties.
            if (propertySet.Contains("deliveryType"))
            {
                config.DeliveryType = dynamicConfig.deliveryType.Value as string;
            }
            if (propertySet.Contains("senderEmail"))
            {
                config.SenderEmail = dynamicConfig.senderEmail.Value as string;
            }
            if (propertySet.Contains("message"))
            {
                config.Message = dynamicConfig.message.Value as string;
            }
            if (propertySet.Contains("subject"))
            {
                config.Subject = dynamicConfig.subject.Value as string;
            }
            if (propertySet.Contains("subjectRazorPath"))
            {
                config.SubjectRazorPath = dynamicConfig.subjectRazorPath.Value as string;
            }
            if (propertySet.Contains("htmlEmailRazorPath"))
            {
                config.HtmlEmailRazorPath = dynamicConfig.htmlEmailRazorPath.Value as string;
            }
            if (propertySet.Contains("textEmailRazorPath"))
            {
                config.TextEmailRazorPath = dynamicConfig.textEmailRazorPath.Value as string;
            }

            // Return the email configuration.
            return config;

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the markup to be used when a razor view is not specified or is incorrect.
        /// </summary>
        /// <param name="path">
        /// The path that was configured.
        /// </param>
        /// <returns>
        /// The markup.
        /// </returns>
        private string RazorViewIncorrectMarkup(string path)
        {
            return $@"<!doctype html>
                <html>
                    <head>
                        <title>Error</title>
                    </head>
                    <body>
                        <h1>Error</h1>
                        <p>Razor view path unspecified or incorrect.</p>
                        <p>Path: ""{HttpUtility.HtmlEncode(path)}""</p>
                    </body>
                </html>";
        }

        /// <summary>
        /// Returns the full path to the specified view.
        /// </summary>
        /// <param name="path">
        /// The partial view path (e.g., "Views\Contact Us.cshtml").
        /// </param>
        /// <returns>
        /// The full view path (e.g. "C:\Website\Views\Contact Us.cshtml").
        /// </returns>
        private string GetViewPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            path = path.Replace("/", @"\");
            if (!path.StartsWith("~"))
            {
                if (!path.StartsWith(@"\"))
                {
                    path = @"\" + path;
                }
                path = "~" + path;
            }
            path = HostingEnvironment.MapPath(path);
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary of form field values by field alias.
        /// </summary>
        /// <param name="context">
        /// The form submission context.
        /// </param>
        /// <returns>
        /// The dictionary of form field values.
        /// </returns>
        private Dictionary<string, object[]> GetFormValues(FormSubmissionContext context)
        {

            // Variables.
            var data = context.Data;
            var files = context.Files;
            var payload = context.Payload;
            var fieldsById = context.Form.Fields.ToDictionary(x => x.Id, x => x);
            var values = new Dictionary<string, object[]>();

            // Add the payload items.
            foreach (var item in payload)
            {
                values[item.Name] = new[] { item.Value };
            }

            // Add the field values.
            foreach (var item in data)
            {
                var field = fieldsById[item.FieldId];
                var name = string.IsNullOrWhiteSpace(field.Alias)
                    ? field.Name
                    : field.Alias;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                values[name] = item.FieldValues.MakeSafe().ToArray();
            }

            // Add the files.
            foreach (var item in files)
            {
                var field = fieldsById[item.FieldId];
                var name = string.IsNullOrWhiteSpace(field.Alias)
                    ? field.Name
                    : field.Alias;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                values[name] = new[] { item };
            }

            // Return the dictionary of values.
            return values;

        }

        /// <summary>
        /// Returns a key for the specified view path and view contents.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view.
        /// </param>
        /// <param name="contents">
        /// The file contents of the view.
        /// </param>
        /// <returns>
        /// The view key.
        /// </returns>
        /// <remarks>
        /// This is used so RazorEngine won't complain when you change the contents of a
        /// view file.
        /// </remarks>
        private string GetViewKey(string viewPath, string contents)
        {
            if (string.IsNullOrWhiteSpace(viewPath) || string.IsNullOrWhiteSpace(contents))
            {
                return "Unknown";
            }
            return viewPath + "_" + contents.GetHashCode().ToString();
        }

        #endregion

    }

}