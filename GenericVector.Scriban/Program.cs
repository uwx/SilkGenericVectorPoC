using System.Runtime.CompilerServices;

// using System.Text.Encodings.Web;
// using System.Web;
// using GenericVector.Scriban;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Web;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
//
// IServiceCollection services = new ServiceCollection();
// services.AddLogging();
// services.AddSingleton<HtmlEncoder, MyHtmlEncoder>()
//
// IServiceProvider serviceProvider = services.BuildServiceProvider();
// ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
//
// await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
//
// var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
// {
//     var dictionary = new Dictionary<string, object?>
//     {
//         { "Message", "Hello from the Render Message component!" }
//     };
//
//     var parameters = ParameterView.FromDictionary(dictionary);
//     var output = await htmlRenderer.RenderComponentAsync<RenderMessage>(parameters);
//
//     return output.ToHtmlString();
// });
//
// Console.WriteLine(html);
//
// public class MyHtmlEncoder : HtmlEncoder
// {
//     public override int FindFirstCharacterToEncode(char* text, int textLength)
//     {
//         return -1;
//     }
//
//     public override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
//     {
//         numberOfCharactersWritten = 0;
//         return false;
//     }
//
//     public override bool WillEncode(int unicodeScalar)
//     {
//         return false;
//     }
//
//     public override int MaxOutputCharactersPerInputCharacter => 0;
// }
//

// See https://aka.ms/new-console-template for more information

using Humanizer;
using Scriban;
using Scriban.Functions;
using Scriban.Parsing;
using Scriban.Runtime;
using Scriban.Syntax;

{
    unsafe
    {
        string s = "fooobar";

        // [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_firstChar")]
        // static extern ref char GetFirstChar(string s);
        // ref char _firstChar = ref GetFirstChar(s);

        var span = s.AsSpan();
        ref readonly char c = ref span.GetPinnableReference();
        ref char _firstChar = ref Unsafe.AsRef<char>(in c);

        var s1 = Unsafe.AsPointer(ref Unsafe.SubtractByteOffset(ref _firstChar, 
            + sizeof(nint) // void* methodTable
            + sizeof(int) // DWORD m_StringLength
        ));
        
        string a = (string)s1;

        var a = *(string*)(&s1);
        Console.WriteLine(a);
    }
}

const int vectorMin = 2;
const int vectorMax = 5;
const int matrixRowMin = 1;
const int matrixRowMax = 5;
const int matrixColMin = 1;
const int matrixColMax = 4;

{
    var template = Template.Parse(File.ReadAllText("../../../../GenericVector/GenericVectors.scriban-cs"));
    for (var i = vectorMin; i <= vectorMax; i++)
    {
        var obj = new ScriptObject();
        obj.Import(new BuiltinFunctions());
        obj.Import(
            new
            {
                VecN = i,
                MinDimensions = vectorMin,
                MaxDimensions = vectorMax,
                LoadFile = new LoadFileFunction()
            },
            renamer: StandardMemberRenamer.Default
        );
        obj.Import(typeof(ScribanHelpers));
        var result = template.Render(
            new TemplateContext(obj)
            {
                TemplateLoader = new MyIncludeFromDisk()
            }
        );

        // Console.WriteLine(result);
        File.WriteAllText($"../../../../GenericVector/Vector{i}.gen.cs", result!);
    }
}
{
    var template = Template.Parse(File.ReadAllText("../../../../GenericVector/SpeedHelpers2.scriban-cs"));
    var result = template.Render();

    File.WriteAllText($"../../../../GenericVector/SpeedHelpers2.gen.cs", result!);
}

public static class ScribanHelpers
{
    public static int Max(int x, int y) => Math.Max(x, y);
    public static int Min(int x, int y) => Math.Min(x, y);
    public static string HumanizeAnd(object[] items) => items.Humanize();

    // public static string Slice(object? value, int start = 0, int? end = null)
    // {
    //     var st = start < 0 ? ^-start : start;
    //     var en = end != null ? end < 0 ? ^-end.Value : end.Value : ^0;
    //
    //     return value?.ToString()?[st..en] ?? "<null>";
    // }
}

public sealed partial class LoadFileFunction : IScriptCustomFunction
{
    public LoadFileFunction()
    {
    }

