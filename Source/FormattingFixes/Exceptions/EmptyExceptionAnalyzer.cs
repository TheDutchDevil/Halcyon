using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FormattingFixes.Exceptions
{
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    internal class EmptyExceptionAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        private readonly List<Type> _exceptionTypes;


        private const string DiagnosticId = "EmptyException";
        private const string Description = "Verifies whether exceptions of a certain type are thrown with a message";
        private const string MessageFormat = "{0} thrown without a message";
        private const string Category = "Exceptions";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description,
            MessageFormat, Category, DiagnosticSeverity.Warning);

        public EmptyExceptionAnalyzer()
        {
            this._exceptionTypes = new List<Type> {typeof (NotImplementedException), typeof (ArgumentException)};
        }

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get { return ImmutableArray.Create(SyntaxKind.ThrowStatement); }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic,
            CancellationToken cancellationToken)
        {
            if (node is ThrowStatementSyntax)
            {
                var throwStatement = node as ThrowStatementSyntax;

                if (throwStatement.Expression is ObjectCreationExpressionSyntax)
                {
                    var creationSyntax = throwStatement.Expression as ObjectCreationExpressionSyntax;
                    var type = semanticModel.GetTypeInfo(throwStatement.Expression);

                    if (this._exceptionTypes.Any(exType => type.Type.Name == exType.Name)
                        && creationSyntax.ArgumentList.Arguments.Count == 0)
                    {
                        addDiagnostic(Diagnostic.Create(Rule, throwStatement.GetLocation(), type.Type.Name));
                    }
                }
            }
        }
    }
}
