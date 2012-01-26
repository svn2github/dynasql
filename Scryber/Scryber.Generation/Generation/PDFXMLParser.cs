/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Scryber.Generation
{
    public class PDFXMLParser : IPDFParser
    {
        private static readonly string[] TextElements = new string[] { "B", "BR", "I", "b", "br", "i" };

        private const string PDFXProcessingInstructionsName = "pdfx";
        private const string ParserLogCategory = "XML PARSER";
        private const string InheritsAttributeName = "inherits";
        private const string CodeBehindAttributeName = "code-file";
        private const string FilePathAttributeName = "source";

        private object _root;

        public object RootComponent
        {
            get { return _root; }
            set { _root = value; }
        }

        private PDFGeneratorSettings _settings;

        public PDFGeneratorSettings Settings
        {
            get { return _settings; }
        }

        private object _mode = null;

        public ConformanceMode Mode
        {
            get
            {
                if (null == _mode)
                    return _settings.ConformanceMode;
                else
                    return (ConformanceMode)_mode;
            }
            set
            {
                _mode = value;
            }
        }

        public PDFXMLParser(PDFGeneratorSettings settings)
        {
            this._settings = settings;
        }

        public IPDFComponent Parse(string source, Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                return Parse(source, reader);
            }
        }

        public IPDFComponent Parse(string source, TextReader reader)
        {
            using (XmlReader xreader = XmlReader.Create(reader))
            {
                return this.Parse(source, xreader);
            }
        }

        public virtual IPDFComponent Parse(string source, XmlReader reader)
        {
            _mode = null; //clear any mode setting.

            IPDFComponent parsed = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.ProcessingInstruction)
                {
                    string name = reader.Name;
                    if (name == PDFXProcessingInstructionsName)
                        this.ParseProcessingInstructions(reader);
                }
                if (reader.NodeType == XmlNodeType.Element)
                {
                    LogBegin(TraceLevel.Message, "Beginning parse of XML file");
                    object value = ParseComponent(reader, true);
                    if (!(value is IPDFComponent))
                        throw new InvalidCastException(String.Format(Errors.CannotConvertObjectToType, value.GetType(), typeof(IPDFComponent)));

                    parsed = (IPDFComponent)value;
                    LogEnd(TraceLevel.Message, "Completed parse of XML file and parsed component '{0}'", parsed);
                    break;
                }
            }
            if (parsed is IPDFLoadableComponent)
            {
                IPDFLoadableComponent comp = parsed as IPDFLoadableComponent;
                comp.LoadedSource = source;
                comp.LoadType = ComponentLoadType.ReflectiveParser;
            }
            return parsed;
        }

        private object ParseComponent(XmlReader reader, bool isroot)
        {
            if (reader.NodeType != XmlNodeType.Element)
                throw new PDFParserException(Errors.CanOnlyParseComponentAsElement);
            
            bool isremote;
            ParserClassDefinition cdef = AssertGetClassDefinition(reader, out isremote);

            if (isremote)
                return this.ParseRemoteComponent(cdef, reader);
            else
                return ParseComplexComponent(reader, cdef, isroot);
        }

        /// <summary>
        /// Resolves and loads a remote referenced definition
        /// </summary>
        /// <param name="cdef"></param>
        /// <param name="resolver"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private object ParseRemoteComponent(ParserClassDefinition cdef, XmlReader reader)
        {
            string element = reader.LocalName;
            string ns = reader.NamespaceURI;
            bool empty = reader.IsEmptyElement;

            if (!reader.MoveToAttribute(FilePathAttributeName))
                throw new PDFParserException(String.Format(Errors.RequiredAttributeNoFoundOnElement, FilePathAttributeName, element));

            string path = reader.Value;
            object complex = this.Settings.Resolver(path);

           if (reader.HasAttributes && reader.AttributeCount > 1)
                this.ParseAttributes(complex, true, reader, cdef);

            if (!empty)
                this.ParseContents(complex, reader, element, ns, cdef);

            //this.ParseComplexComponent(complex, reader, cdef, resolver);
            return complex;
        }

        private object ParseComplexComponent(XmlReader reader, ParserClassDefinition cdef, bool isroot)
        {
            object value;
            LogBegin(TraceLevel.Debug, "Parsing found component '{0}' of type '{1}'", reader.Name, cdef.ClassType);

            if (reader.MoveToAttribute(InheritsAttributeName))
            {
                string typename = reader.Value;
                value = CreateInheritedInstance(typename, cdef);
                if (null == value) //fallback to default
                    value = this.CreateInstance(cdef);

                reader.MoveToFirstAttribute();
            }
            else
            {
                value = this.CreateInstance(cdef);
            }
            if (isroot && null == this.RootComponent)
                this.RootComponent = value;

            ParseComplexComponent(value, reader, cdef);

            LogEnd(TraceLevel.Debug, "Completed parsing of component of type '{0}'", cdef.ClassType);

            return value;
        }

        private object CreateInheritedInstance(string typename, ParserClassDefinition cdef)
        {
            LogAdd(TraceLevel.Debug, "Creating instance of inherited type '{0}'", typename);

            object created = null;

            if (string.IsNullOrEmpty(typename) == false)
            {
                Type found;
                try
                {
                    found = Type.GetType(typename, false);
                }
                catch (Exception ex)
                {
                    if (this.Mode == ConformanceMode.Strict)
                        throw new PDFParserException(String.Format(Errors.CannotCreateInstanceOfType, typename, ex.Message), ex);
                    else
                        LogAdd(TraceLevel.Error, Errors.CannotCreateInstanceOfType, typename, ex.Message);
                    //Default to returning an instance of the base type
                    return this.CreateInstance(cdef);
                }

                LogAdd(TraceLevel.Debug, "Loaded type from name");

                if (null == found || !cdef.ClassType.IsAssignableFrom(found))
                {
                    if (this.Mode == ConformanceMode.Strict)
                        throw new PDFParserException(String.Format(Errors.CannotCreateInstanceOfType, typename, "Cannot create or cast an object of type '" + typename + "'"));
                    else
                        LogAdd(TraceLevel.Error, Errors.CannotCreateInstanceOfType, typename, "Cannot create or cast an object of type '" + typename + "'");

                }
                else
                {
                    LogAdd(TraceLevel.Debug, "Type can be converted to base type '" + cdef.ClassType.FullName);

                    try
                    {
                        created = System.Activator.CreateInstance(found);
                    }
                    catch (Exception ex)
                    {
                        if (this.Mode == ConformanceMode.Strict)
                            throw new PDFParserException(String.Format(Errors.CannotCreateInstanceOfType, typename, ex.Message), ex);
                        else
                            LogAdd(TraceLevel.Error, Errors.CannotCreateInstanceOfType, typename, ex.Message);
                        //Default to returning an instance of the base type
                    }

                }
            }

            return created;

        }

        private void ParseComplexComponent(object component, XmlReader reader, ParserClassDefinition cdef)
        {
            string name = reader.LocalName;
            string ns = reader.NamespaceURI;

            bool isempty = reader.IsEmptyElement;

            if (reader.HasAttributes)
                ParseAttributes(component, false, reader, cdef);
            if (!isempty)
            {
                ParseContents(component, reader, name, ns, cdef);
            }
        }

        private void ParseAttributes(object container, bool isremotecomponent, XmlReader reader, ParserClassDefinition cdef)
        {
            if (!reader.MoveToFirstAttribute())
                return;
            do
            {
                string name = reader.LocalName;
                
                ParserPropertyDefinition attr;
                ParserEventDefinition evt;
                if (IsSpecialAttribute(reader, isremotecomponent))
                {
                    //skip the xmlns attributes
                }
                else if (cdef.Attributes.TryGetPropertyDefinition(name, out attr))
                {
                    string expression;
                    BindingType bindingtype = ParserHelper.IsBindingExpression(reader.Value, out expression);
                    if (bindingtype == BindingType.XPath)
                    {
                        GenerateXPathBindingExpression(container, cdef, attr, expression, bindingtype);
                    }
                    else if (bindingtype == BindingType.Code)
                    {
                        throw new NotSupportedException("No code type supported");
                    }
                    else
                    {
                        object actualValue = attr.GetValue(reader);
                        this.SetValue(container, actualValue, attr);
                    }
                }
                else if (cdef.Events.TryGetPropertyDefinition(name, out evt))
                {
                    string expression;
                    BindingType bindingtype = ParserHelper.IsBindingExpression(reader.Value, out expression);
                    if (bindingtype != BindingType.None)
                    {
                        if (this.Mode == ConformanceMode.Strict)
                            throw new PDFParserException(String.Format(Errors.CannotSpecifyBindingExpressionsOnEvents, expression, name));
                        else
                            LogAdd(TraceLevel.Error, Errors.CannotSpecifyBindingExpressionsOnEvents, expression, name);
                    }
                    else
                    {
                        AttachEventHandler(container, reader.Value, evt, cdef);
                    }
                }
                else if (this.Mode == ConformanceMode.Strict)
                    throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, cdef.ClassType, name, "attribute"));
                else
                    LogAdd(TraceLevel.Message, "Skipping unknown attribute '{0}' on type {1}", name, cdef.ClassType);

            } while (reader.MoveToNextAttribute());
        }

        private const string EventInvokeMethodName = "Invoke";

        private void AttachEventHandler(object container, string methodname, ParserEventDefinition evt, ParserClassDefinition cdef)
        {
            //The event handlers must be implemented on the root component
            Type roottype = this.RootComponent.GetType();

            Type evtType = evt.Event.EventHandlerType;
            System.Reflection.MethodInfo evtsignature = evtType.GetMethod(EventInvokeMethodName);

            List<Type> param = new List<Type>(2);
            foreach (System.Reflection.ParameterInfo pi in evtsignature.GetParameters())
            {
                param.Add(pi.ParameterType);
            }

            System.Reflection.MethodInfo declared = roottype.GetMethod(methodname,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null,
                param.ToArray(), null);

            if (null == declared)
            {
                if (this.Mode == ConformanceMode.Strict)
                    throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, roottype.FullName, methodname, "event handler"));
                else
                    LogAdd(TraceLevel.Error, Errors.ParsedTypeDoesNotContainDefinitionFor, roottype.FullName, methodname, "event handler method");
            }
            else
            {
                Delegate del = Delegate.CreateDelegate(evtType, this.RootComponent, declared);
                evt.Event.AddEventHandler(container, del);
            }

        }

        private bool IsSpecialAttribute(XmlReader reader, bool isremotecomponent)
        {
            if (reader.Depth == 1)
            {
                if (reader.LocalName == InheritsAttributeName)
                    return true;
                else if (reader.LocalName == CodeBehindAttributeName)
                    return true;
                //check code-behind and inherits
            }
            if (reader.Name.ToLower() == "xmlns" || reader.Name.ToLower().StartsWith("xmlns:"))
                return true;
            else if (isremotecomponent && (reader.LocalName.ToLower() == FilePathAttributeName))
                return true;
            else
                return false;
        }

        private void GenerateXPathBindingExpression(object container, ParserClassDefinition cdef, ParserPropertyDefinition attr, string expression, BindingType bindingtype)
        {
            //throw new NotImplementedException();
            if (container is IPDFBindableComponent)
            {
                IPDFBindableComponent bindable = (IPDFBindableComponent)container;
                PDFValueConverter conv = this.GetValueConverter(attr);
                ParserXPathBinding binding = ParserXPathBinding.Create(expression, conv, attr.PropertyInfo);
                bindable.DataBinding += new PDFDataBindEventHandler(binding.BindComponent);
            }
            else
                throw new NotSupportedException(String.Format(Errors.DatabindingIsNotSupportedOnType, cdef.ClassType.FullName));
        }

        private PDFValueConverter GetValueConverter(ParserPropertyDefinition attr)
        {
            PDFValueConverter valueconverter;
            if (ParserDefintionFactory.IsSimpleObjectType(attr.ValueType, out valueconverter))
            {
                return valueconverter;
            }
            else if (ParserDefintionFactory.IsCustomParsableObjectType(attr.ValueType, out valueconverter))
            {
                return valueconverter;
            }
            else
                throw new NotSupportedException(String.Format(Errors.ParserAttributeMustBeSimpleOrCustomParsableType, attr.Name, attr.ValueType));

        }

        
        private void ParseContents(object container, XmlReader reader, string element, string ns, ParserClassDefinition cdef)
        {
            
            LogAdd(TraceLevel.Debug, "Parsing container contents");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.LocalName == element && reader.NamespaceURI == ns)
                        break;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    ParserPropertyDefinition prop;
                    if (cdef.Elements.TryGetPropertyDefinition(reader.LocalName, out prop))
                    {

                        if (prop.ParseType == DeclaredParseType.SimpleElement)
                            this.ParseSimpleElement(container, reader, prop);
                        else if (prop.ParseType == DeclaredParseType.ArrayElement)
                            this.ParseInnerCollection(container, reader, prop);
                        else if (prop.ParseType == DeclaredParseType.ComplexElement)
                            this.ParseComplexType(container, reader, prop);
                        else if (prop.ParseType == DeclaredParseType.TempateElement)
                            this.ParseTemplateContent(container, reader, prop);
                        else if (this.Mode == ConformanceMode.Strict)
                            throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, cdef.ClassType, reader.LocalName, "element"));
                        else
                            reader.Skip();
                    }
                    else if (cdef.DefaultElement != null)
                    {
                        if (cdef.DefaultElement.Name == reader.LocalName && ns == reader.NamespaceURI)
                            this.ParseComplexType(container, reader, cdef.DefaultElement);
                        else if (cdef.DefaultElement.ParseType == DeclaredParseType.ArrayElement)
                        {
                            ParserArrayDefinition arry = (ParserArrayDefinition)cdef.DefaultElement;
                            object collection = InitArrayCollection(container, arry);
                            object component = ParseComponent(reader, false);
                            LogAdd(TraceLevel.Debug, "Adding component '{0}' to default collection in property {1} of type '{2}'", component, arry.PropertyInfo.Name, cdef.ClassType);
                            arry.AddToCollection(collection, component);

                        }
                        else if (this.Mode == ConformanceMode.Strict)
                            throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, cdef.ClassType, reader.LocalName, "element"));
                        else
                        {
                            LogAdd(TraceLevel.Message, "Skipping unknown element '{0}'", reader.Name);
                            reader.Skip();
                        }
                    }
                    else if (this.Mode == ConformanceMode.Strict)
                        throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, cdef.ClassType, reader.LocalName, "element"));
                    else
                    {
                        LogAdd(TraceLevel.Message, "Skipping unknown element '{0}'", reader.Name);
                        reader.Skip();
                    }

                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    if (cdef.DefaultElement != null && string.IsNullOrEmpty(cdef.DefaultElement.Name))
                    {
                        if (cdef.DefaultElement.ParseType == DeclaredParseType.SimpleElement)
                        {
                            ParserSimpleElementDefinition se = (ParserSimpleElementDefinition)cdef.DefaultElement;
                            object value = se.GetValue(reader);
                            this.SetValue(container, value, se);
                            break; //we have read past the end of the content of this container
                        }
                        else if (this.Mode == ConformanceMode.Strict)
                            throw new PDFParserException(String.Format(Errors.ParserAttributeMustBeSimpleOrCustomParsableType, cdef.DefaultElement.PropertyInfo.Name, cdef.ClassType));
                        else
                            LogAdd(TraceLevel.Message, "Skipping text content of property '" + cdef.DefaultElement.PropertyInfo.Name + "' on class '" + cdef.ClassType + ", because it is not a simple or parsable type");
                    }
                    else if (this.Mode == ConformanceMode.Strict)
                        throw new PDFParserException(String.Format(Errors.ParsedTypeDoesNotContainDefinitionFor, cdef.ClassType, reader.LocalName, "element"));
                    else
                    {
                        LogAdd(TraceLevel.Message, "Skipping unknown element '{0}'", reader.Name);
                        reader.Skip();
                    }

                }
            }
        }


        private string ReadAllContent(XmlReader reader, string endelement, string endns)
        {
            return reader.ReadContentAsString();
        }

        private void ParseSimpleElement(object container, XmlReader reader, ParserPropertyDefinition prop)
        {
            LogAdd(TraceLevel.Debug, "Parsing simple element '{0}' for property '{2}' on class '{3}'", reader.Name, prop.Name, prop.PropertyInfo.Name, prop.PropertyInfo.DeclaringType);

            object converted = prop.GetValue(reader);
            this.SetValue(container, converted, prop);
        }

        private void ParseInnerCollection(object container, XmlReader reader, ParserPropertyDefinition prop)
        {
            LogAdd(TraceLevel.Debug, "Parsing inner collection from element '{0}' for property '{2}' on class '{3}'", reader.Name, prop.Name, prop.PropertyInfo.Name, prop.PropertyInfo.DeclaringType);
            bool lastwastext = false;
            StringBuilder textString = new StringBuilder();

            ParserArrayDefinition arraydefn = (ParserArrayDefinition)prop;
            string endname = reader.LocalName;
            string endns = reader.NamespaceURI;

            object collection = InitArrayCollection(container, arraydefn);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (IsTextElement(reader))
                    {
                        this.AppendTextElement(reader, textString);
                        lastwastext = true;
                    }
                    else
                    {
                        if (lastwastext)
                        {
                            AppendTextToCollection(textString, arraydefn, collection);
                            textString.Length = 0;
                            lastwastext = false;
                        }
                        object inner = this.ParseComponent(reader, false);
                        arraydefn.AddToCollection(collection, inner);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (IsTextElement(reader))
                    {
                        this.AppendTextElement(reader, textString);
                        lastwastext = true;
                    }
                    if (reader.LocalName == endname && reader.NamespaceURI == endns)
                        break;
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    string val = reader.Value;
                    if (val.IndexOf('&') > -1)
                        val = System.Web.HttpUtility.HtmlEncode(val);

                    textString.Append(val);
                    lastwastext = true;
                }
            }

            if (lastwastext && textString.Length > 0)
            {
                AppendTextToCollection(textString, arraydefn, collection);
            }
        }

        private System.ComponentModel.PropertyDescriptor _textPropertyFromLiteral = null;
        private System.ComponentModel.PropertyDescriptor _formatPropertyFromLiteral = null;

        private void AppendTextToCollection(StringBuilder textString, ParserArrayDefinition arraydefn, object collection)
        {
            TextFormat format = TextFormat.XML;
            Type literaltype = this.Settings.TextLiteralType;
            object obj = CreateInstance(literaltype);
            if(!(obj is IPDFTextLiteral))
                throw new InvalidCastException(String.Format(Errors.CannotConvertObjectToType,literaltype, typeof(IPDFTextLiteral)));
            
            IPDFTextLiteral literal = (IPDFTextLiteral)obj;
            literal.Text = textString.ToString();
            literal.Format = format;

            arraydefn.AddToCollection(collection, literal);
        }

        

        private bool IsTextElement(XmlReader reader)
        {
            if (Array.IndexOf<string>(TextElements, reader.LocalName) > -1)
                    return true;
            else
                return false;
        }

        private void AppendTextElement(XmlReader reader, StringBuilder content)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                content.Append("<");
                content.Append(reader.LocalName);
                if (reader.IsEmptyElement)
                {
                    content.Append("/>");
                }
                else
                {
                    content.Append(">");
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                content.Append("</");
                content.Append(reader.LocalName);
                content.Append(">");
            }
        }

        private object InitArrayCollection(object container, ParserArrayDefinition arraydefn)
        {
            object collection = arraydefn.PropertyInfo.GetValue(container, null);
            if (null == collection)
            {
                LogAdd(TraceLevel.Debug, "Collection from property is null - creating instance of type '{0}'", arraydefn.ValueType);
                collection = arraydefn.CreateInstance();
                this.SetValue(container, collection, arraydefn);
            }
            return collection;
        }

        private void ParseComplexType(object container, XmlReader reader, ParserPropertyDefinition prop)
        {
            ParserClassDefinition cdef = ParserDefintionFactory.GetClassDefinition(prop.ValueType);

            bool created = false;
            object value = this.GetValue(container, prop);
            if (null == value)
            {
                value = this.CreateInstance(cdef);
                created = true;
            }
            this.ParseComplexComponent(value, reader, cdef);

            if(created)
                this.SetValue(container, value, prop);
        }

        private void ParseTemplateContent(object container, XmlReader reader, ParserPropertyDefinition prop)
        {
            IPDFComponent comp;
            if (container is IPDFComponent)
                comp = container as IPDFComponent;
            else
                throw new InvalidCastException(Errors.TemplateComponentParentMustBeContainer);

            string all = reader.ReadInnerXml();

            object gen = CreateInstance(this.Settings.TempateGeneratorType);
            IPDFTemplateGenerator tempgen = (IPDFTemplateGenerator)gen;
            tempgen.InitTemplate(comp, all, new XmlNamespaceManager(reader.NameTable));

            prop.PropertyInfo.SetValue(container, gen, null);
        }

        private ParserClassDefinition AssertGetClassDefinition(XmlReader reader, out bool isremote)
        {
            LogAdd(TraceLevel.Debug, "Looking for PDFComponent declared with local name '{0}' and namespace '{1}'", reader.LocalName, reader.NamespaceURI);
            
            ParserClassDefinition cdef = ParserDefintionFactory.GetClassDefinition(reader.LocalName, reader.NamespaceURI, out isremote);

            if (null == cdef)
                throw new PDFParserException(String.Format(Errors.NoTypeFoundWithPDFComponentNameInNamespace, reader.LocalName, reader.NamespaceURI));
            return cdef;
        }

        /// <summary>
        /// Gets the current value for the property of the specified instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private object GetValue(object instance, ParserPropertyDefinition property)
        {
            return property.PropertyInfo.GetValue(instance, null);
        }

        private void SetValue(object instance, object value, ParserPropertyDefinition property)
        {
            LogAdd(TraceLevel.Debug, "Setting value of property '{0}' on class {1} to value '{2}'", property.Name, property.PropertyInfo.DeclaringType, value);
            property.PropertyInfo.SetValue(instance, value, null);
        }

        //
        // Logging
        //

        

        //Default values of _dolog. Change with the pdfx processing instruction - parser-log="True|False"
