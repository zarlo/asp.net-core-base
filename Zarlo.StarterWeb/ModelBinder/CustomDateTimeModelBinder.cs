using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Zarlo.StarterWeb.ModelBinder;


public class CustomDateTimeModelBinderProvider
    : IModelBinderProvider
{
    public IModelBinder GetBinder(
        ModelBinderProviderContext context)
    {
        if (CustomDateTimeModelBinder
            .SupportedDatetimeTypes
            .Contains(context.Metadata.ModelType)
            )
        {
            return new BinderTypeModelBinder(
                typeof(CustomDateTimeModelBinder));
        }
        return null;
    }
}

public class CustomDateTimeModelBinder : IModelBinder
{
    public static readonly Type[] SupportedDatetimeTypes =
        new Type[] { typeof(DateTime), typeof(DateTime?) };
    public Task BindModelAsync(ModelBindingContext modelBindingContext)
    {
        if (modelBindingContext == null)
        {
            throw new ArgumentNullException
                (nameof(modelBindingContext));
        }
        if (!SupportedDatetimeTypes
            .Contains(modelBindingContext.ModelType)
            )
        {
            return Task.CompletedTask;
        }
        var modelName = modelBindingContext.ModelName;
        var valueProviderResult = modelBindingContext
            .ValueProvider
            .GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }
        modelBindingContext
            .ModelState
            .SetModelValue(modelName, valueProviderResult);
        var dateTimeToParse
            = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(dateTimeToParse))
        {
            return Task.CompletedTask;
        }
        var formattedDateTime
            = ParseDateTime(dateTimeToParse);
        modelBindingContext.Result
            = ModelBindingResult.Success(formattedDateTime);
        return Task.CompletedTask;
    }
    static DateTime? ParseDateTime(string date)
    {
        var customDatetimeFormats = new string[]
        {
        "ddMMyyyy",
        "dd-MM-yyyy-THH-mm-ss",
        "dd-MM-yyyy-HH-mm-ss",
        "dd-MM-yyyy-HH-mm",
        };
        foreach (var format in customDatetimeFormats)
        {
            if (DateTime.TryParseExact(
                date, format, null,
                DateTimeStyles.None,
                out DateTime validDate)
                )
            {
                return validDate;
            }
        }
        return null;
    }
}
