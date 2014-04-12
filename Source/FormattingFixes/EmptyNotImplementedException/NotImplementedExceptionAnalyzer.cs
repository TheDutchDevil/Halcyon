using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FormattingFixes.EmptyNotImplementedException
{
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    class NotImplementedExceptionAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        private const string DiagnosticId = "EmptyNotImplementedException";
        private const string Description = "Verifies whether a NotImplementedException is thrown with a message";
        private const string MessageFormat = "NotImplementedException thrown without a message";
        private const string Category = "Exceptions";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return ImmutableArray.Create(SyntaxKind.ThrowStatement);
            }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic,
            CancellationToken cancellationToken)
        {
            var throwStatement = node as ThrowStatementSyntax;

            if (throwStatement.Expression is ObjectCreationExpressionSyntax
                && (throwStatement.Expression as ObjectCreationExpressionSyntax).ArgumentList.Arguments.Count == 0)
            {
                addDiagnostic(Diagnostic.Create(Rule, throwStatement.GetLocation()));
            }
        }
    }
}
