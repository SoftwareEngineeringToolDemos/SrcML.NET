﻿/******************************************************************************
 * Copyright (c) 2013 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Vinay Augustine (ABB Group) - initial API, implementation, & documentation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml.Linq;

namespace ABB.SrcML.DataIU.Test {
    [TestFixture]
    public class TypeInventoryTests {
        [Test]
        public void MyTestMethod() {
            var sourceFolder = new FileSystemSourceFolder("path to folder");
            var archive = new SrcMLArchive(sourceFolder);

            // this should generate a data archive for the given srcML archive
            // when we start serializing data, we should consider how the location of the data can be stored in the srcmL archive
            // calling "DataArchive" on a srcML archive that already has data should transparently load the existing data
            DataArchive data = new DataArchive(archive);

            // testDeclaration is some declaration within archive
            var testDeclaration = new XElement(SRC.Declaration, "test data");

            // these are two different ways of passing data into the data archive
            // The first way is a wrapper for the second one
            // The "TypeUse" object takes the XElement and extracts useful information required to identify the type:
            // * name of the type
            // * relevant namespaces / import statements
            // * current class name (which we should later get a type for)
            // The extracted information should be textual information that can be extracted from the XML
            // -- the TypeUse should not generate any information required for the data.
            TypeDefinition typeInfo = data.ResolveType(testDeclaration);

            // TypeUse typeUseForDeclaration = new TypeUse(testDeclaration);
            // TypeDefinition typeInfo = data.ResolveType(typeUseForDeclaration);

            // one of the thing we should be able to do is get the original XML. Behind the scenes, this relies on using XPath queries
            // generated by Extensions.GetXPath()
            XElement typeXml = typeInfo.GetXElement();
            TypeDefinition parentType = typeInfo.Parent;
            XElement parentXml = parentType.GetXElement();
        }

    }
}
