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
using Microsoft.CodeAnalysis.Text;

namespace FormattingFixes.SingleGetStatementFix
{
    [ExportCodeFixProvider(DiagnosticAnalyzer.DiagnosticId, LanguageNames.CSharp)]
    class PropertyAccesorSingleLineFixer : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
