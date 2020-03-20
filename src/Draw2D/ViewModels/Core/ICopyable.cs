using System.Collections.Generic;

namespace Draw2D.ViewModels
{
    public interface ICopyable
    {
        object Copy(Dictionary<object, object> shared);
    }
}
