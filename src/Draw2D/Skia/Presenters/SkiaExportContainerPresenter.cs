﻿using Draw2D.Renderers;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters;

public class SkiaExportContainerPresenter : IContainerPresenter
{
    private readonly IToolContext _context;
    private readonly IContainerView _view;

    public SkiaExportContainerPresenter(IToolContext context, IContainerView view)
    {
        _context = context;
        _view = view;
    }

    public void Dispose()
    {
    }

    public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling)
    {
        using var renderer = new SkiaShapeRenderer(_context, _view, _view.SelectionState);
        using var disposable = new CompositeDisposable();
        using var background = SkiaUtil.ToSKPaint(_view.PrintBackground, null, zx, disposable.Disposables);
        var canvas = context as SKCanvas;
        canvas.DrawRect(SkiaUtil.ToSKRect(dx, dy, _view.Width + dx, _view.Height + dy), background);
        _view.CurrentContainer.Draw(canvas, renderer, dx, dy, zx, null, null);
    }
}