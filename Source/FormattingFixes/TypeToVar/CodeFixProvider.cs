using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;


namespace FormattingFixes.TypeToVar
{
    [ExportCodeFixProvider(DiagnosticAnalyzer.DiagnosticId, LanguageNames.CSharp)]
    internal class CodeFixProvider : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] {DiagnosticAnalyzer.DiagnosticId};
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span,
            IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var varibaleDeclr = (VariableDeclarationSyntax) root.FindToken(span.Start).Parent.Parent;

            return new[]
            {
                CodeAction.Create("Make declaration implicit", c =>
                    MakeStaticDeclarationImplicit(document, varibaleDeclr, c))
            };
        }

        private async Task<Document> MakeStaticDeclarationImplicit(Document document,
            VariableDeclarationSyntax variableDeclaration, CancellationToken
                cancellationToken)
        {
            var newVariableDeclaration = variableDeclaration.WithType(SyntaxFactory.IdentifierName("var"));

            newVariableDeclaration = newVariableDeclaration.WithAdditionalAnnotations(Formatter.Annotation);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(variableDeclaration, newVariableDeclaration);


            return document.WithSyntaxRoot(newRoot);

        }

    }
}