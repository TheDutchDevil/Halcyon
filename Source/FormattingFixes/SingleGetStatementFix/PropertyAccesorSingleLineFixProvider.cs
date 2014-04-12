using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FormattingFixes.TypeToVar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace FormattingFixes.SingleGetStatementFix
{
    [ExportCodeFixProvider(PropertyAccessorSingleLineAnalyzer.DiagnosticId, LanguageNames.CSharp)]
    class PropertyAccesorSingleLineFixProvider : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] {PropertyAccessorSingleLineAnalyzer.DiagnosticId};
        }

        /// <summary>
        /// <para>
        /// Checks for three conditions, and provides fixes for these three conditions:
        /// </para>
        /// <para>
        ///  There is a line between the accessor keyword and the block; the new line
        /// between the keyword and block is removed 
        /// </para>
        /// <para>
        /// There is anew line between the opening brace of the block and the first statement
        /// within the block; the new line between the opening brace and the first statement
        /// is removed
        /// </para>
        /// <para>
        /// There is a new line before the closing brace of the block and the last statement 
        /// within the block; the new line between the last statement and the closing brace
        /// is removed
        /// </para>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="diagnostics"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var accessorDeclr = root.FindToken(span.Start).Parent as AccessorDeclarationSyntax;

            return new[]
            {
                CodeAction.Create("Make the accessor single line", c =>
                    SingleLinePropertyAccessor(document, accessorDeclr, c, span))
            };
        }

        private async Task<Document> SingleLinePropertyAccessor(Document document,
            AccessorDeclarationSyntax accessorDeclaration, CancellationToken cancellationToken,
            TextSpan span)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var newDocument = document;

            /*
             * First check if the keyword and block are on the same line
             */

            if (accessorDeclaration.Keyword.TrailingTrivia.Any(
                trivia => trivia.RawKind == SyntaxFactory.LineFeed.RawKind))
            {
                var newKeyword =
                    accessorDeclaration.Keyword.WithTrailingTrivia(
                        from trivia in accessorDeclaration.Keyword.TrailingTrivia
                        where trivia.RawKind != SyntaxFactory.LineFeed.RawKind
                        select trivia);

                newDocument = newDocument.WithSyntaxRoot(root.ReplaceToken(accessorDeclaration.Keyword, newKeyword));
                root = await newDocument.GetSyntaxRootAsync(cancellationToken);
                accessorDeclaration = root.FindToken(span.Start).Parent as AccessorDeclarationSyntax;
            }

            /*
             * Then check if the opening brace and first statement are on the same line
             */

            if (accessorDeclaration.Body.OpenBraceToken.TrailingTrivia.Any(
                    trivia => trivia.RawKind == SyntaxFactory.LineFeed.RawKind))
            {
                var newOpenBraceToken = accessorDeclaration.Body.OpenBraceToken.WithTrailingTrivia(
                    from trivia in accessorDeclaration.Body.OpenBraceToken.TrailingTrivia
                    where trivia.RawKind != SyntaxFactory.LineFeed.RawKind
                    select trivia);

                newDocument = newDocument.WithSyntaxRoot(root.ReplaceToken(accessorDeclaration.Body.OpenBraceToken, newOpenBraceToken));
                root = await newDocument.GetSyntaxRootAsync(cancellationToken);
                accessorDeclaration = root.FindToken(span.Start).Parent as AccessorDeclarationSyntax;
            }

            /*
             * Lastly, check if the closing brace token is preceded by a new line
             */

            if (accessorDeclaration.Body.Statements.Last().GetTrailingTrivia().Any(
                trivia => trivia.RawKind == SyntaxFactory.LineFeed.RawKind))
            {
                var statements = accessorDeclaration.Body.Statements;

                var newStatement = statements.Last().WithTrailingTrivia(
                    from trivia in statements.Last().GetTrailingTrivia()
                    where trivia.RawKind != SyntaxFactory.LineFeed.RawKind
                    select trivia);

                newDocument =  newDocument.WithSyntaxRoot(root.ReplaceTrivia(statements.Last().GetTrailingTrivia(), ComputeReplacementTrivia));
                root = await newDocument.GetSyntaxRootAsync(cancellationToken);
                accessorDeclaration = root.FindToken(span.Start).Parent as AccessorDeclarationSyntax;
            }

            var formattedAccessorDeclaration = accessorDeclaration.WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(accessorDeclaration, formattedAccessorDeclaration);

            return newDocument.WithSyntaxRoot(root);
        }

        private SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia syntaxTrivia, SyntaxTrivia trivia)
        {
            return syntaxTrivia.RawKind == SyntaxFactory.LineFeed.RawKind ? SyntaxFactory.Space : syntaxTrivia;
        }
    }
}
