using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public abstract class BaseTool : ViewModelBase
{
    private IList<IPointFilter> _filters;
    private IPointFilter _currentFilter;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<IPointFilter> Filters
    {
        get => _filters;
        set => Update(ref _filters, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IPointFilter CurrentFilter
    {
        get => _currentFilter;
        set => Update(ref _currentFilter, value);
    }

    internal void FiltersProcess(IToolContext context, ref double x, ref double y)
    {
        if (_filters != null)
        {
            foreach (var filter in _filters)
            {
                if (filter.Process(context, ref x, ref y))
                {
                    return;
                }
            }
        }
    }

    internal void FiltersClear(IToolContext context)
    {
        if (_filters != null)
        {
            foreach (var filter in _filters)
            {
                filter.Clear(context);
            }
        }
    }
}