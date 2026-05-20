using System.Windows;
using System.Windows.Shapes;

namespace TowerDefenseWPF.Models;

public class Proyectil
{
    public Point Posicion { get; set; }
    public Enemigo Objetivo { get; init; } = null!;
    public Point PosicionObjetivo { get; set; }

    public double Velocidad { get; init; }
    public double Daño { get; init; }

    public bool EsExplosivo { get; init; }
    public double RadioExplosion { get; init; }

    public double CantidadRalenti { get; init; }
    public double DuracionRalenti { get; init; }

    public bool EstaTerminado { get; set; }

    public Shape Cuerpo { get; set; } = null!;
}
