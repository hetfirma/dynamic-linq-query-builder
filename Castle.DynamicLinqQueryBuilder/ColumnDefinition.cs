using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Defines the columns to be filtered against in the UI component of jQuery Query Builder
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        public ColumnDefinition()
        {
            PrettyOutputTransformer = o => o;
        }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [DataMember(Name = "label")]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [DataMember(Name = "field")]
        public string Field { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember(Name = "type")]
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        [DataMember(Name = "input")]
        public string Input { get; set; }
        /// <summary>
        /// Gets or sets the multiple.
        /// </summary>
        /// <value>
        /// The multiple.
        /// </value>
        [DataMember(Name = "multiple")]
        public bool? Multiple { get; set; }
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        [DataMember(Name = "values")]
        public object Values { get; set; }
        /// <summary>
        /// Gets or sets the operators.
        /// </summary>
        /// <value>
        /// The operators.
        /// </value>
        [DataMember(Name = "operators")]
        public List<string> Operators { get; set; }
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        [DataMember(Name = "template")]
        public string Template { get; set; }
        /// <summary>
        /// Gets or sets the item bank not filterable.
        /// </summary>
        /// <value>
        /// The item bank not filterable.
        /// </value>
        [DataMember(Name = "itembanknotfilterable")]
        public bool? ItemBankNotFilterable { get; set; }
        /// <summary>
        /// Gets or sets the item bank not column.
        /// </summary>
        /// <value>
        /// The item bank not column.
        /// </value>
        [DataMember(Name = "itembanknotcolumn")]
        public bool? ItemBankNotColumn { get; set; }
        /// <summary>
        /// Gets or sets the pretty output transformer.
        /// </summary>
        /// <value>
        /// The pretty output transformer.
        /// </value>
        [IgnoreDataMember]
        public Func<object, object> PrettyOutputTransformer { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the plugin to use.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        [DataMember(Name = "plugin")]
        public string Plugin { get; set; }

        /// <summary>
        /// Gets or sets the plugin configuration.
        /// </summary>
        /// <value>
        /// The object defining the plugin configuration.
        /// </value>
        [DataMember(Name = "plugin_config")]
        public object Plugin_config { get; set; }
    }
}
