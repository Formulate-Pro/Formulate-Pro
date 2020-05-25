namespace Formulate.Pro.Forms.Handlers.DesignedEmail
{

    // Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq.Expressions;

    /// <summary>
    /// An ExpandoObject that doesn't throw errors when attempting to access a property that was
    /// never set.
    /// </summary>
    public class SafeExpandoObject : IDynamicMetaObjectProvider
    {

        #region Properties

        /// <summary>
        /// The values set on this instance.
        /// </summary>
        private Dictionary<string, object> Values { get; set; }

        /// <summary>
        /// The value to use when a value isn't found.
        /// </summary>
        private object FallbackValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="fallbackValue">
        /// The instance to use in the event a value isn't found.
        /// </param>
        public SafeExpandoObject(object fallbackValue = null)
        {
            Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            FallbackValue = fallbackValue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the value for the member with the specified name.
        /// </summary>
        /// <param name="name">
        /// The member name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            var disallowedChars = @" ?.-!*#$&()+=/\{}:;'""<>,~`".ToCharArray();
            foreach (var disallowedChar in disallowedChars)
            {
                name = name.Replace(disallowedChar.ToString(), string.Empty);
            }
            Values[name] = value;
        }

        /// <summary>
        /// Get a member value.
        /// </summary>
        /// <param name="binder">
        /// The member binder.
        /// </param>
        /// <param name="result">
        /// The result (may be the fallback value).
        /// </param>
        /// <returns>
        /// The member value, or the fallback value.
        /// </returns>
        public bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Values.ContainsKey(binder.Name) ? Values[binder.Name] : FallbackValue;
            return true;
        }

        /// <summary>
        /// Sets a member to the specified value.
        /// </summary>
        /// <param name="binder">
        /// The member binder information.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <returns>
        /// Always returns true.
        /// </returns>
        public bool TrySetMember(SetMemberBinder binder, object value)
        {
            Values[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Returns a meta object for an expression.
        /// </summary>
        /// <param name="e">
        /// The expression.
        /// </param>
        /// <returns>
        /// The meta object.
        /// </returns>
        /// <remarks>
        /// MetaObject comes from this NuGet package: https://github.com/beccasaurus/MetaObject
        /// </remarks>
        public DynamicMetaObject GetMetaObject(Expression e) => new MetaObject(e, this);

        /// <summary>
        /// Returns the underlying values.
        /// </summary>
        /// <returns>
        /// The values.
        /// </returns>
        public ReadOnlyDictionary<string, object> GetRawValues() => new ReadOnlyDictionary<string, object>(Values);

        #endregion

    }

}