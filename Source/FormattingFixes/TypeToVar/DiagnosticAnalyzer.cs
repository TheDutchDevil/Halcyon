using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FormattingFixes.TypeToVar
{
    // TODO: Consider implementing other interfaces that implement IDiagnosticAnalyzer instead of or in addition to ISymbolAnalyzer

    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    public class DiagnosticAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string DiagnosticId = "TypeToVar";
        private const string Description = "A static type is used";
        private const string MessageFormat = "Declaration requires the var keyword";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning);

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return ImmutableArray.Create(SyntaxKind.LocalDeclarationStatement);
            }
        }

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
            }
        }

        private void NodeThingy()
        {
            var x = 0;
            int y = 0;

            int x2;
            //var x3;
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax) node;

            var variableDeclaration = localDeclaration.Declaration;

            if (!variableDeclaration.Type.IsVar &&
                variableDeclaration.ChildNodes().Any(nodeDeclr => nodeDeclr.ChildNodes().Any(childNode => childNode is EqualsValueClauseSyntax)))
            {
                addDiagnostic(Diagnostic.Create(Rule, variableDeclaration.Type.GetLocation()));
            }
        }
    }
}
