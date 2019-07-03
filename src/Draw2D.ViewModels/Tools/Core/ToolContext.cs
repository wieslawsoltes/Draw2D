// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        private IStyleLibrary _styleLibrary;
        private IGroupLibrary _groupLibrary;
        private IBaseShape _pointTemplate;
        private IHitTest _hitTest;
        private IPathConverter _pathConverter;
        private IList<IContainerView> _containerViews;
        private IContainerView _containerView;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private EditMode _editMode;

#if USE_SERIALIZE_STYLES
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
#else
        [IgnoreDataMember]
#endif
        public IStyleLibrary StyleLibrary
        {
            get => _styleLibrary;
            set => Update(ref _styleLibrary, value);
        }

#if USE_SERIALIZE_GROUPS
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
#else
        [IgnoreDataMember]
#endif
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

        public void SetTool(string title)
        {
            if (CurrentTool is PathTool pathTool && pathTool.Settings.CurrentTool.Title != title)
            {
                pathTool.CleanCurrentTool(this);
                var tool = pathTool.Settings.Tools.Where(t => t.Title == title).FirstOrDefault();
                if (tool != null)
                {
                    pathTool.Settings.CurrentTool = tool;
                }
                else
                {
                    CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
                }
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
            }
        }

        public void LeftDown(double x, double y, Modifier modifier)
        {
            _currentTool.LeftDown(this, x, y, modifier);
        }

        public void LeftUp(double x, double y, Modifier modifier)
        {
            if (_editMode == EditMode.Mouse)
            {
                _currentTool.LeftUp(this, x, y, modifier);
            }
            else if (_editMode == EditMode.Touch)
            {
                _currentTool.LeftDown(this, x, y, modifier);
            }
        }

        public void RightDown(double x, double y, Modifier modifier)
        {
            _currentTool.RightDown(this, x, y, modifier);
        }

        public void RightUp(double x, double y, Modifier modifier)
        {
            _currentTool.RightUp(this, x, y, modifier);
        }

        public void Move(double x, double y, Modifier modifier)
        {
            _currentTool.Move(this, x, y, modifier);
        }

        public double GetWidth()
        {
            return ContainerView?.Width ?? 0.0;
        }

        public double GetHeight()
        {
            return ContainerView?.Height ?? 0.0;
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
