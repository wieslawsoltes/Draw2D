using System.Collections.Generic;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters;

public class SkiaExportSelectedPresenter : IContainerPresenter
{
    private readonly IToolContext _context;
    private readonly IContainerView _view;

    public SkiaExportSelectedPresenter(IToolContext context, IContainerView view)
    {
        _context = context;
        _view = view;
    }

    public void Dispose()
    {
    }

    private bool IsAcceptedShape(IBaseShape shape)
    {
        return !(shape is IPointShape || shape is FigureShape);
    }

    public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling)
    {
        using var renderer = new SkiaShapeRenderer(_context, _view, _view.SelectionState);
        using var disposable = new CompositeDisposable();
        using var background = SkiaUtil.ToSKPaint(_view.PrintBackground, null, zx, disposable.Disposables);
        var canvas = context as SKCanvas;
        canvas.DrawRect(SkiaUtil.ToSKRect(dx, dy, _view.Width + dx, _view.Height + dy), background);

        var selected = new List<IBaseShape>(_view.SelectionState?.Shapes);
        foreach (var shape in selected)
        {
            if (IsAcceptedShape(shape))
            {
                shape.Draw(canvas, renderer, dx, dy, zx, null, null);
            }
        }
    }
}