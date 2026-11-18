using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OficinaEF.Infrastructure;

// Evita o erro clássico de preço: "129.90" virar 12990 em servidor pt-BR.
// Aceita "129.90" (campo type=number do navegador) e "1.299,90" (digitado à mão).
public class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        Type tipo = context.Metadata.ModelType;
        return tipo == typeof(decimal) || tipo == typeof(decimal?) ? new DecimalModelBinder() : null;
    }
}

public class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ValueProviderResult resultado = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (resultado == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, resultado);

        string? texto = resultado.FirstValue?.Trim();
        if (string.IsNullOrEmpty(texto))
        {
            if (bindingContext.ModelMetadata.IsNullableValueType)
                bindingContext.Result = ModelBindingResult.Success(null);
            else
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Informe um valor.");
            return Task.CompletedTask;
        }

        texto = texto.Replace("R$", "").Replace(" ", "").Replace(" ", "");

        // Se tem vírgula, é formato brasileiro: 1.299,90 -> 1299.90
        if (texto.Contains(','))
            texto = texto.Replace(".", "").Replace(',', '.');

        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor))
            bindingContext.Result = ModelBindingResult.Success(valor);
        else
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Informe um valor numérico válido.");

        return Task.CompletedTask;
    }
}
