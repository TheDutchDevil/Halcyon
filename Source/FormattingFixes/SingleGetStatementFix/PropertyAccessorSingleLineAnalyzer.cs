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

namespace FormattingFixes.SingleGetStatementFix
{
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    class PropertyAccessorSingleLineAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string DiagnosticId = "MultilineAccessor";
        private const string Description = "Checks the accessor of properties, throws a warning whenever one can be placed one one line";
        private const string MessageFormat = "{0} accessor of {1} can be placed on one line";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Info);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return ImmutableArray.Create(SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
            }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            if (node is AccessorDeclarationSyntax)
            {
                var accessorDeclr = node as AccessorDeclarationSyntax;

                if (accessorDeclr.Body == null)
                {
                    return;
                }

                var block = accessorDeclr.Body;

                /*
                 * If no new lines are found at the start and end of the block, and there are no new lines
                 * between the block and accessor (get/set) the block is correctly formatted, if not, a diagnostic
                 * is added at the location of the accessor.
                 */

                var lineTokenKind = SyntaxFactory.LineFeed.RawKind;

                var openingNotNewLined =
                    block.OpenBraceToken.TrailingTrivia.All(trivia => trivia.RawKind != lineTokenKind);

                var closingNotNewLined =
                    block.Statements.Last().GetTrailingTrivia().All(trivia => trivia.RawKind != lineTokenKind);

                var accessorOnSameLineAsBlock =
                    accessorDeclr.Keyword.TrailingTrivia.All(trivia => trivia.RawKind != lineTokenKind);

                if (block.Statements.Count == 1 &&
                    !(openingNotNewLined &&
                    closingNotNewLined &&
                    accessorOnSameLineAsBlock))
                {
                    addDiagnostic(Diagnostic.Create(Rule, 
                        accessorDeclr.Keyword.GetLocation(),
                       char.ToUpper(accessorDeclr.Keyword.Text[0]) + accessorDeclr.Keyword.Text.Substring(1), 
                        (accessorDeclr.Parent.Parent as PropertyDeclarationSyntax).Identifier));
                }
            }
        }
    }
}
