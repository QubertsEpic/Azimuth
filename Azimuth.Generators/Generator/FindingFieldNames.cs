using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.Generators
{
    public class FindingFieldNames
    {
        public const string PositionStructName = "PositionStruct";

        public static IEnumerable<(string type, string name)> FindFieldNames(GeneratorExecutionContext context, string structName)
        {
            var libraryContext = context;
            foreach (var tree in libraryContext.Compilation.SyntaxTrees)
            {
                var semanticModel = libraryContext.Compilation.GetSemanticModel(tree);

                foreach (var str in tree.GetRoot().DescendantNodesAndSelf().OfType<StructDeclarationSyntax>())
                {
                    if (str.Identifier.ValueText == structName)
                    {
                        foreach (var field in str.Members.OfType<FieldDeclarationSyntax>())
                        {
                            if (field.Declaration.Type is PredefinedTypeSyntax predefinedType)
                            {
                                yield return (predefinedType.Keyword.ValueText, field.Declaration.Variables[0].Identifier.ValueText);
                            }
                        }
                    }
                }
            }
        }
    }
}
