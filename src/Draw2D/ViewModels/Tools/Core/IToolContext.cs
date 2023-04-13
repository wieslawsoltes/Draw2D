using System;
using System.Collections.Generic;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Tools;

public interface IToolContext : IInputTarget, IDisposable
{
    IDocumentContainer DocumentContainer { get; set; }
    IHitTest HitTest { get; set; }
    IPathConverter PathConverter { get; set; }
    IList<ITool> Tools { get; set; }
    ITool CurrentTool { get; set; }
    EditMode EditMode { get; set; }
    void SetTool(string title);
}