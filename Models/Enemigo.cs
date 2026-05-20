using System.Windows;
using System.Windows.Shapes;

namespace TowerDefenseWPF.Models;

public class Enemigo
{
    public TipoEnemigo Tipo { get; init; }
    public double VidaMaxima { get; init; }
    public double Vida { get; set; }
    public double VelocidadBase { get; init; }
    public int Recompensa { get; init; }
    public double Radio { get; init; }

    public Point Posicion { get; set; }
    public int SiguientePuntoControlIndex { get; set; } = 1;

    public double FactorRalenti { get; set; } = 1.0;
    public double TiempoRalentiRestante { get; set; }

    public bool EstaMuerto => Vida <= 0;
    public bool LlegoAlFinal { get; set; }

    public Ellipse Cuerpo { get; set; } = null!;
    public Rectangle FondoBarraVida { get; set; } = null!;
    public Rectangle BarraVida { get; set; } = null!;

    public double VelocidadActual => VelocidadBase * FactorRalenti;
}
