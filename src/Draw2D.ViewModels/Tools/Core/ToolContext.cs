// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class ToolContext : ViewModelBase, IToolContext
    {
        public static EditMode[] EditModeValues { get; } = (EditMode[])Enum.GetValues(typeof(EditMode));

        private IDocumentContainer _documentContainer;
        private IHitTest _hitTest;
        private IPathConverter _pathConverter;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private EditMode _editMode;

        [IgnoreDataMember]
        public IDocumentContainer DocumentContainer
        {
            get => _documentContainer;
            set => Update(ref _documentContainer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathConverter PathConverter
        {
            get => _pathConverter;
            set => Update(ref _pathConverter, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EditMode EditMode
        {
            get => _editMode;
            set => Update(ref _editMode, value);
        }

        public void Dispose()
        {
            _documentContainer?.Dispose();
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
