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
        internal const string DiagnosticId = "MultilineAccessor";
        private const string Description = "Checks the accessor of properties, throws a warning whenever one can be placed one one line";
        private const string MessageFormat = "{0} accessor of {1} can be placed on one line";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning);


        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get { return ImmutableArray.Create(SyntaxKind.MethodDeclaration); }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            if (node is MethodDeclarationSyntax)
            {
                var methodDeclr = node as MethodDeclarationSyntax;

                var methodSymbl = semanticModel.GetDeclaredSymbol(methodDeclr);

                if (methodSymbl.ReturnType.AllInterfaces.Any(type => type.Name == "IEnumerable"))
                {
                    
                }
            }
        }
    }
}
