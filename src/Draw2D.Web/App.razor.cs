using Avalonia.Web.Blazor;

namespace Draw2D.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<Draw2D.App>()
            .SetupWithSingleViewLifetime();
    }
}
