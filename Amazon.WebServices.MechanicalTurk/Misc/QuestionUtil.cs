#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion
 

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Amazon.WebServices.MechanicalTurk.Domain;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{
	/// <summary>
	/// Utilities for question handling
	/// </summary>
	public sealed class QuestionUtil
    {
        private static Regex formattedContentSplitter = new Regex("<FormattedContent>", RegexOptions.Compiled);
        private static Regex htmlContentSplitter = new Regex("<HTMLContent>", RegexOptions.Compiled);

        #region Constructors
        static QuestionUtil()
        {
            string xmlNamespace = "http://mechanicalturk.amazonaws.com/AWSMechanicalTurkDataSchemas/2005-10-01/QuestionForm.xsd";
            // read the namespace for the schema from the generated file to dynamically reflect changes
            XmlRootAttribute[] atts = (XmlRootAttribute[])
                (typeof(QuestionForm).GetCustomAttributes(typeof(System.Xml.Serialization.XmlRootAttribute), false));
            if (atts.Length == 1)
            {
                xmlNamespace = atts[0].Namespace;
            }

            TPL_FREE_TEXT_QUESTION_FORM = TPL_FREE_TEXT_QUESTION_FORM.Replace("@NAMESPACE@", xmlNamespace);

            xmlNamespace = "http://mechanicalturk.amazonaws.com/AWSMechanicalTurkDataSchemas/2006-07-14/ExternalQuestion.xsd";
            // same for external question
            atts = (XmlRootAttribute[])
                (typeof(ExternalQuestion).GetCustomAttributes(typeof(System.Xml.Serialization.XmlRootAttribute), false));
            if (atts.Length == 1)
            {
                xmlNamespace = atts[0].Namespace;
            }

            TPL_EXTERNAL_QUESTION_FORM = TPL_EXTERNAL_QUESTION_FORM.Replace("@NAMESPACE@", xmlNamespace);

            xmlNamespace = "http://mechanicalturk.amazonaws.com/AWSMechanicalTurkDataSchemas/2011-11-11/HTMLQuestion.xsd";
            // same for HTML question
            atts = (XmlRootAttribute[])
                (typeof(HTMLQuestion).GetCustomAttributes(typeof(System.Xml.Serialization.XmlRootAttribute), false));
            if (atts.Length == 1)
            {
                xmlNamespace = atts[0].Namespace;
            }

            TPL_HTML_QUESTION_FORM = TPL_HTML_QUESTION_FORM.Replace("@NAMESPACE@", xmlNamespace);
        }

        private QuestionUtil()
        {
        }
        #endregion

        #region Question handling
        private static string TPL_FREE_TEXT_QUESTION_FORM = 
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<QuestionForm xmlns=\"@NAMESPACE@\">" +
            "{0}" +
            "</QuestionForm>";
        private static string TPL_EXTERNAL_QUESTION_FORM =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<ExternalQuestion xmlns=\"@NAMESPACE@\">" +
            "{0}" +
            "</ExternalQuestion>";
        private static string TPL_HTML_QUESTION_FORM =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<HTMLQuestion xmlns=\"@NAMESPACE@\">" +
            "{0}" +
            "</HTMLQuestion>";
        private static string TPL_FREE_TEXT_QUESTION_SINGLE =
            "  <Question>" +
            "    <QuestionIdentifier>{0}</QuestionIdentifier>" +
            "    <QuestionContent>" +
            "      <Text>{1}</Text>" +
            "    </QuestionContent>" +
            "    <AnswerSpecification>" +
            "      <FreeTextAnswer/>" +
            "    </AnswerSpecification>" +
            "  </Question>";

        private static string InjectCDataBlocksForFormattedContents(string question)
        {
            return InjectCDataBlocksForTag(question, "FormattedContent", formattedContentSplitter);
        }

        private static string InjectCDataBlocksForHTMLContents(string question)
        {
            return InjectCDataBlocksForTag(question, "HTMLContent", htmlContentSplitter);
        }

        /// <summary>
        /// Injects CDATA into the formatted contents, since the XML Serializer only
        /// properly handles CDATA when a custom CData serializer can be used
        /// (see http://geekswithblogs.net/cmartin/archive/2005/11/30/61705.aspx).
        /// This is not the case for us, because QuestionForm is generated and the
        /// question must be of type string. Not ideal.
        /// </summary>
        private static string InjectCDataBlocksForTag(string question, string tag, Regex splitter)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int i1 = question.IndexOf(startTag);
            if (i1 != -1)
            {
                // there is at least one formatted content - process them all                
                string[] parts = splitter.Split(question);
                StringBuilder sb = new StringBuilder();
                int i2 = 0;
                for (int i = 0; i < parts.Length; i++)
                {
                    i2 = parts[i].IndexOf(endTag);
                    if (i2 == -1)
                    {
                        sb.Append(parts[i]);
                    }
                    else
                    {
                        // wrap and unescape this block
                        sb.Append(startTag + "<![CDATA[");
                        sb.Append(XmlUtil.XmlDecode(parts[i].Substring(0, i2)));  // cdata block
                        sb.Append("]]>" + endTag);
                        sb.Append(parts[i].Substring(i2 + endTag.Length));
                    }
                }

                question = sb.ToString();
            }

            return question;
        }
        
        /// <summary>
        /// Serializes the question file, containing XML, into an XML string accepted by Mechanical Turk. Validates the XML
        /// and cleans up encoding.
        /// </summary>
        /// <param name="questionFile">A file to serialize, containing a QuestionForm, ExternalQuestion, or HTMLQuestion</param>
        /// <returns>XML string</returns>
        public static string SerializeQuestionFileToString(string questionFile)
        {
            if (questionFile == null)
            {
                throw new ArgumentNullException("questionFile", "Can't serialize null questionFile");
            }
            string xml = File.ReadAllText(questionFile);
            if (xml.Contains("</QuestionForm>"))
            {
                return SerializeQuestionForm(ReadQuestionFormFromFile(questionFile));
            }
            else if (xml.Contains("</ExternalQuestion>"))
            {
                return SerializeExternalQuestion(ReadExternalQuestionFromFile(questionFile));
            }
            else if (xml.Contains("</HTMLQuestion>"))
            {
                return SerializeHTMLQuestion(ReadHTMLQuestionFromFile(questionFile));
            }
            else
            {
                throw new ArgumentNullException("questionFile", "Can't find supported question type");
            }
        }

        /// <summary>
        /// Serializes the question form into XML accepted by Mechanical Turk
        /// </summary>
        /// <param name="form">A <see cref="QuestionForm"/> instance to serialize</param>
        /// <returns>XML string</returns>
        public static string SerializeQuestionForm(QuestionForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form", "Can't serialize null form");
            }

            string s1 = XmlUtil.SerializeXML(form);

            int i1 = s1.IndexOf("<Overview>");
            if (i1 == -1)
            {
                i1 = s1.IndexOf("<Question>");
            }

            if (i1 == -1)
            {
                throw new ArgumentException("Cannot serialize question form (contains no questions)");
            }

            s1 = InjectCDataBlocksForFormattedContents(s1);
            s1= string.Format(TPL_FREE_TEXT_QUESTION_FORM, 
                s1.Substring(i1, s1.IndexOf("</QuestionForm>") - i1));

            return s1;
        }

        /// <summary>
        /// Deserializes XML into a question form
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>A <see cref="QuestionForm"/> instance</returns>
        public static QuestionForm DeserializeQuestionForm(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Can't deserialize empty or null XML to question form", "xml");
            }

            return (QuestionForm)XmlUtil.DeserializeXML(typeof(QuestionForm), xml);
        }

        /// <summary>
        /// Reads a question form from an XML file
        /// </summary>
        /// <param name="file">File containing the question in XML format</param>
        /// <returns>A <see cref="QuestionForm"/> instance</returns>
        public static QuestionForm ReadQuestionFormFromFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Can't deserialize empty or null XML file to question form", "file");
            }

            string xml = File.ReadAllText(file);

            return DeserializeQuestionForm(xml);
        }

        /// <summary>
        /// Serializes the external question into XML accepted by Mechanical Turk
        /// </summary>
        /// <param name="form">A <see cref="ExternalQuestion"/> instance to serialize</param>
        /// <returns>XML string</returns>
        public static string SerializeExternalQuestion(ExternalQuestion form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form", "Can't serialize null form");
            }

            string s = XmlUtil.SerializeXML(form);

            // fast transform of xml serializer output
            int index1 = s.IndexOf("<ExternalQuestion") + 1;
            index1 = s.IndexOf('>', index1);
            s = s.Substring(index1+1).Replace("</ExternalQuestion>", string.Empty);

            return string.Format(TPL_EXTERNAL_QUESTION_FORM, s);
        }

        /// <summary>
        /// Deserializes XML into a external question
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>A <see cref="ExternalQuestion"/> instance</returns>
        public static ExternalQuestion DeserializeExternalQuestion(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Can't deserialize empty or null XML to external question", "xml");
            }

            return (ExternalQuestion)XmlUtil.DeserializeXML(typeof(ExternalQuestion), xml);
        }

        /// <summary>
        /// Reads an external question from an XML file
        /// </summary>
        /// <param name="file">File containing the question in XML format</param>
        /// <returns>A <see cref="ExternalQuestion"/> instance</returns>
        public static ExternalQuestion ReadExternalQuestionFromFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Can't deserialize empty or null XML file to external question", "file");
            }

            string xml = File.ReadAllText(file);

            return DeserializeExternalQuestion(xml);
        }

        /// <summary>
        /// Serializes the HTML question into XML accepted by Mechanical Turk
        /// </summary>
        /// <param name="question">A <see cref="HTMLQuestion"/> instance to serialize</param>
        /// <returns>XML string</returns>
        public static string SerializeHTMLQuestion(HTMLQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("form", "Can't serialize null form");
            }

            string s = XmlUtil.SerializeXML(question);

            s = InjectCDataBlocksForHTMLContents(s);

            // fast transform of xml serializer output
            int index1 = s.IndexOf("<HTMLQuestion") + 1;
            index1 = s.IndexOf('>', index1);
            s = s.Substring(index1 + 1).Replace("</HTMLQuestion>", string.Empty);

            return string.Format(TPL_HTML_QUESTION_FORM, s);
        }

        /// <summary>
        /// Deserializes XML into an HTML question
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>A <see cref="HTMLQuestion"/> instance</returns>
        public static HTMLQuestion DeserializeHTMLQuestion(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Can't deserialize empty or null XML to external question", "xml");
            }

            return (HTMLQuestion)XmlUtil.DeserializeXML(typeof(HTMLQuestion), xml);
        }

        /// <summary>
        /// Reads an HTML question from an XML file
        /// </summary>
        /// <param name="file">File containing the question in XML format</param>
        /// <returns>A <see cref="HTMLQuestion"/> instance</returns>
        public static HTMLQuestion ReadHTMLQuestionFromFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Can't deserialize empty or null XML file to HTMLQuestion", "file");
            }

            string xml = File.ReadAllText(file);

            return DeserializeHTMLQuestion(xml);
        }

        /// <summary>
		/// Constructs a Question XML String that contains a single freetext question.
		/// </summary>
		/// <param name="question">The question phrase to ask</param>
		/// <returns>Question in XML format</returns>
		public static string ConvertSingleFreeTextQuestionToXML(string question)
		{
            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException("Can't convert empty or null question to question form", "question");
            }

            return ConvertMultipleFreeTextQuestionToXML(new string[] { question });
		}

        /// <summary>
        /// Constructs a Question XML String that contains a multiple freetext questions
        /// </summary>
        /// <param name="questions">The question phrase to ask</param>
        /// <returns>Question in XML format</returns>
        public static string ConvertMultipleFreeTextQuestionToXML(string[] questions)
        {
            if (questions==null || questions.Length==0) 
            {
                throw new ArgumentNullException("questions", "Empty or null question cannot be converted to XML");
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < questions.Length; i++)
            {
                if (questions[i] != null && questions[i].Length > 0)
                {
                    sb.Append(string.Format(TPL_FREE_TEXT_QUESTION_SINGLE, i + 1, XmlUtil.XmlEncode(questions[i])));
                }
                else
                {
                    MTurkLog.Warn("Ignoring empty question at position {0}", i);
                }
            }

            return string.Format(TPL_FREE_TEXT_QUESTION_FORM, sb.ToString());
        }

        /// <summary>
        /// Deserializes XML into a question form answer
        /// </summary>
        /// <param name="xml">XML string representing answers to a HIT question</param>
        /// <returns>A <see cref="QuestionFormAnswers"/> instance</returns>
        public static QuestionFormAnswers DeserializeQuestionFormAnswers(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Can't deserialize empty or null XML to question form answers", "xml");
            }

            return (QuestionFormAnswers)XmlUtil.DeserializeXML(typeof(QuestionFormAnswers), xml);
        }
		#endregion
	}
}
