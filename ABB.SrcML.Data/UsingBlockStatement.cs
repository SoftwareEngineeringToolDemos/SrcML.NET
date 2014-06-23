﻿/******************************************************************************
 * Copyright (c) 2014 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - initial API, implementation, & documentation
 *    Vinay Augustine (ABB Group) - initial API, implementation, & documentation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ABB.SrcML.Data
{
    /// <summary>
    /// Represents a using block statement in C#.
    /// These are of the form: 
    /// <code> using(Foo f = new Foo()) { ... } </code>
    /// Note that this is different from a using directive, e.g. <code>using System.Text;</code>
    /// </summary>
    public class UsingBlockStatement : BlockStatement {
        private Expression initExpression;

        /// <summary> The XML name for UsingBlockStatement </summary>
        public new const string XmlName = "UsingBlock";

        /// <summary> XML Name for <see cref="Initializer" /> </summary>
        public const string XmlInitializerName = "Initializer";

        /// <summary>
        /// Instance method for getting <see cref="UsingBlockStatement.XmlName"/>
        /// </summary>
        /// <returns>Returns the XML name for UsingBlockStatement</returns>
        public override string GetXmlName() { return UsingBlockStatement.XmlName; }

        /// <summary> The intialization expression for the using block. </summary>
        public Expression Initializer {
            get { return initExpression; }
            set {
                initExpression = value;
                initExpression.ParentStatement = this;
            }
        }

        protected override void ReadXmlChild(XmlReader reader) {
            if(XmlInitializerName == reader.Name) {
                Initializer = XmlSerialization.ReadChildExpression(reader);
            } else {
                base.ReadXmlChild(reader);
            }
        }

        protected override void WriteXmlContents(XmlWriter writer) {
            if(null != Initializer) {
                XmlSerialization.WriteElement(writer, Initializer, XmlInitializerName);
            }
            base.WriteXmlContents(writer);
        }
    }
}