    public object Invoke(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
    {
        if (arguments.Count == 0)
        {
            throw new ScriptRuntimeException(callerContext.Span, "Expecting at least the name of the template to include for the <include> function");
        }

        var templateName = context.ObjectToString(arguments[0]);

        // If template name is empty, throw an exception
        if (string.IsNullOrEmpty(templateName))
        {
            // In a liquid template context, we let an include to continue without failing
            if (context is LiquidTemplateContext)
            {
                return null;
            }
            throw new ScriptRuntimeException(callerContext.Span, $"Include template name cannot be null or empty");
        }

        var templateLoader = context.TemplateLoader;
        if (templateLoader == null)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Unable to include <{templateName}>. No TemplateLoader registered in TemplateContext.TemplateLoader");
        }

        string templatePath;

        try
        {
            templatePath = templateLoader.GetPath(context, callerContext.Span, templateName);
        }
        catch (Exception ex) when (ex is not ScriptRuntimeException)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Unexpected exception while getting the path for the include name `{templateName}`", ex);
        }
        // If template path is empty (probably because template doesn't exist), throw an exception
        if (templatePath == null)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Include template path is null for `{templateName}");
        }

        if (!context.CachedTemplates.TryGetValue(templatePath, out var template))
        {

            string templateText;
            try
            {
                templateText = templateLoader.Load(context, callerContext.Span, templatePath);
            }
            catch (Exception ex) when (ex is not ScriptRuntimeException)
            {
                throw new ScriptRuntimeException(callerContext.Span, $"Unexpected exception while loading the include `{templateName}` from path `{templatePath}`", ex);
            }

            if (templateText == null)
            {
                throw new ScriptRuntimeException(callerContext.Span, $"The result of including `{templateName}->{templatePath}` cannot be null");
            }

            // Clone parser options
            var parserOptions = context.TemplateLoaderParserOptions;
            var lexerOptions = context.TemplateLoaderLexerOptions;
            template = Template.Parse(templateText, templatePath, parserOptions, lexerOptions);

            // If the template has any errors, throw an exception
            if (template.HasErrors)
            {
                throw new ScriptParserRuntimeException(callerContext.Span, $"Error while parsing template `{templateName}` from `{templatePath}`", template.Messages);
            }

            context.CachedTemplates.Add(templatePath, template);
        }

        // Make sure that we cannot recursively include a template
        object result;
        context.EnterRecursive(callerContext);
        var previousIndent = context.CurrentIndent;
        context.CurrentIndent = null;
        context.PushOutput();
        var previousArguments = context.GetValue(ScriptVariable.Arguments);
        try
        {
            context.SetValue(ScriptVariable.Arguments, arguments, true, true);
            // if (previousIndent != null)
            // {
            //     // We reset before and after the fact that we have a new line
            //     context.ResetPreviousNewLine();
            // }
            result = template.Evaluate(context);
            // if (previousIndent != null)
            // {
            //     context.ResetPreviousNewLine();
            // }
        }
        finally
        {
            context.PopOutput();
            context.CurrentIndent = previousIndent;
            context.ExitRecursive(callerContext);

            // Remove the arguments
            context.DeleteValue(ScriptVariable.Arguments);
            if (previousArguments != null)
            {
                // Restore them if necessary
                context.SetValue(ScriptVariable.Arguments, previousArguments, true);
            }
        }
        return result;
    }

    public async ValueTask<object> InvokeAsync(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
    {
        if (arguments.Count == 0)
        {
            throw new ScriptRuntimeException(callerContext.Span, "Expecting at least the name of the template to include for the <include> function");
        }

        var templateName = context.ObjectToString(arguments[0]);

        // If template name is empty, throw an exception
        if (string.IsNullOrEmpty(templateName))
        {
            // In a liquid template context, we let an include to continue without failing
            if (context is LiquidTemplateContext)
            {
                return null;
            }
            throw new ScriptRuntimeException(callerContext.Span, $"Include template name cannot be null or empty");
        }

        var templateLoader = context.TemplateLoader;
        if (templateLoader == null)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Unable to include <{templateName}>. No TemplateLoader registered in TemplateContext.TemplateLoader");
        }

        string templatePath;

        try
        {
            templatePath = templateLoader.GetPath(context, callerContext.Span, templateName);
        }
        catch (Exception ex) when (ex is not ScriptRuntimeException)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Unexpected exception while getting the path for the include name `{templateName}`", ex);
        }
        // If template path is empty (probably because template doesn't exist), throw an exception
        if (templatePath == null)
        {
            throw new ScriptRuntimeException(callerContext.Span, $"Include template path is null for `{templateName}");
        }

        if (!context.CachedTemplates.TryGetValue(templatePath, out var template))
        {

            string templateText;
            try
            {
                templateText = await templateLoader.LoadAsync(context, callerContext.Span, templatePath);
            }
            catch (Exception ex) when (ex is not ScriptRuntimeException)
            {
                throw new ScriptRuntimeException(callerContext.Span, $"Unexpected exception while loading the include `{templateName}` from path `{templatePath}`", ex);
            }

            if (templateText == null)
            {
                throw new ScriptRuntimeException(callerContext.Span, $"The result of including `{templateName}->{templatePath}` cannot be null");
            }

            // Clone parser options
            var parserOptions = context.TemplateLoaderParserOptions;
            var lexerOptions = context.TemplateLoaderLexerOptions;
            template = Template.Parse(templateText, templatePath, parserOptions, lexerOptions);

            // If the template has any errors, throw an exception
            if (template.HasErrors)
            {
                throw new ScriptParserRuntimeException(callerContext.Span, $"Error while parsing template `{templateName}` from `{templatePath}`", template.Messages);
            }

            context.CachedTemplates.Add(templatePath, template);
        }

        // Make sure that we cannot recursively include a template
        object result;
        context.EnterRecursive(callerContext);
        var previousIndent = context.CurrentIndent;
        context.CurrentIndent = null;
        context.PushOutput();
        var previousArguments = await context.GetValueAsync(ScriptVariable.Arguments);
        try
        {
            context.SetValue(ScriptVariable.Arguments, arguments, true, true);
            // if (previousIndent != null)
            // {
            //     // We reset before and after the fact that we have a new line
            //     context.ResetPreviousNewLine();
            // }
            result = await template.EvaluateAsync(context);
            // if (previousIndent != null)
            // {
            //     context.ResetPreviousNewLine();
            // }
        }
        finally
        {
            context.PopOutput();
            context.CurrentIndent = previousIndent;
            context.ExitRecursive(callerContext);

            // Remove the arguments
            context.DeleteValue(ScriptVariable.Arguments);
            if (previousArguments != null)
            {
                // Restore them if necessary
                await context.SetValueAsync(ScriptVariable.Arguments, previousArguments, true);
            }
        }
        return result;
    }

    public int RequiredParameterCount => 1;

    public int ParameterCount => 1;

    public ScriptVarParamKind VarParamKind => ScriptVarParamKind.Direct;

    public Type ReturnType => typeof(object);

    public ScriptParameterInfo GetParameterInfo(int index) => index switch
    {
        0 => new ScriptParameterInfo(typeof(string), "template_name"),
        _ => new ScriptParameterInfo(typeof(object), "value")
    };
}

