// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class DocumentContainer : ViewModelBase, IDocumentContainer
    {
        private IStyleLibrary _styleLibrary;
        private IGroupLibrary _groupLibrary;
        private IBaseShape _pointTemplate;
        private IList<IContainerView> _containerViews;
        private IContainerView _containerView;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IStyleLibrary StyleLibrary
        {
            get => _styleLibrary;
            set => Update(ref _styleLibrary, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IGroupLibrary GroupLibrary
        {
            get => _groupLibrary;
            set => Update(ref _groupLibrary, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape PointTemplate
        {
            get => _pointTemplate;
            set => Update(ref _pointTemplate, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IContainerView> ContainerViews
        {
            get => _containerViews;
            set => Update(ref _containerViews, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IContainerView ContainerView
        {
            get => _containerView;
            set => Update(ref _containerView, value);
        }

        public void Dispose()
        {
            if (_containerViews != null)
            {
                foreach (var containerView in _containerViews)
                {
                    containerView.ContainerPresenter?.Dispose();
                    containerView.ContainerPresenter = null;
                    containerView.SelectionState = null;
                    containerView.WorkingContainer = null;
                }
            }
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new ToolContext()
            {
                Name = this.Name
            };

            return copy;
        }
    }
}
