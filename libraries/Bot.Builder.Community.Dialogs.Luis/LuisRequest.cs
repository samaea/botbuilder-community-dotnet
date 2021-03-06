namespace Bot.Builder.Community.Dialogs.Luis
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Rest;

    /// <summary>
    /// Object that contains all the possible parameters to build Luis request.
    /// </summary>
    public sealed class LuisRequest : ILuisOptions
    {
        /// <summary>
        /// Constructs an instance of the LuisReqeuest.
        /// </summary>
        /// <param name="query"> The text query.</param>
        public LuisRequest(string query)
        {
            this.Query = query;
            this.Log = true;
        }

        /// <summary>
        /// Gets or sets thee text query.
        /// </summary>
        /// <value>
        /// Thee text query.
        /// </value>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating if logging of queries to LUIS is allowed.
        /// </summary>
        /// <value>
        /// The flag indicating if logging of queries to LUIS is allowed.
        /// </value>
        public bool? Log { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating if spell checking is on.
        /// </summary>
        /// <value>
        /// The flag indicating if spell checking is on.
        /// </value>
        public bool? SpellCheck { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating if the staging endpoint should be used.
        /// </summary>
        /// <value>
        /// The flag indicating if the staging endpoint should be used.
        /// </value>
        public bool? Staging { get; set; }

        /// <summary>
        /// Gets or sets the time zone offset.
        /// </summary>
        /// <value>
        /// The time zone offset.
        /// </value>
        public double? TimezoneOffset { get; set; }

        /// <summary>
        /// Gets or sets the verbose flag.
        /// </summary>
        /// <value>
        /// The verbose flag.
        /// </value>
        public bool? Verbose { get; set; }

        /// <summary>
        /// Gets or sets the Bing Spell Check subscription key.
        /// </summary>
        /// <value>
        /// The Bing Spell Check subscription key.
        /// </value>
        public string BingSpellCheckSubscriptionKey { get; set; }

        /// <summary>
        /// Gets or sets any extra query parameters for the URL.
        /// </summary>
        /// <value>
        /// Any extra query parameters for the URL.
        /// </value>
        public string ExtraParameters { get; set; }

        /// <summary>
        /// Gets or sets the context id.
        /// </summary>
        /// <value>
        /// The context id.
        /// </value>
        [Obsolete("Action binding in LUIS should be replaced with code.")]
        public string ContextId { get; set; }

        /// <summary>
        /// Gets or sets the Force setting the parameter when using action binding.
        /// </summary>
        /// <value>
        /// The Force setting the parameter when using action binding.
        /// </value>
        [Obsolete("Action binding in LUIS should be replaced with code.")]
        public string ForceSet { get; set; }

        /// <summary>
        /// Build the Uri for issuing the request for the specified Luis model.
        /// </summary>
        /// <param name="model"> The Luis model.</param>
        /// <returns> The request Uri.</returns>
        public Uri BuildUri(ILuisModel model)
        {
            if (model.ModelID == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "id");
            }

            if (model.SubscriptionKey == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "subscriptionKey");
            }

            var queryParameters = new List<string>();
            queryParameters.Add($"subscription-key={Uri.EscapeDataString(model.SubscriptionKey)}");
            queryParameters.Add($"q={Uri.EscapeDataString(Query)}");
            UriBuilder builder;

            var id = Uri.EscapeDataString(model.ModelID);
            switch (model.ApiVersion)
            {
#pragma warning disable CS0612
                case LuisApiVersion.V1:
                    builder = new UriBuilder(model.UriBase);
                    queryParameters.Add($"id={id}");
                    break;
#pragma warning restore CS0612
                case LuisApiVersion.V2:
                    // v2.0 have the model as path parameter
                    builder = new UriBuilder(new Uri(model.UriBase, id));
                    break;
                default:
                    throw new ArgumentException($"{model.ApiVersion} is not a valid Luis api version.");
            }

            if (Log != null)
            {
                queryParameters.Add($"log={Uri.EscapeDataString(Convert.ToString(Log))}");
            }

            if (SpellCheck != null)
            {
                queryParameters.Add($"spellCheck={Uri.EscapeDataString(Convert.ToString(SpellCheck))}");
            }

            if (Staging != null)
            {
                queryParameters.Add($"staging={Uri.EscapeDataString(Convert.ToString(Staging))}");
            }

            if (TimezoneOffset != null)
            {
                queryParameters.Add($"timezoneOffset={Uri.EscapeDataString(Convert.ToString(TimezoneOffset))}");
            }

            if (Verbose != null)
            {
                queryParameters.Add($"verbose={Uri.EscapeDataString(Convert.ToString(Verbose))}");
            }

            if (!string.IsNullOrWhiteSpace(BingSpellCheckSubscriptionKey))
            {
                queryParameters.Add($"bing-spell-check-subscription-key={Uri.EscapeDataString(BingSpellCheckSubscriptionKey)}");
            }
#pragma warning disable CS0618
            if (ContextId != null)
            {
                queryParameters.Add($"contextId={Uri.EscapeDataString(ContextId)}");
            }

            if (ForceSet != null)
            {
                queryParameters.Add($"forceSet={Uri.EscapeDataString(ForceSet)}");
            }
#pragma warning restore CS0618
            if (ExtraParameters != null)
            {
                queryParameters.Add(ExtraParameters);
            }

            builder.Query = string.Join("&", queryParameters);
            return builder.Uri;
        }
    }
}
