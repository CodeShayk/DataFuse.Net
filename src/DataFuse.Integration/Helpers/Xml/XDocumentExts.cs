using System.Xml.Linq;

namespace DataFuse.Integration.Helpers.Xml;

public static class XDocumentExts
{
    public static void RemoveNamespaces(this XDocument document)
    {
        if (document.Root is null)
            return;
        foreach (var xElement in document.Root.DescendantsAndSelf())
        {
            if (xElement.Name.Namespace != XNamespace.None)
                xElement.Name = XNamespace.None.GetName(xElement.Name.LocalName);
            if (xElement.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
                xElement.ReplaceAttributes(xElement.Attributes().Select(a =>
                {
                    if (a.IsNamespaceDeclaration)
                        return null;
                    return !(a.Name.Namespace != XNamespace.None) ? a : new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value);
                }));
        }
    }
}
