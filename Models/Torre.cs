using System.Windows;
using System.Windows.Shapes;
using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;

public class Torre
{
    public TipoTorre Tipo { get; init; }
    public Point Posicion { get; init; }

    public double Rango { get; set; }
    public double Daño { get; set; }
    public double VelocidadDisparo { get; set; }
    public double VelocidadProyectil { get; set; }

    public bool EsExplosivo { get; set; }
    public double RadioExplosion { get; set; }

    public double CantidadRalenti { get; set; }
    public double DuracionRalenti { get; set; }

    public int OroTotalInvertido { get; set; }
    public double EnfriamientoDisparo { get; set; }

    public ArbolBinario<NodoMejora> ArbolMejoras { get; set; } = null!;
    public NodoArbolBinario<NodoMejora>? NodoActual { get; set; }

    public Shape Cuerpo { get; set; } = null!;
}
