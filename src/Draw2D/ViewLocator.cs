using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Draw2D.ViewModels;

namespace Draw2D;

public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public Control Build(object data)
    {
        var name = data.GetType().FullName.Replace("ViewModel", "View");
        var type = Type.GetType(name + "View");

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type);
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}
