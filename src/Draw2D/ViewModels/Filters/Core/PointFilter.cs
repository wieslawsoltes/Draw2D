using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Filters
{
    [DataContract(IsReference = true)]
    public abstract class PointFilter : ViewModelBase, IPointFilter
    {
        private IList<IBaseShape> _guides;

        public new abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IBaseShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(guide);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(guide);
            }
            Guides.Clear();
        }
    }
}