#if DEBUG
        private bool _dolog = true;
#else
        private bool _dolog = false;
#endif

        internal bool DoLog
        {
            get { return _dolog; }
        }

        private void LogAdd(TraceLevel level, string message, params object[] args)
        {
            if (this.DoLog && this.Settings.Log.ShouldLog(level))
            {
                this.Settings.Log.Add(level, ParserLogCategory, SafeFormat(message, args));
            }
        }

        private void LogBegin(TraceLevel level, string message, params object[] args)
        {
            if (this.DoLog && this.Settings.Log.ShouldLog(level))
            {
                this.Settings.Log.Begin(level, SafeFormat(message, args));
            }
        }

        private void LogEnd(TraceLevel level, string message, params object[] args)
        {
            if (this.DoLog && this.Settings.Log.ShouldLog(level))
            {
                this.Settings.Log.End(level, SafeFormat(message, args));
            }
        }

        private string SafeFormat(string message, object[] args)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;
            else if (null == args || args.Length == 0)
                return message;
            else
            {
                try
                {
                    string formatted = string.Format(message, args);
                    message = formatted;
                }
                catch (Exception)
                {
                    //Intentional sinking of exception
                }
                return message;
            }
        }

        //
        // processing instructions
        //

        public void ParseProcessingInstructions(XmlReader reader)
        {
            string value = reader.Value;
            
            
            string mode = GetProcessingValue("parser-mode", value);
            if (!string.IsNullOrEmpty(mode) && Enum.IsDefined(typeof(ConformanceMode), mode))
            {
                this.Mode = (ConformanceMode)Enum.Parse(typeof(ConformanceMode), mode);
            }

            
            string level = GetProcessingValue("parser-log", value);
            bool dolog;
            if (!string.IsNullOrEmpty(level) && bool.TryParse(level, out dolog))
            {
                this._dolog = dolog;
            }

            LogAdd(TraceLevel.Debug, "Parsed processing instructions '{0}'. Parser mode = {1}, and logging = {2}", value, this.Settings.ConformanceMode, this.DoLog);
        }


        private string GetProcessingValue(string attributename, string full)
        {
            if (string.IsNullOrEmpty(attributename))
                return string.Empty;

            int index = full.IndexOf(attributename + "=");
            
            if (index == 0 || (index > 0 && full[index-1] == ' '))//make sure we are at a boundary
            {
                index += attributename.Length + 1;

                char separator = full[index];
                int start, end;

                if (separator == '\'' || separator == '"')
                {
                    start = index + 1;
                    end = full.IndexOf(separator, start);
                }
                else
                {
                    start = index;
                    end = full.IndexOf(' ', start);
                    if (end < start)
                        end = full.Length; 
                }

                return full.Substring(start, end - start);
            }
            return string.Empty;
        }

        internal object CreateInstance(ParserClassDefinition cdef)
        {
            if (null == cdef)
                throw new ArgumentNullException("cdef");

            return CreateInstance(cdef.ClassType);
        }

        internal object CreateInstance(Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new PDFParserException(String.Format(Errors.CannotCreateInstanceOfType, type, ex.Message), ex);
            }
        }
    }
}
