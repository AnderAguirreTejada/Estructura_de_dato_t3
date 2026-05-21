using System.Windows;
using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Camino del mapa definido como una Lista propia de waypoints.
/// Los enemigos siguen estos puntos en orden, segmento a segmento.
/// Empieza y termina fuera del lienzo (1000x600) para que entren y salgan suavemente.
/// </summary>
public static class DatosMapa
{
    public const double AnchuraLienzo = 1000;
    public const double AltoLienzo = 600;

    public static readonly Lista<Point> PuntosControl = CrearPuntos();

    private static Lista<Point> CrearPuntos()
    {
        var lista = new Lista<Point>();
        lista.Agregar(new Point(-30,  80));
        lista.Agregar(new Point(200,  80));
        lista.Agregar(new Point(200, 300));
        lista.Agregar(new Point(500, 300));
        lista.Agregar(new Point(500, 130));
        lista.Agregar(new Point(800, 130));
        lista.Agregar(new Point(800, 480));
        lista.Agregar(new Point(1030, 480));
        return lista;
    }

    public static bool EstáEnCamino(Point p, double tolerancia = 30)
    {
        for (int i = 0; i < PuntosControl.Cantidad - 1; i++)
        {
            if (DistanciaAlSegmento(p, PuntosControl[i], PuntosControl[i + 1]) <= tolerancia)
                return true;
        }
        return false;
    }

    private static double DistanciaAlSegmento(Point p, Point a, Point b)
    {
        double dx = b.X - a.X;
        double dy = b.Y - a.Y;
        double lenSq = dx * dx + dy * dy;
        if (lenSq < 0.0001) return Distancia(p, a);
        double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lenSq;
        t = Math.Clamp(t, 0, 1);
        var proj = new Point(a.X + t * dx, a.Y + t * dy);
        return Distancia(p, proj);
    }

    private static double Distancia(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
