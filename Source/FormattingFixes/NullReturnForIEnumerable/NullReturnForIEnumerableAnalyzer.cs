using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FormattingFixes.NullReturnForIEnumerable
{

    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    class NullReturnForIEnumerableAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string DiagnosticId = "NullReturnForIEnumerable";
        private const string Description = "Verifies the return expression of methods that return IEnumerable and verifies that they do not retunr null";
        private const string MessageFormat = "{1} which returns an implementation of IEnumerable returns a null value. Fix this by returning an empty {0} implementation.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning);


        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return ImmutableArray.Create(SyntaxKind.MethodDeclaration);
            }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            if (node is MethodDeclarationSyntax)
            {
                var methodDeclr = node as MethodDeclarationSyntax;

                var methodSymbl = semanticModel.GetDeclaredSymbol(methodDeclr);

                if (methodSymbl.ReturnType.AllInterfaces.Any(type => type.Name == "IEnumerable"))
                {
                    foreach (var returnStatement in from returnStatement in methodDeclr.DescendantNodes()
                        where returnStatement is ReturnStatementSyntax
                        select returnStatement as ReturnStatementSyntax)
                    {
                        if (returnStatement.Expression is LiteralExpressionSyntax &&
                            (returnStatement.Expression as LiteralExpressionSyntax).CSharpKind() == SyntaxKind.NullLiteralExpression)
                        {
                            addDiagnostic(Diagnostic.Create(Rule, returnStatement.GetLocation(),
                                methodDeclr.ReturnType.ToString(), methodDeclr.Identifier.Value));
                        }
                    }


                }
            }
        }
    }
}
