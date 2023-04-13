using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Tools;

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

    [IgnoreDataMember]
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

    private bool CompareToolTitle(ITool tool, string title)
    {
        if (tool == null || string.IsNullOrEmpty(tool.Title) || string.IsNullOrEmpty(title))
        {
            return false;
        }
        return string.Compare(tool.Title, title, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public void SetTool(string title)
    {
        if (CurrentTool is PathTool pathTool && !CompareToolTitle(pathTool.Settings.CurrentTool, title))
        {
            pathTool.CleanCurrentTool(this);
            var tool = pathTool.Settings.Tools.Where(t => CompareToolTitle(t, title)).FirstOrDefault();
            if (tool != null)
            {
                pathTool.Settings.CurrentTool = tool;
            }
            else
            {
                CurrentTool = Tools.Where(t => CompareToolTitle(t, title)).FirstOrDefault();
            }
        }
        else
        {
            CurrentTool = Tools.Where(t => CompareToolTitle(t, title)).FirstOrDefault();
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
        return _documentContainer?.ContainerView?.Width ?? 0.0;
    }

    public double GetHeight()
    {
        return _documentContainer?.ContainerView?.Height ?? 0.0;
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