// public class ForEachDimension : IScriptCustomFunction
// {
//     public ScriptParameterInfo GetParameterInfo(int index)
//     {
//         throw new NotImplementedException();
//     }
//
//     public int RequiredParameterCount => throw new NotImplementedException();
//
//     public int ParameterCount => throw new NotImplementedException();
//
//     public ScriptVarParamKind VarParamKind => throw new NotImplementedException();
//
//     public Type ReturnType => throw new NotImplementedException();
//
//     public object Invoke(
//         TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement
//     )
//     {
//         context.Import(default, new
//         {
//             
//         });
//         blockStatement.cont
//         context.SetValue();
//         blockStatement.Evaluate(context);
//     }
//
//     public ValueTask<object> InvokeAsync(
//         TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement
//     )
//     {
//         throw new NotImplementedException();
//     }
// }

/// <summary>
/// A very simple ITemplateLoader loading directly from the disk, without any checks...etc.
/// </summary>
public class MyIncludeFromDisk : ITemplateLoader
{
    public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        return ValueTask.FromResult(Load(context, callerSpan, templatePath));
    }

    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        return Path.Combine("../../../../GenericVector/", templateName);
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        // Template path was produced by the `GetPath` method above in case the Template has 
        // not been loaded yet
        return File.ReadAllText(templatePath);
    }
}

//var template2 = Template.Parse(File.ReadAllText("../../../../GenericVector/GenericMatrix.scriban-cs"));
//for (var x = matrixRowMin; x <= matrixRowMax; x++)
//for (var y = matrixColMin; y <= matrixColMax; y++)
//{
//    var result = template2.Render(new { Rows = x, Columns = y, MinRows = matrixRowMin, MinColumns = matrixColMin, MaxRows = matrixRowMax, MaxColumns = matrixColMax });

//    File.WriteAllText($"../../../../GenericVector/Matrix{x}X{y}.gen.cs", result!);
//}