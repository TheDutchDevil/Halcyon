using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FormattingFixes.TypeToVar
{
         [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    public class TypeToVarAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string DiagnosticId = "TypeToVar";
        private const string Description = "A static type declaration is used";
        private const string MessageFormat = "Local variable {0} is declared without the var keyword";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning);

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get { return ImmutableArray.Create(SyntaxKind.VariableDeclaration); }
        }

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            if (node is VariableDeclarationSyntax)
            {
                var variableDeclr = node as VariableDeclarationSyntax;

                if (variableDeclr.Parent is UsingStatementSyntax ||
                    variableDeclr.Parent is ForEachStatementSyntax ||
                    variableDeclr.Parent is LocalDeclarationStatementSyntax)
                {
                    /*
                     * Variable declaration is in a using statement, local declaration statement or
                     * for each statement, making it eligible for the refactoring
                     */

                AnalyzeDeclaration(addDiagnostic, variableDeclr);
                }
            }
        }

        private static void AnalyzeDeclaration(Action<Diagnostic> addDiagnostic, VariableDeclarationSyntax variableDeclaration)
        {
            var variableInitializerDoesNotAssignNull =
                                variableDeclaration.Variables.Any(
                                    declr =>
                                        declr.Initializer != null &&
                                        declr.Initializer.Value.CSharpKind() != SyntaxKind.NullLiteralExpression);

            var variableIdentifierIsNotADelegate = !(variableDeclaration.Type is IdentifierNameSyntax);

            if (!variableDeclaration.Type.IsVar &&
                variableDeclaration.DescendantNodes().Any(cNode => cNode is EqualsValueClauseSyntax) &&
                variableInitializerDoesNotAssignNull &&
                variableIdentifierIsNotADelegate)
            {
                addDiagnostic(Diagnostic.Create(Rule, variableDeclaration.Type.GetLocation(),
                    variableDeclaration.Variables.First().Identifier.Value));
            }
        }

        private String fieldVar = "";
    }
}
