using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Linq;

namespace TaskMangementClientSide
{
    public class InputSelectMultiple<TValue> : InputBase<TValue>
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "select");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "multiple", "multiple");
            builder.AddAttribute(3, "class", CssClass);

            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string[]>(
                this,
                values => CurrentValue = ParseValues(values),
                Array.Empty<string>()
            ));

            builder.AddContent(5, ChildContent);
            builder.CloseElement();
        }

        private TValue ParseValues(string[] values)
        {
            if (typeof(TValue) == typeof(List<int>))
            {
                var ints = values.Select(int.Parse).ToList();
                return (TValue)(object)ints;
            }

            throw new InvalidOperationException($"Unsupported type {typeof(TValue)} in InputSelectMultiple.");
        }

        // Required abstract method — not used for multiple select
        protected override bool TryParseValueFromString(
            string? value,
            out TValue result,
            out string? validationErrorMessage)
        {
            result = default!;
            validationErrorMessage = null;
            return true;
        }

        protected override string FormatValueAsString(TValue? value)
        {
            if (value is List<int> list)
                return string.Join(",", list);

            return string.Empty;
        }
    }
}
