using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class MatrixConvolutionImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public MatrixConvolutionImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new MatrixConvolutionImageFilter()
        {
            Title = this.Title
        };
    }
}