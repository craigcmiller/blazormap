﻿using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class DiagnosticsDataLayer : IDataLayer
{
    readonly SKPaint _textPaint = new SKPaint
    {
        Style = SKPaintStyle.Fill,
        IsAntialias = true,
        Color = SKColors.Black
    };

    public void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter)
    {
        converter.CanvasToLatLon(0, 0, out double topLat, out double leftLon);

        canvas.DrawText($"{topLat:0.00} {leftLon:0.00}", new SKPoint(20, 20), _textPaint);

        converter.ProjectedToCanvas(converter.ProjectedLeft + converter.ProjectedWidth / 2.0, converter.ProjectedBottom, out float x, out float y);
        //Console.WriteLine($"{x}, {y}");
        canvas.DrawText($"PRJY", new SKPoint(x, y), _textPaint);
    }
}